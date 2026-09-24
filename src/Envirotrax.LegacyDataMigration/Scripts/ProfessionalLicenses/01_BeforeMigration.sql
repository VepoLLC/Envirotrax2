IF COL_LENGTH('ProfessionalUserLicenses', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE ProfessionalUserLicenses
    ADD LegacyRecordId INT NULL;
END
