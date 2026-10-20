-- V2 gives every migrated row a new Id. LegacyRecordId keeps the V1 record id, and every guard and join
-- in this migration reads it.
IF COL_LENGTH('CsiInspections', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE CsiInspections
    ADD LegacyRecordId INT NULL;
END

IF COL_LENGTH('CsiInspectionVisuallyIdentifiedAssemblies', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE CsiInspectionVisuallyIdentifiedAssemblies
    ADD LegacyRecordId INT NULL;
END

IF COL_LENGTH('CsiInspectionImages', 'LegacyRecordId') IS NULL
BEGIN
    ALTER TABLE CsiInspectionImages
    ADD LegacyRecordId INT NULL;
END

-- V1 rows that could not be migrated, with the reason. SourceTable tells inspections, assemblies and
-- images apart.
IF OBJECT_ID('MigrationSkippedCsiInspections', 'U') IS NULL
BEGIN
    CREATE TABLE MigrationSkippedCsiInspections (
        LegacyRecordId INT NULL,
        LegacyUserId NVARCHAR(100) NULL,
        SourceTable NVARCHAR(256),
        Reason NVARCHAR(500),
        MigratedAt DATETIME DEFAULT GETUTCDATE()
    )
END

-- Scratch list of images still to be moved, rebuilt on every run. CsiInspectionImages.FilePath is required,
-- so an image row is only inserted by CsiInspectionService once its file is really in Azure Storage.
IF OBJECT_ID('MigrationPendingCsiInspectionImages', 'U') IS NOT NULL
BEGIN
    DROP TABLE MigrationPendingCsiInspectionImages
END

CREATE TABLE MigrationPendingCsiInspectionImages (
    LegacyRecordId INT NOT NULL PRIMARY KEY,
    WaterSupplierId INT NOT NULL,
    InspectionId INT NOT NULL,
    ProfessionalId INT NOT NULL,
    Description NVARCHAR(1000) NULL,
    LegacyFilePath NVARCHAR(500) NOT NULL,
    IsStoredInDatabase BIT NOT NULL
)
