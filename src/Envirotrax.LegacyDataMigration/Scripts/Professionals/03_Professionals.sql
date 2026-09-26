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

    -- A login can hold more than one row in the same V1 table, and V1 works off whichever row it reads
    -- first: VepoUserAccount loads one, and the balance it shows is that row's. Adding them up would
    -- invent money the professional never had, so each member counts once. The company stays in the
    -- partition because the same login can be its own company in one row and somebody else's
    -- sub-account in another, and both of those companies are real.
    ;WITH AccountsWithoutDuplicates AS
    (
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyCompanyUserId, accounts.LegacyUserId, accounts.LegacyUserType ORDER BY accounts.LegacyRecordId) AS AccountRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
    ),
    CompanyAggregates AS
    (
        SELECT LegacyCompanyUserId,
            MAX(CASE WHEN LegacyUserType = 2 THEN 1 ELSE 0 END) AS HasBackflowTesting,
            MAX(CASE WHEN LegacyUserType = 4 THEN 1 ELSE 0 END) AS HasCsiInspection,
            MAX(CASE WHEN LegacyUserType = 5 THEN 1 ELSE 0 END) AS HasFogTransportation,
            MAX(CASE WHEN LegacyUserType = 6 THEN 1 ELSE 0 END) AS HasFogInspection,
            SUM(CASE WHEN AccountRank = 1 THEN ISNULL(AccountBalance, 0) ELSE 0 END) AS AccountBalance,
            ISNULL(MIN(CreationDate), GETUTCDATE()) AS CreationDate
        FROM AccountsWithoutDuplicates
        GROUP BY LegacyCompanyUserId
    ),
    CompanyOwners AS
    (
        -- A company can hold a master account in several V1 tables at once. The company fields come from
        -- one of them, picked in this order: BPAT, CSI inspector, FOG transporter, FOG inspector.
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyCompanyUserId ORDER BY accounts.SourcePriority, accounts.LegacyRecordId) AS OwnerRank
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
