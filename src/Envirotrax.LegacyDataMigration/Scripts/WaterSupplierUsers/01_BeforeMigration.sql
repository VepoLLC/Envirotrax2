
IF COL_LENGTH('WaterSupplierUsers', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE WaterSupplierUsers
    ADD LegacyRecordId INT NULL;
END