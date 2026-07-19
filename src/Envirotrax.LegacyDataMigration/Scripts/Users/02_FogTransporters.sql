USE Vepo
GO

BEGIN TRAN

BEGIN TRY

    INSERT INTO Envirotrax2Dev.dbo.MigrationSkippedUsers (UserID, SourceTable, Reason)
    SELECT UserID, 'FogTransporters', 'Duplicate UserID in source table'
    FROM FogTransporters
    GROUP BY UserID
    HAVING COUNT(*) > 1

    INSERT INTO Envirotrax2Dev.dbo.MigrationSkippedUsers (UserID, SourceTable, Reason)
    SELECT UserID, 'FogTransporters', 'UserID already exists in AspNetUsers'
    FROM FogTransporters
    WHERE EXISTS (SELECT 1 FROM Envirotrax2Dev.dbo.AspNetUsers WHERE UserName = FogTransporters.UserID)

    INSERT INTO Envirotrax2Dev.dbo.AspNetUsers
        ([UserName], [NormalizedUserName], [Email], [NormalizedEmail], [LegacyPasswordHash], [EmailConfirmed], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount], [IsSuperUser], [SecurityStamp])
    SELECT FogTransporters.UserID, UPPER(FogTransporters.UserID), FogTransporters.UserID, UPPER(FogTransporters.UserID), FogTransporters.[Password], 1, CASE WHEN ISNULL(FogTransporters.CellNumber, '') = '' THEN NULL ELSE FogTransporters.CellNumber END, CASE WHEN ISNULL(FogTransporters.CellNumber, '') = '' THEN 0 ELSE 1 END, CASE WHEN FogTransporters.DisableTwoFactorAuthentication = 1 THEN 0 ELSE 1 END, 1, 0, 0, NEWID()
    FROM FogTransporters
    LEFT JOIN Envirotrax2Dev.dbo.MigrationSkippedUsers skipped
        ON skipped.UserID = FogTransporters.UserID AND skipped.SourceTable = 'FogTransporters'
    WHERE skipped.UserID IS NULL

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN
    THROW
END CATCH