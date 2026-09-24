IF COL_LENGTH('ProfessionalInsurances', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE ProfessionalInsurances
    ADD LegacyRecordId INT NULL;
END

-- Where the policy file sits on the legacy file server, until it is copied into Azure Storage.
IF COL_LENGTH('ProfessionalInsurances', 'LegacyFilePath') IS NULL
BEGIN
    ALTER TABLE ProfessionalInsurances
    ADD LegacyFilePath NVARCHAR(500) NULL;
END
