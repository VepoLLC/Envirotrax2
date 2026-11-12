IF OBJECT_ID('MigrationSkippedSiteLetterLogs', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedSiteLetterLogs (
        LegacyWaterSupplierId INT NULL,
        LegacySiteId INT NULL,
        ProcessFolder NVARCHAR(500) NULL,
        WaterSupplierFolder NVARCHAR(500) NULL,
        FileName NVARCHAR(500) NULL,
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END

IF OBJECT_ID('MigrationPendingSiteLetterLogs', 'U') IS NOT NULL
BEGIN
    DROP TABLE MigrationPendingSiteLetterLogs
END

CREATE TABLE MigrationPendingSiteLetterLogs (
    Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
    WaterSupplierId INT NOT NULL,
    SiteId INT NOT NULL,
    ProcessDate DATETIME2 NOT NULL,
    FileName NVARCHAR(255) NOT NULL,
    Page INT NOT NULL,
    LegacyFilePath NVARCHAR(500) NOT NULL,
    FilePath NVARCHAR(500) NOT NULL,
    INDEX IX_MigrationPendingSiteLetterLogs_LegacyFilePath (LegacyFilePath)
)
