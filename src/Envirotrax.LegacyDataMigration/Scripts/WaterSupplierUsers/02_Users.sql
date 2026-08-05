
BEGIN TRAN

BEGIN TRY

    INSERT INTO WaterSupplierUsers
        (LegacyRecordId, ContactName, EmailAddress, CellNumber, UserId, WaterSupplierId)
    SELECT ID, ContactName, UserId, CellNumber,
        (SELECT Id FROM AspNetUsers WHERE Email = LegacyUsers.UserID),
        (SELECT Id FROM WaterSuppliers WHERE LegacyRecordId = LegacyUsers.WaterSupplierID)
    FROM 
    (
        SELECT Users.* FROM Vepo.dbo.WaterSupplierUserAccounts AS Users
        LEFT JOIN Vepo.dbo.WaterSuppliers AS Suppliers
            ON Suppliers.Id = Users.WaterSupplierID
        LEFT JOIN MigrationSkippedUsers AS SkippedUsers
            ON SkippedUsers.UserID = Users.UserID
        -- Water supplier of the user exists and user record migration is not skipped due to errors.
        WHERE Suppliers.Id IS NOT NULL AND SkippedUsers.UserID IS NULL
    ) AS LegacyUsers
    WHERE NOT EXISTS
    (
        SELECT 1 FROM WaterSupplierUsers
        WHERE LegacyRecordId = LegacyUsers.ID
    );

    COMMIT TRAN;
END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
