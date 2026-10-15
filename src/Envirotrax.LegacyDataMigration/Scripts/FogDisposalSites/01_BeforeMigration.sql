-- The V1 record id is also written into FogDisposalSites.Id with IDENTITY_INSERT, the way sites, water
-- suppliers and backflow tests are migrated (EN-241). This column is still what the guard in the
-- migration reads, because Id alone cannot say whether a row came from V1 or was created in V2.
IF COL_LENGTH('FogDisposalSites', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE FogDisposalSites
    ADD LegacyRecordId INT NULL;
END
