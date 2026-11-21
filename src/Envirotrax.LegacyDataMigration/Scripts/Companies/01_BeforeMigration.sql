-- Links each migrated company back to its V1 ContactManagerContacts.ID. V2 assigns the row its own Id, so this
-- column is what the guard in the migration reads, and what the additional contacts (03_CompanyContacts.sql) and
-- the later contact log migration find a company by.
IF COL_LENGTH('crm.Companies', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE crm.Companies
    ADD LegacyRecordId INT NULL;
END

-- Links each migrated additional contact back to its V1 ContactManagerAdditionalContacts.ID, for the guard in the
-- migration.
IF COL_LENGTH('crm.CompanyContacts', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE crm.CompanyContacts
    ADD LegacyRecordId INT NULL;
END

-- The V1 additional contacts the migration leaves out, with the reason.
IF OBJECT_ID('MigrationSkippedCompanyContacts', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedCompanyContacts (
        LegacyRecordId INT,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END
