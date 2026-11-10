-- The V1 record id is also written into BackflowTests.Id with IDENTITY_INSERT, the way sites and
-- water suppliers are migrated (EN-241). This column is still what every guard and every join in the
-- migration reads, because Id alone cannot say whether a row came from V1 or was created in V2.
IF COL_LENGTH('BackflowTests', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE BackflowTests
    ADD LegacyRecordId INT NULL;
END

-- Where each of the five test images sits on the legacy file server, until it is copied into Azure
-- Storage. 03_BackflowTestImages.sql fills them in. The file server address itself is deliberately not
-- stored here: it is configuration and lives in Program.cs, because that server is being decommissioned.
IF COL_LENGTH('BackflowTests', 'LegacyAssemblyImagePath') IS NULL
BEGIN
    ALTER TABLE BackflowTests
    ADD LegacyAssemblyImagePath NVARCHAR(500) NULL;
END

IF COL_LENGTH('BackflowTests', 'LegacySerialNumberImagePath') IS NULL
BEGIN
    ALTER TABLE BackflowTests
    ADD LegacySerialNumberImagePath NVARCHAR(500) NULL;
END

IF COL_LENGTH('BackflowTests', 'LegacyBypassAssemblyImagePath') IS NULL
BEGIN
    ALTER TABLE BackflowTests
    ADD LegacyBypassAssemblyImagePath NVARCHAR(500) NULL;
END

IF COL_LENGTH('BackflowTests', 'LegacyBypassSerialNumberImagePath') IS NULL
BEGIN
    ALTER TABLE BackflowTests
    ADD LegacyBypassSerialNumberImagePath NVARCHAR(500) NULL;
END

IF COL_LENGTH('BackflowTests', 'LegacyAirGapImagePath') IS NULL
BEGIN
    ALTER TABLE BackflowTests
    ADD LegacyAirGapImagePath NVARCHAR(500) NULL;
END

-- Set once 03_BackflowTestImages.sql has queued a test's images, even when it found none to queue, so an
-- image that turned out not to exist is never queued again by a later run.
IF COL_LENGTH('BackflowTests', 'LegacyImagesQueued') IS NULL
BEGIN
    ALTER TABLE BackflowTests
    ADD LegacyImagesQueued BIT NULL;
END

-- Set when a water supplier saved the test in V1. V1 keeps no record of an image uploaded from its
-- supplier pages, so some of the paths queued for such a test are guesses, and finding no file is expected.
IF COL_LENGTH('BackflowTests', 'LegacyImagesUnrecorded') IS NULL
BEGIN
    ALTER TABLE BackflowTests
    ADD LegacyImagesUnrecorded BIT NULL;
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
