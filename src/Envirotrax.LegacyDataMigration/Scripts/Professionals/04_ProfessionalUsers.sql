BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedProfessionals (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT accounts.LegacyRecordId, accounts.LegacyUserId, accounts.SourceTable, 'No AspNetUsers record for the legacy login'
    FROM MigrationLegacyProfessionalAccounts AS accounts
    WHERE NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE AspNetUsers.UserName = accounts.LegacyUserId)
        AND NOT EXISTS
        (
            SELECT 1
            FROM MigrationSkippedProfessionals AS skipped
            WHERE skipped.LegacyRecordId = accounts.LegacyRecordId
                AND skipped.SourceTable = accounts.SourceTable
                AND skipped.Reason = 'No AspNetUsers record for the legacy login'
        )

    ;WITH UserAggregates AS
    (
        SELECT LegacyCompanyUserId, LegacyUserId,
            MAX(CASE WHEN LegacyUserType = 2 THEN 1 ELSE 0 END) AS IsBackflowTester,
            MAX(CASE WHEN LegacyUserType = 4 THEN 1 ELSE 0 END) AS IsCsiInspector,
            MAX(CASE WHEN LegacyUserType = 5 THEN 1 ELSE 0 END) AS IsFogTransporter,
            MAX(CASE WHEN LegacyUserType = 6 THEN 1 ELSE 0 END) AS IsFogInspector,
            -- In V1 the AdminAccount flag is set on a handful of accounts and marks Vepo staff. The company
            -- itself is run by whoever owns the master account, so that owner becomes the admin in V2.
            MAX(CASE WHEN IsCompanyOwner = 1 OR AdminAccount = 1 THEN 1 ELSE 0 END) AS IsAdmin
        FROM MigrationLegacyProfessionalAccounts
        GROUP BY LegacyCompanyUserId, LegacyUserId
    ),
    JobTitles AS
    (
        SELECT LegacyCompanyUserId, LegacyUserId, MAX(JobTitle) AS JobTitle
        FROM MigrationLegacyProfessionalAccounts
        WHERE ISNULL(JobTitle, '') <> ''
        GROUP BY LegacyCompanyUserId, LegacyUserId
    ),
    PrimaryRows AS
    (
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyCompanyUserId, accounts.LegacyUserId ORDER BY accounts.SourcePriority) AS RowRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
    )
    INSERT INTO ProfessionalUsers
        (ProfessionalId, UserId, LegacyRecordId, LegacyUserId, ContactName, JobTitle, IsAdmin,
         IsWiseGuy, IsBackflowTester, IsCsiInspector, IsFogInspector, IsFogTransporter,
         BillingFirstName, BillingLastName, BillingAddress, BillingCity, BillingStateId, BillingZipCode)
    SELECT
        professionals.Id,
        users.Id,
        primaryRows.LegacyRecordId,
        primaryRows.LegacyUserId,
        NULLIF(primaryRows.ContactName, ''),
        LEFT(jobTitles.JobTitle, 150),
        aggregates.IsAdmin,
        0,
        aggregates.IsBackflowTester,
        aggregates.IsCsiInspector,
        aggregates.IsFogInspector,
        aggregates.IsFogTransporter,
        NULLIF(primaryRows.BillingFirstName, ''),
        NULLIF(primaryRows.BillingLastName, ''),
        NULLIF(primaryRows.BillingAddress, ''),
        NULLIF(primaryRows.BillingCity, ''),
        billingStates.Id,
        LEFT(NULLIF(primaryRows.BillingZipCode, ''), 25)
    FROM PrimaryRows AS primaryRows
    INNER JOIN UserAggregates AS aggregates
        ON aggregates.LegacyCompanyUserId = primaryRows.LegacyCompanyUserId
        AND aggregates.LegacyUserId = primaryRows.LegacyUserId
    INNER JOIN Professionals AS professionals
        ON professionals.LegacyUserId = primaryRows.LegacyCompanyUserId
    INNER JOIN AspNetUsers AS users
        ON users.UserName = primaryRows.LegacyUserId
    LEFT JOIN JobTitles AS jobTitles
        ON jobTitles.LegacyCompanyUserId = primaryRows.LegacyCompanyUserId
        AND jobTitles.LegacyUserId = primaryRows.LegacyUserId
    LEFT JOIN States AS billingStates
        ON billingStates.Name = CASE WHEN primaryRows.BillingState = 'International' THEN 'International (Outside the USA)' ELSE primaryRows.BillingState END
    WHERE primaryRows.RowRank = 1
        AND NOT EXISTS
        (
            SELECT 1
            FROM ProfessionalUsers AS alreadyInserted
            WHERE alreadyInserted.ProfessionalId = professionals.Id
                AND alreadyInserted.UserId = users.Id
        )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
