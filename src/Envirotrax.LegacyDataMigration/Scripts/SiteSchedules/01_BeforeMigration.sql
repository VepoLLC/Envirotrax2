IF COL_LENGTH('SiteSchedules', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE SiteSchedules
    ADD LegacyRecordId INT NULL;
END

IF OBJECT_ID('MigrationSkippedSiteSchedules', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedSiteSchedules (
        LegacyRecordId INT NULL,
        LegacyUserId NVARCHAR(100) NULL,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END
