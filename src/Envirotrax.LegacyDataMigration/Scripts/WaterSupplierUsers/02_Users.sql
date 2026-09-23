
BEGIN TRAN

BEGIN TRY

    INSERT INTO WaterSupplierUsers
        (LegacyRecordId, ContactName, EmailAddress, UserId, WaterSupplierId)
    SELECT ID, ContactName, UserId,
        (SELECT Id FROM AspNetUsers WHERE Email = LegacyUsers.UserID),
        (SELECT Id FROM WaterSuppliers WHERE LegacyRecordId = LegacyUsers.WaterSupplierID)
    FROM 
    (
        SELECT Users.* FROM Vepo.dbo.WaterSupplierUserAccounts AS Users
        LEFT JOIN Vepo.dbo.WaterSuppliers AS Suppliers
            ON Suppliers.Id = Users.WaterSupplierID
        LEFT JOIN AspNetUsers AS ExistingUsers
            ON ExistingUsers.Email = Users.UserID
        -- Water supplier of the user exists and user record migration is not skipped due to errors.
        WHERE Suppliers.Id IS NOT NULL AND ExistingUsers.Id IS NOT NULL
    ) AS LegacyUsers
    WHERE NOT EXISTS
    (
        SELECT 1 FROM WaterSupplierUsers
        WHERE LegacyRecordId = LegacyUsers.ID
    )

    COMMIT TRAN;
END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
