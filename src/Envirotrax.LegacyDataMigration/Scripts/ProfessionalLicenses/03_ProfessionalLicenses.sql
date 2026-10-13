BEGIN TRAN

BEGIN TRY

    ;WITH AccountsWithoutDuplicates AS
    (
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyUserId, accounts.LegacyUserType ORDER BY accounts.LegacyRecordId) AS AccountRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
    )
    INSERT INTO MigrationSkippedProfessionals (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT licenses.ID, licenses.UserID, 'SaveLicenses', 'No professional account for the company license'
    FROM Vepo.dbo.SaveLicenses AS licenses
    INNER JOIN ProfessionalLicenseTypes AS licenseTypes
        ON licenseTypes.Name = licenses.Type
        AND licenseTypes.ProfessionalType = licenses.UserType
        AND licenseTypes.LicenseScope = 1
    LEFT JOIN AccountsWithoutDuplicates AS accounts
        ON accounts.LegacyUserId = licenses.UserID
        AND accounts.LegacyUserType = licenses.UserType
        AND accounts.AccountRank = 1
    LEFT JOIN Professionals AS professionals
        ON professionals.LegacyUserId = accounts.LegacyCompanyUserId
    WHERE professionals.Id IS NULL
        AND NOT EXISTS
        (
            SELECT 1
            FROM MigrationSkippedProfessionals AS skipped
            WHERE skipped.LegacyRecordId = licenses.ID
                AND skipped.SourceTable = 'SaveLicenses'
        )

    ;WITH AccountsWithoutDuplicates AS
    (
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyUserId, accounts.LegacyUserType ORDER BY accounts.LegacyRecordId) AS AccountRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
    ),
    CompanyLicenses AS
    (
        SELECT
            licenses.ID,
            licenses.UserID,
            ROW_NUMBER() OVER
            (
                PARTITION BY professionals.Id, licenseTypes.Id, LTRIM(RTRIM(ISNULL(licenses.Number, '')))
                ORDER BY licenses.ExpirationDate DESC, licenses.ID
            ) AS LicenseRank
        FROM Vepo.dbo.SaveLicenses AS licenses
        INNER JOIN ProfessionalLicenseTypes AS licenseTypes
            ON licenseTypes.Name = licenses.Type
            AND licenseTypes.ProfessionalType = licenses.UserType
            AND licenseTypes.LicenseScope = 1
        INNER JOIN AccountsWithoutDuplicates AS accounts
            ON accounts.LegacyUserId = licenses.UserID
            AND accounts.LegacyUserType = licenses.UserType
            AND accounts.AccountRank = 1
        INNER JOIN Professionals AS professionals
            ON professionals.LegacyUserId = accounts.LegacyCompanyUserId
    )
    INSERT INTO MigrationSkippedProfessionals (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT companyLicenses.ID, companyLicenses.UserID, 'SaveLicenses', 'The same company license is registered on another account of the company'
    FROM CompanyLicenses AS companyLicenses
    WHERE companyLicenses.LicenseRank > 1
        AND NOT EXISTS
        (
            SELECT 1
            FROM MigrationSkippedProfessionals AS skipped
            WHERE skipped.LegacyRecordId = companyLicenses.ID
                AND skipped.SourceTable = 'SaveLicenses'
        )

    ;WITH AccountsWithoutDuplicates AS
    (
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyUserId, accounts.LegacyUserType ORDER BY accounts.LegacyRecordId) AS AccountRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
    ),
    CompanyLicenses AS
    (
        SELECT
            licenses.ID,
            licenses.UserType,
            LEFT(ISNULL(licenses.Number, ''), 50) AS LicenseNumber,
            licenses.ExpirationDate,
            COALESCE(licenses.CreationDate, accounts.CreationDate, GETUTCDATE()) AS CreatedTime,
            professionals.Id AS ProfessionalId,
            licenseTypes.Id AS LicenseTypeId,
            ROW_NUMBER() OVER
            (
                PARTITION BY professionals.Id, licenseTypes.Id, LTRIM(RTRIM(ISNULL(licenses.Number, '')))
                ORDER BY licenses.ExpirationDate DESC, licenses.ID
            ) AS LicenseRank
        FROM Vepo.dbo.SaveLicenses AS licenses
        INNER JOIN ProfessionalLicenseTypes AS licenseTypes
            ON licenseTypes.Name = licenses.Type
            AND licenseTypes.ProfessionalType = licenses.UserType
            AND licenseTypes.LicenseScope = 1
        INNER JOIN AccountsWithoutDuplicates AS accounts
            ON accounts.LegacyUserId = licenses.UserID
            AND accounts.LegacyUserType = licenses.UserType
            AND accounts.AccountRank = 1
        INNER JOIN Professionals AS professionals
            ON professionals.LegacyUserId = accounts.LegacyCompanyUserId
    )
    INSERT INTO ProfessionalLicenses
        (LegacyRecordId, ProfessionalId, ProfessionalType, LicenseTypeId, LicenseNumber, ExpirationDate, CreatedById, CreatedTime)
    SELECT
        companyLicenses.ID,
        companyLicenses.ProfessionalId,
        companyLicenses.UserType,
        companyLicenses.LicenseTypeId,
        companyLicenses.LicenseNumber,
        companyLicenses.ExpirationDate,
        NULL,
        companyLicenses.CreatedTime
    FROM CompanyLicenses AS companyLicenses
    WHERE companyLicenses.LicenseRank = 1
        AND NOT EXISTS
        (
            SELECT 1
            FROM ProfessionalLicenses AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = companyLicenses.ID
                OR
                (
                    alreadyInserted.ProfessionalId = companyLicenses.ProfessionalId
                    AND alreadyInserted.LicenseTypeId = companyLicenses.LicenseTypeId
                    AND alreadyInserted.LicenseNumber = companyLicenses.LicenseNumber
                )
        )

    DELETE userLicenses
    FROM ProfessionalUserLicenses AS userLicenses
    INNER JOIN ProfessionalLicenseTypes AS licenseTypes
        ON licenseTypes.Id = userLicenses.LicenseTypeId
    WHERE licenseTypes.LicenseScope = 1
        AND userLicenses.LegacyRecordId IS NOT NULL

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
