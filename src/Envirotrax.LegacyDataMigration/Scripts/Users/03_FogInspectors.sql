USE Vepo
GO

BEGIN TRAN

BEGIN TRY

    INSERT INTO Envirotrax2Dev.dbo.MigrationSkippedUsers (UserID, SourceTable, Reason)
    SELECT UserID, 'FogInspectors', 'Duplicate UserID in source table'
    FROM FogInspectors
    GROUP BY UserID
    HAVING COUNT(*) > 1

    INSERT INTO Envirotrax2Dev.dbo.MigrationSkippedUsers (UserID, SourceTable, Reason)
    SELECT UserID, 'FogInspectors', 'UserID already exists in AspNetUsers'
    FROM FogInspectors
    WHERE EXISTS (SELECT 1 FROM Envirotrax2Dev.dbo.AspNetUsers WHERE UserName = FogInspectors.UserID)

    INSERT INTO Envirotrax2Dev.dbo.AspNetUsers
        ([UserName], [NormalizedUserName], [Email], [NormalizedEmail], [LegacyPasswordHash], [EmailConfirmed], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount], [IsSuperUser], [SecurityStamp])
    SELECT FogInspectors.UserID, UPPER(FogInspectors.UserID), FogInspectors.UserID, UPPER(FogInspectors.UserID), FogInspectors.[Password], 1, CASE WHEN ISNULL(FogInspectors.CellNumber, '') = '' THEN NULL ELSE FogInspectors.CellNumber END, CASE WHEN ISNULL(FogInspectors.CellNumber, '') = '' THEN 0 ELSE 1 END, CASE WHEN FogInspectors.DisableTwoFactorAuthentication = 1 THEN 0 ELSE 1 END, 1, 0, 0, NEWID()
    FROM FogInspectors
    LEFT JOIN Envirotrax2Dev.dbo.MigrationSkippedUsers skipped
        ON skipped.UserID = FogInspectors.UserID AND skipped.SourceTable = 'FogInspectors'
    WHERE skipped.UserID IS NULL

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN
    THROW
END CATCH