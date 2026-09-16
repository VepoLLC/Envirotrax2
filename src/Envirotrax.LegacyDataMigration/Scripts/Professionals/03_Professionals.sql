BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedProfessionals (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT accounts.LegacyRecordId, accounts.LegacyCompanyUserId, accounts.SourceTable, 'Master account of the company is missing in V1'
    FROM MigrationLegacyProfessionalAccounts AS accounts
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM MigrationLegacyProfessionalAccounts AS owners
        WHERE owners.LegacyUserId = accounts.LegacyCompanyUserId
            AND owners.IsCompanyOwner = 1
    )
    AND NOT EXISTS
    (
        SELECT 1
        FROM MigrationSkippedProfessionals AS skipped
        WHERE skipped.LegacyRecordId = accounts.LegacyRecordId
            AND skipped.SourceTable = accounts.SourceTable
            AND skipped.Reason = 'Master account of the company is missing in V1'
    )

    ;WITH CompanyAggregates AS
    (
        SELECT LegacyCompanyUserId,
            MAX(CASE WHEN LegacyUserType = 2 THEN 1 ELSE 0 END) AS HasBackflowTesting,
            MAX(CASE WHEN LegacyUserType = 4 THEN 1 ELSE 0 END) AS HasCsiInspection,
            MAX(CASE WHEN LegacyUserType = 5 THEN 1 ELSE 0 END) AS HasFogTransportation,
            MAX(CASE WHEN LegacyUserType = 6 THEN 1 ELSE 0 END) AS HasFogInspection,
            SUM(ISNULL(AccountBalance, 0)) AS AccountBalance,
            ISNULL(MIN(CreationDate), GETUTCDATE()) AS CreationDate
        FROM MigrationLegacyProfessionalAccounts
        GROUP BY LegacyCompanyUserId
    ),
    CompanyOwners AS
    (
        -- A company can hold a master account in several V1 tables at once. The company fields come from
        -- one of them, picked in this order: BPAT, CSI inspector, FOG transporter, FOG inspector.
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyCompanyUserId ORDER BY accounts.SourcePriority) AS OwnerRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
        WHERE accounts.IsCompanyOwner = 1
    )
    INSERT INTO Professionals
        (LegacyRecordId, LegacyUserId, Name, CompanyEmail, Address, City, StateId, ZipCode,
         PhoneNumber, FaxNumber, WebSiteUrl, HidePublicListing,
         HasWiseGuys, HasBackflowTesting, HasCsiInspection, HasFogInspection, HasFogTransportation,
         AccountBalance, CreatedById, CreatedTime)
    SELECT
        owners.LegacyRecordId,
        owners.LegacyCompanyUserId,
        ISNULL(NULLIF(owners.CompanyName, ''), ISNULL(NULLIF(owners.ContactName, ''), owners.LegacyCompanyUserId)),
        CASE WHEN LEN(ISNULL(NULLIF(owners.EmailAddress, ''), owners.LegacyCompanyUserId)) <= 100
             THEN ISNULL(NULLIF(owners.EmailAddress, ''), owners.LegacyCompanyUserId)
             ELSE NULL
        END,
        NULLIF(owners.Address, ''),
        NULLIF(owners.City, ''),
        states.Id,
        LEFT(NULLIF(owners.ZipCode, ''), 25),
        NULLIF(owners.WorkNumber, ''),
        NULLIF(owners.FaxNumber, ''),
        NULLIF(owners.WebsiteUrl, ''),
        ISNULL(owners.HidePublicListing, 0),
        0,
        companies.HasBackflowTesting,
        companies.HasCsiInspection,
        companies.HasFogInspection,
        companies.HasFogTransportation,
        companies.AccountBalance,
        NULL,
        companies.CreationDate
    FROM CompanyOwners AS owners
    INNER JOIN CompanyAggregates AS companies
        ON companies.LegacyCompanyUserId = owners.LegacyCompanyUserId
    LEFT JOIN States AS states
        ON states.Name = CASE WHEN owners.State = 'International' THEN 'International (Outside the USA)' ELSE owners.State END
    WHERE owners.OwnerRank = 1
        AND NOT EXISTS
        (
            SELECT 1
            FROM Professionals AS alreadyInserted
            WHERE alreadyInserted.LegacyUserId = owners.LegacyCompanyUserId
        )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
