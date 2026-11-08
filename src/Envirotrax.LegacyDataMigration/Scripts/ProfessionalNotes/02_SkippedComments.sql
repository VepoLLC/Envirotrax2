BEGIN TRAN

BEGIN TRY

    ;WITH AccountsWithoutDuplicates AS
    (
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyUserId, accounts.LegacyUserType ORDER BY accounts.LegacyRecordId) AS AccountRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
    ),
    -- V1 shows a comment to every water supplier its author is a user of, and Vepo administrators are
    -- users of supplier 0, which V2 has as the Vepo LLC supplier (WaterSupplier.EnvirotraxAdminDomain).
    AuthorWaterSuppliers AS
    (
        SELECT DISTINCT supplierUsers.UserID AS LegacyUserId, waterSuppliers.Id AS WaterSupplierId
        FROM Vepo.dbo.WaterSupplierUserAccounts AS supplierUsers
        INNER JOIN WaterSuppliers AS waterSuppliers
            ON (supplierUsers.WaterSupplierID = 0 AND waterSuppliers.Domain = 'vepollc')
            OR (supplierUsers.WaterSupplierID <> 0 AND waterSuppliers.LegacyRecordId = supplierUsers.WaterSupplierID)
    ),
    Comments AS
    (
        SELECT
            comments.ID,
            comments.UserID,
            CASE
                -- The Management Studio writes an empty comment as soon as Add is clicked, so an empty
                -- one is a note nobody wrote rather than one that was cleared.
                -- Line breaks and tabs count as empty too: V2 requires a note text that is not blank.
                WHEN TRIM(CHAR(9) + CHAR(10) + CHAR(13) + ' ' FROM CAST(comments.Comment AS NVARCHAR(MAX))) = '' THEN 'Empty comment'
                -- A sub-account deleted in V1 leaves its comments behind, and so lands here as well.
                WHEN professionals.Id IS NULL THEN 'No professional account for the comment'
                WHEN accounts.IsCompanyOwner = 0 AND professionalUsers.UserId IS NULL THEN 'No professional user for the sub-account comment'
                WHEN NOT EXISTS
                (
                    SELECT 1
                    FROM AuthorWaterSuppliers AS authorSuppliers
                    WHERE authorSuppliers.LegacyUserId = comments.SubmitterUserID
                ) THEN 'No migrated water supplier for the comment author'
            END AS Reason
        FROM Vepo.dbo.SaveAccountCommentLog AS comments
        LEFT JOIN AccountsWithoutDuplicates AS accounts
            ON accounts.LegacyUserId = comments.UserID
            AND accounts.LegacyUserType = comments.Type
            AND accounts.AccountRank = 1
        LEFT JOIN Professionals AS professionals
            ON professionals.LegacyUserId = accounts.LegacyCompanyUserId
        LEFT JOIN AspNetUsers AS users
            ON users.UserName = comments.UserID
        LEFT JOIN ProfessionalUsers AS professionalUsers
            ON professionalUsers.ProfessionalId = professionals.Id
            AND professionalUsers.UserId = users.Id
    )
    INSERT INTO MigrationSkippedProfessionals (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT comments.ID, comments.UserID, 'SaveAccountCommentLog', comments.Reason
    FROM Comments AS comments
    WHERE comments.Reason IS NOT NULL
        AND NOT EXISTS
        (
            SELECT 1
            FROM MigrationSkippedProfessionals AS skipped
            WHERE skipped.LegacyRecordId = comments.ID
                AND skipped.SourceTable = 'SaveAccountCommentLog'
        )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
