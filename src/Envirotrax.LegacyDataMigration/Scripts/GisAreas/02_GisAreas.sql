BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedGisAreas (LegacyRecordId, SourceTable, Reason)
    SELECT WaterSupplierGisAreas.ID, 'WaterSupplierGisAreas', 'No matching water supplier'
    FROM Vepo.dbo.WaterSupplierGisAreas
    LEFT JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = WaterSupplierGisAreas.WaterSupplierID
    LEFT JOIN MigrationSkippedGisAreas skipped
        ON skipped.LegacyRecordId = WaterSupplierGisAreas.ID AND skipped.SourceTable = 'WaterSupplierGisAreas'
    WHERE WaterSuppliers.Id IS NULL
        AND skipped.LegacyRecordId IS NULL

    INSERT INTO GisAreas
        (LegacyRecordId, WaterSupplierId, [Name], Color,
         MinLongitude, MinLatitude, MaxLongitude, MaxLatitude,
         CreatedById, CreatedTime)
    SELECT
        WaterSupplierGisAreas.ID, WaterSuppliers.Id, WaterSupplierGisAreas.[Name], ISNULL(NULLIF(WaterSupplierGisAreas.Color, ''), '#000000'),
        bounds.MinLongitude, bounds.MinLatitude, bounds.MaxLongitude, bounds.MaxLatitude,
        NULL, WaterSupplierGisAreas.CreationDate
    FROM Vepo.dbo.WaterSupplierGisAreas
    INNER JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = WaterSupplierGisAreas.WaterSupplierID
    LEFT JOIN
    (
        SELECT
            AreaID,
            MIN(Longitude) AS MinLongitude,
            MIN(Latitude) AS MinLatitude,
            MAX(Longitude) AS MaxLongitude,
            MAX(Latitude) AS MaxLatitude
        FROM Vepo.dbo.WaterSupplierGisAreaCoordinates AS coordinates
        WHERE coordinates.Latitude IS NOT NULL
            AND coordinates.Longitude IS NOT NULL
            AND NOT (coordinates.Latitude = 0 AND coordinates.Longitude = 0)
        GROUP BY AreaID
    ) AS bounds
        ON bounds.AreaID = WaterSupplierGisAreas.ID
    WHERE NOT EXISTS (
        SELECT 1
        FROM GisAreas AS alreadyInserted
        WHERE alreadyInserted.LegacyRecordId = WaterSupplierGisAreas.ID
    )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
