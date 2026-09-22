IF COL_LENGTH('BackflowGauges', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE BackflowGauges
    ADD LegacyRecordId INT NULL;
END

-- Where the Test for Accuracy report sits on the legacy file server, until it is copied into Azure
-- Storage. The file server address itself is deliberately not stored here: it is configuration and
-- lives in Program.cs, because that server is being decommissioned too.
IF COL_LENGTH('BackflowGauges', 'LegacyFilePath') IS NULL
BEGIN
    ALTER TABLE BackflowGauges
    ADD LegacyFilePath NVARCHAR(500) NULL;
END

-- LegacyUserId is kept alongside the record id because a V1 gauge names its owner by login
-- (SaveBpatGauges.BpatID), not by a numeric id, so the login is what makes a skipped row findable.
IF OBJECT_ID('MigrationSkippedBackflowGauges', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedBackflowGauges (
        LegacyRecordId INT NULL,
        LegacyUserId NVARCHAR(100) NULL,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END
