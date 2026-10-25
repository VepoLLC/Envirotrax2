BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedApiAccounts (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyAccounts.ID, legacyAccounts.UserID, 'ApiAccounts', 'No matching migrated water supplier'
    FROM Vepo.dbo.ApiAccounts AS legacyAccounts
    LEFT JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyAccounts.WaterSupplierID
    LEFT JOIN MigrationSkippedApiAccounts AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyAccounts.ID
        AND alreadySkipped.SourceTable = 'ApiAccounts'
    WHERE legacyAccounts.WaterSupplierID <> 0
        AND WaterSuppliers.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    INSERT INTO ApiAccounts
        (LegacyRecordId, UserId, ApiKeyHash, WaterSupplierId, IsLive,
         PermissionWaterSuppliers, PermissionSites, PermissionBackflowTests,
         PermissionCsiInspections, PermissionFogInspections, PermissionFogTripTickets,
         CreatedById, CreatedTime)
    SELECT
        legacyAccounts.ID,
        legacyAccounts.UserID,
        CASE
            WHEN legacyAccounts.ApiKey = '' THEN ''
            ELSE CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', legacyAccounts.ApiKey), 2)
        END,
        CASE
            WHEN legacyAccounts.WaterSupplierID = 0 THEN 0
            ELSE WaterSuppliers.Id
        END,
        legacyAccounts.IsLive,
        legacyAccounts.PermissionWaterSuppliers,
        legacyAccounts.PermissionSites,
        legacyAccounts.PermissionBackflowTests,
        legacyAccounts.PermissionCsiInspections,
        legacyAccounts.PermissionFogInspections,
        legacyAccounts.PermissionFogTripTickets,
        NULL,
        legacyAccounts.CreationDate
    FROM Vepo.dbo.ApiAccounts AS legacyAccounts
    LEFT JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyAccounts.WaterSupplierID
    WHERE (legacyAccounts.WaterSupplierID = 0 OR WaterSuppliers.Id IS NOT NULL)
        AND NOT EXISTS
        (
            SELECT 1
            FROM ApiAccounts AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = legacyAccounts.ID
        )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
