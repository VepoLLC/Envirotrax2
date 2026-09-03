
IF COL_LENGTH('Sites', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE Sites
    ADD LegacyRecordId INT NULL;
END

IF OBJECT_ID('MigrationSkippedSites', 'U') IS NULL
BEGIN
	CREATE TABLE MigrationSkippedSites (
		SiteId INT,
		SourceTable NVARCHAR(256),
		Reason NVARCHAR(500),
		MigratedAt DATETIME DEFAULT GETUTCDATE()
	)
END
