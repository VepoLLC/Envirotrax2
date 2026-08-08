BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedUsers (UserID, SourceTable, Reason)
    SELECT UserID, 'FogTransporters', 'Duplicate UserID in source table'
    FROM Vepo.dbo.FogTransporters
    GROUP BY UserID
    HAVING COUNT(*) > 1

    INSERT INTO MigrationSkippedUsers (UserID, SourceTable, Reason)
    SELECT UserID, 'FogTransporters', 'UserID already exists in AspNetUsers'
    FROM Vepo.dbo.FogTransporters
    WHERE EXISTS (SELECT 1 FROM AspNetUsers WHERE UserName = FogTransporters.UserID)

    INSERT INTO AspNetUsers
        ([UserName], [NormalizedUserName], [Email], [NormalizedEmail], [PasswordHash], [IsMigratedLegacyPasswordHashed], [PasswordExpirationDate], [EmailConfirmed], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount], [IsSuperUser], [SecurityStamp])
    SELECT FogTransporters.UserID, UPPER(FogTransporters.UserID), FogTransporters.UserID, UPPER(FogTransporters.UserID), FogTransporters.[Password], 0, GETUTCDATE(), 1, CASE WHEN ISNULL(FogTransporters.CellNumber, '') = '' THEN NULL ELSE FogTransporters.CellNumber END, CASE WHEN ISNULL(FogTransporters.CellNumber, '') = '' THEN 0 ELSE 1 END, CASE WHEN FogTransporters.DisableTwoFactorAuthentication = 1 THEN 0 ELSE 1 END, 1, 0, 0, NEWID()
    FROM Vepo.dbo.FogTransporters
    LEFT JOIN MigrationSkippedUsers skipped
        ON skipped.UserID = FogTransporters.UserID AND skipped.SourceTable = 'FogTransporters'
    WHERE skipped.UserID IS NULL

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH