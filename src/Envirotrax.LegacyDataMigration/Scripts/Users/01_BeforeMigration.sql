
USE Envirotrax2Dev
GO

IF COL_LENGTH('AspNetUsers', 'IsMigratedLegacyPasswordHashed') IS NULL
BEGIN
    ALTER TABLE AspNetUsers
    ADD IsMigratedLegacyPasswordHashed BIT NOT NULL DEFAULT 0;
END

IF COL_LENGTH('AspNetUsers', 'PasswordExpirationDate') IS NULL
BEGIN
    ALTER TABLE AspNetUsers
    ADD PasswordExpirationDate DATETIME2 NULL;
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

