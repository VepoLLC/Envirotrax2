IF COL_LENGTH('ApiAccounts', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE ApiAccounts
    ADD LegacyRecordId INT NULL;
END

IF OBJECT_ID('MigrationSkippedApiAccounts', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedApiAccounts (
        LegacyRecordId INT NULL,
        LegacyUserId NVARCHAR(100) NULL,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END
