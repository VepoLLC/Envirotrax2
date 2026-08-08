BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedUsers (UserID, SourceTable, Reason)
    SELECT UserID, 'SaveBpats', 'Duplicate UserID in source table'
    FROM Vepo.dbo.SaveBpats
    GROUP BY UserID
    HAVING COUNT(*) > 1

    INSERT INTO MigrationSkippedUsers (UserID, SourceTable, Reason)
    SELECT UserID, 'SaveBpats', 'UserID already exists in AspNetUsers'
    FROM Vepo.dbo.SaveBpats
    WHERE EXISTS (SELECT 1 FROM AspNetUsers WHERE UserName = SaveBpats.UserID)

    INSERT INTO AspNetUsers
        ([UserName], [NormalizedUserName], [Email], [NormalizedEmail], [PasswordHash], [IsMigratedLegacyPasswordHashed], [PasswordExpirationDate], [EmailConfirmed], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount], [IsSuperUser], [SecurityStamp])
    SELECT SaveBpats.UserID, UPPER(SaveBpats.UserID), SaveBpats.UserID, UPPER(SaveBpats.UserID), SaveBpats.[Password], 0, GETUTCDATE(), 1, CASE WHEN ISNULL(SaveBpats.CellNumber, '') = '' THEN NULL ELSE SaveBpats.CellNumber END, CASE WHEN ISNULL(SaveBpats.CellNumber, '') = '' THEN 0 ELSE 1 END, CASE WHEN SaveBpats.DisableTwoFactorAuthentication = 1 THEN 0 ELSE 1 END, 1, 0, 0, NEWID()
    FROM Vepo.dbo.SaveBpats
    LEFT JOIN MigrationSkippedUsers skipped
        ON skipped.UserID = SaveBpats.UserID AND skipped.SourceTable = 'SaveBpats'
    WHERE skipped.UserID IS NULL

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH