IF COL_LENGTH('ProfessionalUserLicenses', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE ProfessionalUserLicenses
    ADD LegacyRecordId INT NULL;
END

IF COL_LENGTH('ProfessionalLicenses', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE ProfessionalLicenses
    ADD LegacyRecordId INT NULL;
END
