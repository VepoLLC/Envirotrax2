USE Vepo
GO

BEGIN TRAN

BEGIN TRY

    INSERT INTO Envirotrax2Dev.dbo.MigrationSkippedUsers (UserID, SourceTable, Reason)
    SELECT UserID, 'WaterSupplierUserAccounts', 'Duplicate UserID in source table'
    FROM WaterSupplierUserAccounts
    GROUP BY UserID
    HAVING COUNT(*) > 1

    INSERT INTO Envirotrax2Dev.dbo.MigrationSkippedUsers (UserID, SourceTable, Reason)
    SELECT UserID, 'WaterSupplierUserAccounts', 'UserID already exists in AspNetUsers'
    FROM WaterSupplierUserAccounts
    WHERE EXISTS (SELECT 1 FROM Envirotrax2Dev.dbo.AspNetUsers WHERE UserName = WaterSupplierUserAccounts.UserID)

    INSERT INTO Envirotrax2Dev.dbo.AspNetUsers
        ([UserName], [NormalizedUserName], [Email], [NormalizedEmail], [LegacyPasswordHash], [EmailConfirmed], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount], [IsSuperUser], [SecurityStamp])
    SELECT WaterSupplierUserAccounts.UserID, UPPER(WaterSupplierUserAccounts.UserID), WaterSupplierUserAccounts.UserID, UPPER(WaterSupplierUserAccounts.UserID), WaterSupplierUserAccounts.[Password], 1, CASE WHEN ISNULL(WaterSupplierUserAccounts.CellNumber, '') = '' THEN NULL ELSE WaterSupplierUserAccounts.CellNumber END, CASE WHEN ISNULL(WaterSupplierUserAccounts.CellNumber, '') = '' THEN 0 ELSE 1 END, CASE WHEN WaterSupplierUserAccounts.DisableTwoFactorAuthentication = 1 THEN 0 ELSE 1 END, 1, 0, 0, NEWID()
    FROM WaterSupplierUserAccounts
    LEFT JOIN Envirotrax2Dev.dbo.MigrationSkippedUsers skipped
        ON skipped.UserID = WaterSupplierUserAccounts.UserID AND skipped.SourceTable = 'WaterSupplierUserAccounts'
    WHERE skipped.UserID IS NULL

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN
    THROW
END CATCH