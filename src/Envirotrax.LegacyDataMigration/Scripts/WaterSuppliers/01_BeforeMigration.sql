
IF COL_LENGTH('WaterSuppliers', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE WaterSuppliers
    ADD LegacyRecordId INT NULL;
END

