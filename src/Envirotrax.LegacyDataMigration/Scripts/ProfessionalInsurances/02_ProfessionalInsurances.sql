BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedProfessionals (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT licenses.ID, licenses.UserID, 'SaveLicenses', 'No professional account for the insurance policy'
    FROM Vepo.dbo.SaveLicenses AS licenses
    WHERE licenses.Type = 'Insurance Policy'
        AND NOT EXISTS
        (
            SELECT 1
            FROM MigrationLegacyProfessionalAccounts AS accounts
            WHERE accounts.LegacyUserId = licenses.UserID
                AND accounts.LegacyUserType = licenses.UserType
        )
        AND NOT EXISTS
        (
            SELECT 1
            FROM MigrationSkippedProfessionals AS skipped
            WHERE skipped.LegacyRecordId = licenses.ID
                AND skipped.SourceTable = 'SaveLicenses'
        )

    -- A policy file lives on the legacy file server only when ImageStored is 1. V1 can also serve one
    -- out of SaveLicenses.FileData (water_suppliers/license_view.aspx.vb), but that column is empty
    -- throughout, so the rest of the policies have no document at all - in V1 either. They still
    -- migrate; this records that they arrive without a file, rather than leaving it to be noticed later.
    INSERT INTO MigrationSkippedProfessionals (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT licenses.ID, licenses.UserID, 'SaveLicenses', 'Policy has no file in V1'
    FROM Vepo.dbo.SaveLicenses AS licenses
    WHERE licenses.Type = 'Insurance Policy'
        AND (ISNULL(licenses.ImageStored, 0) = 0 OR ISNULL(licenses.FileType, '') = '')
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
    INSERT INTO ProfessionalInsurances
        (LegacyRecordId, ProfessionalId, InsuranceNumber, ExpirationDate, CoverageAmount, FilePath, LegacyFilePath, CreatedById, CreatedTime)
    SELECT
        licenses.ID,
        professionals.Id,
        -- V2 requires the number, V1 does not always hold one. An empty string keeps the policy itself,
        -- with the expiration date that compliance is actually checked against.
        LEFT(ISNULL(licenses.Number, ''), 50),
        licenses.ExpirationDate,
        -- Not every V1 page writes InsuranceCoverage when a policy is added, so zero there means the
        -- amount was never filled in rather than a policy that covers nothing. It becomes NULL, which
        -- is how V2 tells 'unknown' from an amount that really is below what the water supplier asks.
        NULLIF(licenses.InsuranceCoverage, 0),
        '',
        -- V1 stored policy files in folders derived from the record id: licenses/<ID rounded down to 10000>/<ID>.<pdf|jpg>.
        CASE
            WHEN licenses.ImageStored = 1 AND ISNULL(licenses.FileType, '') <> ''
            THEN CONCAT('licenses/', CAST(FLOOR(licenses.ID / 10000) * 10000 AS VARCHAR(20)), '/', CAST(licenses.ID AS VARCHAR(20)),
                        CASE WHEN licenses.FileType = '.pdf' THEN '.pdf' ELSE '.jpg' END)
        END,
        NULL,
        -- No V1 page fills CreationDate in when a policy is added, so it can be NULL. The account the
        -- policy belongs to is the closest date that is certainly not later than the policy itself.
        COALESCE(licenses.CreationDate, accounts.CreationDate, GETUTCDATE())
    FROM Vepo.dbo.SaveLicenses AS licenses
    INNER JOIN AccountsWithoutDuplicates AS accounts
        ON accounts.LegacyUserId = licenses.UserID
        AND accounts.LegacyUserType = licenses.UserType
        AND accounts.AccountRank = 1
    INNER JOIN Professionals AS professionals
        ON professionals.LegacyUserId = accounts.LegacyCompanyUserId
    WHERE licenses.Type = 'Insurance Policy'
        AND NOT EXISTS
        (
            SELECT 1
            FROM ProfessionalInsurances AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = licenses.ID
        )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
