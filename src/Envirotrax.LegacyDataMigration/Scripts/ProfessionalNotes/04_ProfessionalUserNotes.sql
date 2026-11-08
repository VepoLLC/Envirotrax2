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
    INSERT INTO ProfessionalUserNotes
        (LegacyRecordId, WaterSupplierId, ProfessionalId, UserId, ProfessionalType, NoteText, CreatedById, CreatedTime)
    SELECT
        comments.ID,
        authorSuppliers.WaterSupplierId,
        professionalUsers.ProfessionalId,
        professionalUsers.UserId,
        comments.Type,
        CAST(comments.Comment AS NVARCHAR(MAX)),
        authors.Id,
        comments.CreationDate
    FROM Vepo.dbo.SaveAccountCommentLog AS comments
    INNER JOIN AccountsWithoutDuplicates AS accounts
        ON accounts.LegacyUserId = comments.UserID
        AND accounts.LegacyUserType = comments.Type
        AND accounts.AccountRank = 1
        AND accounts.IsCompanyOwner = 0
    INNER JOIN Professionals AS professionals
        ON professionals.LegacyUserId = accounts.LegacyCompanyUserId
    INNER JOIN AspNetUsers AS users
        ON users.UserName = comments.UserID
    INNER JOIN ProfessionalUsers AS professionalUsers
        ON professionalUsers.ProfessionalId = professionals.Id
        AND professionalUsers.UserId = users.Id
    INNER JOIN AuthorWaterSuppliers AS authorSuppliers
        ON authorSuppliers.LegacyUserId = comments.SubmitterUserID
    LEFT JOIN AspNetUsers AS authors
        ON authors.UserName = comments.SubmitterUserID
    WHERE TRIM(CHAR(9) + CHAR(10) + CHAR(13) + ' ' FROM CAST(comments.Comment AS NVARCHAR(MAX))) <> ''
        AND NOT EXISTS
        (
            SELECT 1
            FROM ProfessionalUserNotes AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = comments.ID
                AND alreadyInserted.WaterSupplierId = authorSuppliers.WaterSupplierId
        )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
