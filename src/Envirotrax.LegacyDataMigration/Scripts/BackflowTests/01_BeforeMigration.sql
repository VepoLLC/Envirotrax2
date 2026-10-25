-- The V1 record id is also written into BackflowTests.Id with IDENTITY_INSERT, the way sites and
-- water suppliers are migrated (EN-241). This column is still what every guard and every join in the
-- migration reads, because Id alone cannot say whether a row came from V1 or was created in V2.
IF COL_LENGTH('BackflowTests', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE BackflowTests
    ADD LegacyRecordId INT NULL;
END

-- LegacyUserId is kept alongside the record id because a V1 test names its tester by login
-- (SaveBackflowDeviceTests.BpatID or InspectorID), not by a numeric id, so the login is what makes a
-- skipped row findable.
IF OBJECT_ID('MigrationSkippedBackflowTests', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedBackflowTests (
        LegacyRecordId INT NULL,
        LegacyUserId NVARCHAR(100) NULL,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END
