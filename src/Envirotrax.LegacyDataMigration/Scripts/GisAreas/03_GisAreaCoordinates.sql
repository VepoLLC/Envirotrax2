BEGIN TRAN

BEGIN TRY

    -- Separator rows are structure, not data, so they are not logged as skipped — only points that
    -- really could not be migrated go into the log.
    INSERT INTO MigrationSkippedGisAreas (LegacyRecordId, SourceTable, Reason)
    SELECT coordinates.ID, 'WaterSupplierGisAreaCoordinates', 'Missing coordinate value'
    FROM Vepo.dbo.WaterSupplierGisAreaCoordinates AS coordinates
    LEFT JOIN MigrationSkippedGisAreas AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = coordinates.ID AND alreadySkipped.SourceTable = 'WaterSupplierGisAreaCoordinates'
    WHERE (coordinates.Latitude IS NULL OR coordinates.Longitude IS NULL)
        AND alreadySkipped.LegacyRecordId IS NULL

    INSERT INTO MigrationSkippedGisAreas (LegacyRecordId, SourceTable, Reason)
    SELECT coordinates.ID, 'WaterSupplierGisAreaCoordinates', 'GIS area was not migrated'
    FROM Vepo.dbo.WaterSupplierGisAreaCoordinates AS coordinates
    LEFT JOIN GisAreas
        ON GisAreas.LegacyRecordId = coordinates.AreaID
    LEFT JOIN MigrationSkippedGisAreas AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = coordinates.ID AND alreadySkipped.SourceTable = 'WaterSupplierGisAreaCoordinates'
    WHERE GisAreas.Id IS NULL
        AND coordinates.Latitude IS NOT NULL
        AND coordinates.Longitude IS NOT NULL
        AND NOT (coordinates.Latitude = 0 AND coordinates.Longitude = 0)
        AND alreadySkipped.LegacyRecordId IS NULL

    -- V1 keeps every part of an area in one table, one after another, separated by a row with
    -- coordinates 0, 0: the points before the first separator are the outer edge, each block after it
    -- is a hole (WaterSupplierGisArea.vb, LoadPoints). V2 has no separator rows — instead each point
    -- carries PolygonIndex, so the ring number is the count of separators standing before that point.
    ;WITH legacyCoordinates AS
    (
        SELECT
            coordinates.ID,
            coordinates.AreaID,
            coordinates.Latitude,
            coordinates.Longitude,
            CASE
                WHEN coordinates.Latitude = 0 AND coordinates.Longitude = 0 THEN 1
                ELSE 0
            END AS IsPolygonSeparator
        FROM Vepo.dbo.WaterSupplierGisAreaCoordinates AS coordinates
        WHERE coordinates.Latitude IS NOT NULL
            AND coordinates.Longitude IS NOT NULL
    ),
    indexedCoordinates AS
    (
        SELECT
            legacyCoordinates.ID,
            legacyCoordinates.AreaID,
            legacyCoordinates.Latitude,
            legacyCoordinates.Longitude,
            legacyCoordinates.IsPolygonSeparator,
            ISNULL(
                SUM(legacyCoordinates.IsPolygonSeparator) OVER (
                    PARTITION BY legacyCoordinates.AreaID
                    ORDER BY legacyCoordinates.ID
                    ROWS BETWEEN UNBOUNDED PRECEDING AND 1 PRECEDING
                ), 0) AS PolygonIndex
        FROM legacyCoordinates
    )
    INSERT INTO GisAreaCoordinates
        (LegacyRecordId, WaterSupplierId, AreaId, PolygonIndex, Latitude, Longitude)
    SELECT
        indexedCoordinates.ID, GisAreas.WaterSupplierId, GisAreas.Id, indexedCoordinates.PolygonIndex,
        indexedCoordinates.Latitude, indexedCoordinates.Longitude
    FROM indexedCoordinates
    INNER JOIN GisAreas
        ON GisAreas.LegacyRecordId = indexedCoordinates.AreaID
    WHERE indexedCoordinates.IsPolygonSeparator = 0
        AND NOT EXISTS (
            SELECT 1
            FROM GisAreaCoordinates AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = indexedCoordinates.ID
        )
    -- Vertex order is the shape itself, and in V2 it is restored by Id, so the rows have to be inserted
    -- in the legacy order. MAXDOP 1 keeps the plan serial, otherwise identity values may not follow it.
    ORDER BY indexedCoordinates.AreaID, indexedCoordinates.ID
    OPTION (MAXDOP 1)

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
