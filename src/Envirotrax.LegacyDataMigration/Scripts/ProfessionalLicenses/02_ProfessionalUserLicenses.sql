BEGIN TRAN

BEGIN TRY

    -- In V1 the license type is a free string, and the list offered depended on the state of the account
    -- (WaterSupplierLicensing.GetLicensesForState). In V2 it is the ProfessionalLicenseTypes lookup, and
    -- the name alone identifies the row: every seeded name belongs to exactly one state. Matching on the
    -- state as well would only drop licenses an account carried over from the state it used to work in.
    ;WITH AccountsWithoutDuplicates AS
    (
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyUserId, accounts.LegacyUserType ORDER BY accounts.LegacyRecordId) AS AccountRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
    ),
    Licenses AS
    (
        SELECT
            licenses.ID,
            licenses.UserID,
            licenses.UserType,
            licenses.Type,
            licenses.Number,
            licenses.ExpirationDate,
            licenses.CreationDate,
            accounts.LegacyCompanyUserId,
            users.Id AS AspNetUserId,
            licenseTypes.Id AS LicenseTypeId
        FROM Vepo.dbo.SaveLicenses AS licenses
        LEFT JOIN AccountsWithoutDuplicates AS accounts
            ON accounts.LegacyUserId = licenses.UserID
            AND accounts.LegacyUserType = licenses.UserType
            AND accounts.AccountRank = 1
        LEFT JOIN AspNetUsers AS users
            ON users.UserName = licenses.UserID
        LEFT JOIN ProfessionalLicenseTypes AS licenseTypes
            ON licenseTypes.Name = licenses.Type
            AND licenseTypes.ProfessionalType = licenses.UserType
        WHERE licenses.Type <> 'Insurance Policy'
    )
    INSERT INTO MigrationSkippedProfessionals (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT licenses.ID, licenses.UserID, 'SaveLicenses',
        CASE
            WHEN licenses.LegacyCompanyUserId IS NULL THEN 'No professional account for the license'
            WHEN licenses.AspNetUserId IS NULL THEN 'No AspNetUsers record for the legacy login'
            ELSE 'License type is not found'
        END
    FROM Licenses AS licenses
    WHERE (licenses.LegacyCompanyUserId IS NULL OR licenses.AspNetUserId IS NULL OR licenses.LicenseTypeId IS NULL)
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
    )
    INSERT INTO ProfessionalUserLicenses
        (LegacyRecordId, ProfessionalId, UserId, ProfessionalType, LicenseTypeId, LicenseNumber, ExpirationDate, CreatedById, CreatedTime)
    SELECT
        licenses.ID,
        professionalUsers.ProfessionalId,
        professionalUsers.UserId,
        licenses.UserType,
        licenseTypes.Id,
        -- V2 requires the number, V1 does not always hold one. An empty string keeps the license itself,
        -- with the type and the expiration date that compliance is actually checked against.
        LEFT(ISNULL(licenses.Number, ''), 50),
        licenses.ExpirationDate,
        NULL,
        -- No V1 page fills CreationDate in when a license is added, so it can be NULL. The account the
        -- license belongs to is the closest date that is certainly not later than the license itself.
        COALESCE(licenses.CreationDate, accounts.CreationDate, GETUTCDATE())
    FROM Vepo.dbo.SaveLicenses AS licenses
    INNER JOIN AccountsWithoutDuplicates AS accounts
        ON accounts.LegacyUserId = licenses.UserID
        AND accounts.LegacyUserType = licenses.UserType
        AND accounts.AccountRank = 1
    INNER JOIN Professionals AS professionals
        ON professionals.LegacyUserId = accounts.LegacyCompanyUserId
    INNER JOIN AspNetUsers AS users
        ON users.UserName = licenses.UserID
    INNER JOIN ProfessionalUsers AS professionalUsers
        ON professionalUsers.ProfessionalId = professionals.Id
        AND professionalUsers.UserId = users.Id
    INNER JOIN ProfessionalLicenseTypes AS licenseTypes
        ON licenseTypes.Name = licenses.Type
        AND licenseTypes.ProfessionalType = licenses.UserType
    WHERE licenses.Type <> 'Insurance Policy'
        AND NOT EXISTS
        (
            SELECT 1
            FROM ProfessionalUserLicenses AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = licenses.ID
        )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
