IF COL_LENGTH('RecordLogs', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE RecordLogs
    ADD LegacyRecordId INT NULL;
END

IF COL_LENGTH('RecordLogs', 'LegacyLoggedRecordId') IS NULL
BEGIN
    ALTER TABLE RecordLogs
    ADD LegacyLoggedRecordId INT NULL;
END

IF OBJECT_ID('MigrationTranslatedRecordLogs', 'U') IS NOT NULL
BEGIN
    DROP TABLE MigrationTranslatedRecordLogs
END

CREATE TABLE MigrationTranslatedRecordLogs (
    LegacyLogId INT NOT NULL PRIMARY KEY,
    TableName NVARCHAR(100) NOT NULL,
    RecordId INT NOT NULL,
    WaterSupplierId INT NULL,
    ProfessionalId INT NULL
)
