-- The V1 record id is also written into FogVehicles.Id with IDENTITY_INSERT, the way sites, water
-- suppliers and backflow tests are migrated (EN-241). This column is still what the guards in the
-- migration read, because Id alone cannot say whether a row came from V1 or was created in V2.
IF COL_LENGTH('FogVehicles', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE FogVehicles
    ADD LegacyRecordId INT NULL;
END

-- A V2 permit is keyed by its water supplier and vehicle and has no id of its own, so this column is
-- the only way back to the V1 FogVehiclePermits row it came from.
IF COL_LENGTH('FogVehiclePermits', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE FogVehiclePermits
    ADD LegacyRecordId INT NULL;
END

-- One table for both entities, told apart by SourceTable. LegacyUserId is the vehicle's TransporterID,
-- because a V1 vehicle names its owner by login, not by a numeric id; permits name no user.
IF OBJECT_ID('MigrationSkippedFogVehicles', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedFogVehicles (
        LegacyRecordId INT NULL,
        LegacyUserId NVARCHAR(100) NULL,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END
