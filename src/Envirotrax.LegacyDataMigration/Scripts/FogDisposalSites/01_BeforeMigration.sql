-- Links each migrated disposal site back to its V1 FogDisposalSites.ID. V2 assigns the row its own Id, so
-- this column is what the guard in the migration, and any later migration that points at a site, reads.
IF COL_LENGTH('FogDisposalSites', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE FogDisposalSites
    ADD LegacyRecordId INT NULL;
END
