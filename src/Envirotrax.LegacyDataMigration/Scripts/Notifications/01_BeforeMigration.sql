IF COL_LENGTH('Notifications', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE Notifications
    ADD LegacyRecordId INT NULL;
END

-- LegacyUserId is kept alongside the record id because a V1 notification names its recipient by login
-- (WaterSupplierNotifications.UserID), not by a numeric id, so the login is what makes a skipped row
-- findable.
IF OBJECT_ID('MigrationSkippedNotifications', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedNotifications (
        LegacyRecordId INT NULL,
        LegacyUserId NVARCHAR(100) NULL,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END
