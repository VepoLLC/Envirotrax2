-- The V1 record id is also written into FogInspections.Id with IDENTITY_INSERT, the way sites, water
-- suppliers and backflow tests are migrated (EN-241). This column is still what every guard and every
-- join in the migration reads, because Id alone cannot say whether a row came from V1 or was created in V2.
IF COL_LENGTH('FogInspections', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE FogInspections
    ADD LegacyRecordId INT NULL;
END

-- Where the exterior and interior photos sit on the legacy file server, until they are copied into
-- Azure Storage. The file server address itself is deliberately not stored here: it is configuration
-- and lives in Program.cs, because that server is being decommissioned too.
IF COL_LENGTH('FogInspections', 'LegacyExteriorImagePath') IS NULL
BEGIN
    ALTER TABLE FogInspections
    ADD LegacyExteriorImagePath NVARCHAR(500) NULL;
END

IF COL_LENGTH('FogInspections', 'LegacyInteriorImagePath') IS NULL
BEGIN
    ALTER TABLE FogInspections
    ADD LegacyInteriorImagePath NVARCHAR(500) NULL;
END

-- V1 keeps the signature as PNG bytes in Vepo.dbo.FogInspections.Signature rather than as a file, so
-- there is no path to record, only that there is a signature to copy into Azure Storage.
IF COL_LENGTH('FogInspections', 'HasLegacySignature') IS NULL
BEGIN
    ALTER TABLE FogInspections
    ADD HasLegacySignature BIT NULL;
END

-- LegacyUserId is kept alongside the record id because a V1 inspection names its inspector by login
-- (FogInspections.InspectorID), not by a numeric id, so the login is what makes a skipped row findable.
IF OBJECT_ID('MigrationSkippedFogInspections', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedFogInspections (
        LegacyRecordId INT NULL,
        LegacyUserId NVARCHAR(100) NULL,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END
