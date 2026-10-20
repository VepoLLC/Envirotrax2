BEGIN TRAN

BEGIN TRY

    -- Images with InspectionId = 0 were uploaded to a form that was never submitted, so they are skipped silently.
    INSERT INTO MigrationSkippedCsiInspections (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyImages.ID, NULL, 'CsiInspectionImages', 'No matching migrated inspection'
    FROM Vepo.dbo.CsiInspectionImages AS legacyImages
    LEFT JOIN CsiInspections
        ON CsiInspections.LegacyRecordId = legacyImages.InspectionId
    LEFT JOIN MigrationSkippedCsiInspections AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyImages.ID
        AND alreadySkipped.SourceTable = 'CsiInspectionImages'
    WHERE ISNULL(legacyImages.InspectionId, 0) <> 0
        AND CsiInspections.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    INSERT INTO MigrationPendingCsiInspectionImages
        (LegacyRecordId, WaterSupplierId, InspectionId, ProfessionalId, Description, LegacyFilePath, IsStoredInDatabase)
    SELECT
        legacyImages.ID,
        inspections.WaterSupplierId,
        inspections.Id,
        inspections.ProfessionalId,
        LEFT(NULLIF(CAST(legacyImages.Description AS NVARCHAR(MAX)), ''), 1000),
        -- V1 keeps no path: an image lives on the legacy file server at csi/<ID rounded down to 10000>/<ID>.jpg,
        -- unless its bytes are still in ImageData, in which case CsiInspectionService reads them from V1.
        CONCAT('csi/', CAST((legacyImages.ID / 10000) * 10000 AS VARCHAR(20)), '/', CAST(legacyImages.ID AS VARCHAR(20)), '.jpg'),
        CASE WHEN legacyImages.ImageData IS NULL THEN 0 ELSE 1 END
    FROM Vepo.dbo.CsiInspectionImages AS legacyImages
    INNER JOIN CsiInspections AS inspections
        ON inspections.LegacyRecordId = legacyImages.InspectionId
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM CsiInspectionImages AS alreadyInserted
        WHERE alreadyInserted.LegacyRecordId = legacyImages.ID
    )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
