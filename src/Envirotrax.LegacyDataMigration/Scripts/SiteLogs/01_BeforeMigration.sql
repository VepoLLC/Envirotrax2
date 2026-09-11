
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

IF OBJECT_ID('MigrationSkippedSiteLogs', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedSiteLogs (
        SiteLogId INT,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END
