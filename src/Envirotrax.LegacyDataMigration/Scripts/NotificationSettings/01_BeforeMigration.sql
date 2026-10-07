IF COL_LENGTH('NotificationSettings', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE NotificationSettings
    ADD LegacyRecordId INT NULL;
END

-- LegacyUserId is kept alongside the record id because a V1 notification setting names its recipient
-- by login (WaterSupplierNotificationSettings.UserID), not by a numeric id, so the login is what
-- makes a skipped row findable.
IF OBJECT_ID('MigrationSkippedNotificationSettings', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedNotificationSettings (
        LegacyRecordId INT NULL,
        LegacyUserId NVARCHAR(100) NULL,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END
