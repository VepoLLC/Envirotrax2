IF COL_LENGTH('RenewalEmailVerifications', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE RenewalEmailVerifications
    ADD LegacyRecordId INT NULL;
END

IF OBJECT_ID('MigrationSkippedRenewalEmailVerifications', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedRenewalEmailVerifications (
        VerificationId INT,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END
