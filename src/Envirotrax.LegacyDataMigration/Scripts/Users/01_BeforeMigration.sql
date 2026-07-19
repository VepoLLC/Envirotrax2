
USE Envirotrax2Dev
GO

IF COL_LENGTH('AspNetUsers', 'LegacyPasswordHash') IS NULL
BEGIN
    ALTER TABLE AspNetUsers
    ADD LegacyPasswordHash NVARCHAR(MAX) NULL;
END

IF OBJECT_ID('MigrationSkippedUsers', 'U') IS NULL
BEGIN
	CREATE TABLE MigrationSkippedUsers (
		UserID NVARCHAR(256),
		SourceTable NVARCHAR(256),
		Reason NVARCHAR(500),
		MigratedAt DATETIME DEFAULT GETUTCDATE()
	)
END

