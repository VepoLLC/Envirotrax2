-- Links each migrated trip ticket back to its V1 FogTripTickets.ID. V2 assigns the row its own Id, so
-- this column is what every guard and every join in the migration reads.
IF COL_LENGTH('FogTripTickets', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE FogTripTickets
    ADD LegacyRecordId INT NULL;
END

-- V1 keeps the three signatures as PNG bytes on the ticket itself rather than as files, so there is no
-- path to record, only that there is a signature to copy into Azure Storage.
IF COL_LENGTH('FogTripTickets', 'HasLegacyTransporterSignature') IS NULL
BEGIN
    ALTER TABLE FogTripTickets
    ADD HasLegacyTransporterSignature BIT NULL;
END

IF COL_LENGTH('FogTripTickets', 'HasLegacyGeneratorSignature') IS NULL
BEGIN
    ALTER TABLE FogTripTickets
    ADD HasLegacyGeneratorSignature BIT NULL;
END

IF COL_LENGTH('FogTripTickets', 'HasLegacyReceiverSignature') IS NULL
BEGIN
    ALTER TABLE FogTripTickets
    ADD HasLegacyReceiverSignature BIT NULL;
END

-- LegacyUserId is kept alongside the record id because a V1 ticket names its transporter by login
-- (FogTripTickets.TransporterID), not by a numeric id, so the login is what makes a skipped row findable.
IF OBJECT_ID('MigrationSkippedFogTripTickets', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedFogTripTickets (
        LegacyRecordId INT NULL,
        LegacyUserId NVARCHAR(100) NULL,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END
