BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedUsers (UserID, SourceTable, Reason)
    SELECT UserID, 'FogInspectors', 'Duplicate UserID in source table'
    FROM Vepo.dbo.FogInspectors
    GROUP BY UserID
    HAVING COUNT(*) > 1

    INSERT INTO MigrationSkippedUsers (UserID, SourceTable, Reason)
    SELECT UserID, 'FogInspectors', 'UserID already exists in AspNetUsers'
    FROM Vepo.dbo.FogInspectors
    WHERE EXISTS (SELECT 1 FROM AspNetUsers WHERE UserName = FogInspectors.UserID)

    INSERT INTO AspNetUsers
        ([UserName], [NormalizedUserName], [Email], [NormalizedEmail], [PasswordHash], [IsMigratedLegacyPasswordHashed], [PasswordExpirationDate], [EmailConfirmed], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount], [IsSuperUser], [SecurityStamp])
    SELECT FogInspectors.UserID, UPPER(FogInspectors.UserID), FogInspectors.UserID, UPPER(FogInspectors.UserID), FogInspectors.[Password], 0, GETUTCDATE(), 1, CASE WHEN ISNULL(FogInspectors.CellNumber, '') = '' THEN NULL ELSE FogInspectors.CellNumber END, CASE WHEN ISNULL(FogInspectors.CellNumber, '') = '' THEN 0 ELSE 1 END, CASE WHEN FogInspectors.DisableTwoFactorAuthentication = 1 THEN 0 ELSE 1 END, 1, 0, 0, NEWID()
    FROM Vepo.dbo.FogInspectors
    LEFT JOIN MigrationSkippedUsers skipped
        ON skipped.UserID = FogInspectors.UserID AND skipped.SourceTable = 'FogInspectors'
    WHERE skipped.UserID IS NULL

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH