-- Links each migrated company back to its V1 ContactManagerContacts.ID. V2 assigns the row its own Id, so this
-- column is what the guard in the migration, and the later additional contact and contact log migrations, read.
IF COL_LENGTH('crm.Companies', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE crm.Companies
    ADD LegacyRecordId INT NULL;
END
