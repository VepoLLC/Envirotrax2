IF COL_LENGTH('BackflowRenewalRequirements', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE BackflowRenewalRequirements
    ADD LegacyRecordId INT NULL;
END

IF OBJECT_ID('MigrationSkippedBackflowRenewalRequirements', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedBackflowRenewalRequirements (
        LegacyRecordId INT NULL,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END
