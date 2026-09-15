
IF COL_LENGTH('SiteLogs', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE SiteLogs
    ADD LegacyRecordId INT NULL;
END

-- Holds the V1 CsiBackflowSiteLog.AssemblyID until BackflowTests is migrated. Once it is,
-- AssemblyId can be backfilled by joining BackflowTests on LegacyRecordId + WaterSupplierId.
IF COL_LENGTH('SiteLogs', 'LegacyAssemblyRecordId') IS NULL
BEGIN
    ALTER TABLE SiteLogs
    ADD LegacyAssemblyRecordId INT NULL;
END

-- Where the attachment sits underneath the legacy file server root, for example
-- site_log/17390000/17394521.pdf. SiteLogService downloads each one, uploads it to Azure Storage and
-- only then fills in FileAttachmentPath. The file server address itself is deliberately not stored
-- here: it is configuration and lives in Program.cs, because that server is being decommissioned too.
IF COL_LENGTH('SiteLogs', 'LegacyFilePath') IS NULL
BEGIN
    ALTER TABLE SiteLogs
    ADD LegacyFilePath NVARCHAR(500) NULL;
END

IF OBJECT_ID('MigrationSkippedSiteLogs', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedSiteLogs (
        SiteLogId INT,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END
