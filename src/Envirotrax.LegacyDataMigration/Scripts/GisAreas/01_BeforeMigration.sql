IF COL_LENGTH('GisAreas', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE GisAreas
    ADD LegacyRecordId INT NULL;
END

IF COL_LENGTH('GisAreaCoordinates', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE GisAreaCoordinates
    ADD LegacyRecordId INT NULL;
END

IF OBJECT_ID('MigrationSkippedGisAreas', 'U') IS NULL
BEGIN
	CREATE TABLE MigrationSkippedGisAreas (
		LegacyRecordId INT,
		SourceTable NVARCHAR(256),
		Reason NVARCHAR(500),
		MigratedAt DATETIME DEFAULT GETUTCDATE()
	)
END
