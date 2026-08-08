BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedUsers (UserID, SourceTable, Reason)
    SELECT UserID, 'CsiInspectors', 'Duplicate UserID in source table'
    FROM Vepo.dbo.CsiInspectors
    GROUP BY UserID
    HAVING COUNT(*) > 1

    INSERT INTO MigrationSkippedUsers (UserID, SourceTable, Reason)
    SELECT UserID, 'CsiInspectors', 'UserID already exists in AspNetUsers'
    FROM Vepo.dbo.CsiInspectors
    WHERE EXISTS (SELECT 1 FROM AspNetUsers WHERE UserName = CsiInspectors.UserID)

    INSERT INTO AspNetUsers
        ([UserName], [NormalizedUserName], [Email], [NormalizedEmail], [PasswordHash], [IsMigratedLegacyPasswordHashed], [PasswordExpirationDate], [EmailConfirmed], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount], [IsSuperUser], [SecurityStamp])
    SELECT CsiInspectors.UserID, UPPER(CsiInspectors.UserID), CsiInspectors.UserID, UPPER(CsiInspectors.UserID), CsiInspectors.[Password], 0, GETUTCDATE(), 1, CASE WHEN ISNULL(CsiInspectors.CellNumber, '') = '' THEN NULL ELSE CsiInspectors.CellNumber END, CASE WHEN ISNULL(CsiInspectors.CellNumber, '') = '' THEN 0 ELSE 1 END, CASE WHEN CsiInspectors.DisableTwoFactorAuthentication = 1 THEN 0 ELSE 1 END, 1, 0, 0, NEWID()
    FROM Vepo.dbo.CsiInspectors
    LEFT JOIN MigrationSkippedUsers skipped
        ON skipped.UserID = CsiInspectors.UserID AND skipped.SourceTable = 'CsiInspectors'
    WHERE skipped.UserID IS NULL

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
