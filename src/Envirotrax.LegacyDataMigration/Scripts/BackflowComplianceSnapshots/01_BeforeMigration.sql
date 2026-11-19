IF OBJECT_ID('MigrationSkippedBackflowComplianceSnapshots', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedBackflowComplianceSnapshots (
        LegacyRecordId INT NULL,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END
