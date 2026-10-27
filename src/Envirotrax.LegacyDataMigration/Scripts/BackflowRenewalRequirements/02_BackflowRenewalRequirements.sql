BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedBackflowRenewalRequirements (LegacyRecordId, SourceTable, Reason)
    SELECT legacyRequirements.ID, 'WaterSupplierBackflowRenewalRequirements', 'No matching migrated water supplier'
    FROM Vepo.dbo.WaterSupplierBackflowRenewalRequirements AS legacyRequirements
    LEFT JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyRequirements.WaterSupplierID
    LEFT JOIN MigrationSkippedBackflowRenewalRequirements AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyRequirements.ID
        AND alreadySkipped.SourceTable = 'WaterSupplierBackflowRenewalRequirements'
    WHERE WaterSuppliers.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    INSERT INTO BackflowRenewalRequirements
        (LegacyRecordId, WaterSupplierId, PropertyType, DeviceType, HazardType,
         HasSiteOssf, AuxWaterSupply, RenewalYears, CreatedById, CreatedTime)
    SELECT
        legacyRequirements.ID,
        waterSuppliers.Id,
        legacyRequirements.PropertyType,
        legacyRequirements.DeviceType,
        legacyRequirements.HazardType,
        legacyRequirements.SiteOssf,
        legacyRequirements.AuxWaterSupply,
        legacyRequirements.RenewalYears,
        NULL,
        legacyRequirements.CreationDate
    FROM Vepo.dbo.WaterSupplierBackflowRenewalRequirements AS legacyRequirements
    INNER JOIN WaterSuppliers AS waterSuppliers
        ON waterSuppliers.LegacyRecordId = legacyRequirements.WaterSupplierID
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM BackflowRenewalRequirements AS alreadyInserted
        WHERE alreadyInserted.LegacyRecordId = legacyRequirements.ID
    )

    DELETE MigrationSkippedBackflowRenewalRequirements
    FROM MigrationSkippedBackflowRenewalRequirements
    INNER JOIN BackflowRenewalRequirements
        ON BackflowRenewalRequirements.LegacyRecordId = MigrationSkippedBackflowRenewalRequirements.LegacyRecordId
    WHERE MigrationSkippedBackflowRenewalRequirements.SourceTable = 'WaterSupplierBackflowRenewalRequirements'

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
