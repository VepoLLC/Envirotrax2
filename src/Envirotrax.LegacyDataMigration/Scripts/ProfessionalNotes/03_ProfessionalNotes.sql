BEGIN TRAN

BEGIN TRY

    ;WITH AccountsWithoutDuplicates AS
    (
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyUserId, accounts.LegacyUserType ORDER BY accounts.LegacyRecordId) AS AccountRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
    ),
    -- Same rule as in 02_SkippedComments.sql: one note per water supplier the author is a user of.
    AuthorWaterSuppliers AS
    (
        SELECT DISTINCT supplierUsers.UserID AS LegacyUserId, waterSuppliers.Id AS WaterSupplierId
        FROM Vepo.dbo.WaterSupplierUserAccounts AS supplierUsers
        INNER JOIN WaterSuppliers AS waterSuppliers
            ON (supplierUsers.WaterSupplierID = 0 AND waterSuppliers.Domain = 'vepollc')
            OR (supplierUsers.WaterSupplierID <> 0 AND waterSuppliers.LegacyRecordId = supplierUsers.WaterSupplierID)
    )
    INSERT INTO ProfessionalNotes
        (LegacyRecordId, WaterSupplierId, ProfessionalId, ProfessionalType, NoteText, CreatedById, CreatedTime)
    SELECT
        comments.ID,
        authorSuppliers.WaterSupplierId,
        professionals.Id,
        comments.Type,
        CAST(comments.Comment AS NVARCHAR(MAX)),
        authors.Id,
        comments.CreationDate
    FROM Vepo.dbo.SaveAccountCommentLog AS comments
    INNER JOIN AccountsWithoutDuplicates AS accounts
        ON accounts.LegacyUserId = comments.UserID
        AND accounts.LegacyUserType = comments.Type
        AND accounts.AccountRank = 1
        AND accounts.IsCompanyOwner = 1
    INNER JOIN Professionals AS professionals
        ON professionals.LegacyUserId = accounts.LegacyCompanyUserId
    INNER JOIN AuthorWaterSuppliers AS authorSuppliers
        ON authorSuppliers.LegacyUserId = comments.SubmitterUserID
    LEFT JOIN AspNetUsers AS authors
        ON authors.UserName = comments.SubmitterUserID
    WHERE TRIM(CHAR(9) + CHAR(10) + CHAR(13) + ' ' FROM CAST(comments.Comment AS NVARCHAR(MAX))) <> ''
        AND NOT EXISTS
        (
            SELECT 1
            FROM ProfessionalNotes AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = comments.ID
                AND alreadyInserted.WaterSupplierId = authorSuppliers.WaterSupplierId
        )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
