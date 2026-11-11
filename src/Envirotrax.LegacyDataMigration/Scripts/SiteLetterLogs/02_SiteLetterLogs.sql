BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedSiteLetterLogs (LegacyWaterSupplierId, LegacySiteId, ProcessFolder, WaterSupplierFolder, FileName, Reason)
    SELECT DISTINCT
        legacyLetterLogs.WaterSupplierID,
        legacyLetterLogs.SiteID,
        legacyLetterLogs.ProcessFolder,
        legacyLetterLogs.WaterSupplierFolder,
        legacyLetterLogs.FileName,
        CASE
            WHEN waterSuppliers.Id IS NULL THEN 'No matching migrated water supplier'
            ELSE 'No matching migrated site'
        END
    FROM Vepo.dbo.SiteLetterLog AS legacyLetterLogs
    LEFT JOIN WaterSuppliers AS waterSuppliers
        ON waterSuppliers.LegacyRecordId = legacyLetterLogs.WaterSupplierID
    LEFT JOIN Sites AS sites
        ON sites.LegacyRecordId = legacyLetterLogs.SiteID
    WHERE (waterSuppliers.Id IS NULL OR sites.Id IS NULL)
        AND NOT EXISTS (
            SELECT 1
            FROM MigrationSkippedSiteLetterLogs AS alreadySkipped
            WHERE alreadySkipped.LegacySiteId = legacyLetterLogs.SiteID
                AND alreadySkipped.ProcessFolder = legacyLetterLogs.ProcessFolder
                AND alreadySkipped.WaterSupplierFolder = legacyLetterLogs.WaterSupplierFolder
                AND alreadySkipped.FileName = legacyLetterLogs.FileName
        )

    INSERT INTO MigrationPendingSiteLetterLogs (WaterSupplierId, SiteId, ProcessDate, FileName, Page, LegacyFilePath, FilePath)
    SELECT
        waterSuppliers.Id,
        sites.Id,
        processDates.ProcessDate,
        legacyLetterLogs.FileName,
        MIN(legacyLetterLogs.Page),
        CONCAT(
            legacyLetterLogs.ProcessFolder, '\',
            legacyLetterLogs.WaterSupplierFolder, '\',
            CASE WHEN MIN(legacyLetterLogs.Page) = 0 THEN 'Emails\' END,
            legacyLetterLogs.FileName),
        CONCAT(
            'letters/',
            FORMAT(processDates.ProcessDate, 'yyyy-MM-dd HH-mm-ss', 'en-US'), '/',
            legacyLetterLogs.WaterSupplierFolder, '/',
            CASE WHEN MIN(legacyLetterLogs.Page) = 0 THEN 'Emails/' END,
            legacyLetterLogs.FileName)
    FROM Vepo.dbo.SiteLetterLog AS legacyLetterLogs
    INNER JOIN WaterSuppliers AS waterSuppliers
        ON waterSuppliers.LegacyRecordId = legacyLetterLogs.WaterSupplierID
    INNER JOIN Sites AS sites
        ON sites.LegacyRecordId = legacyLetterLogs.SiteID
    CROSS APPLY (
        SELECT CHARINDEX(' ', legacyLetterLogs.ProcessFolder) AS SpaceIndex
    ) AS processFolders
    CROSS APPLY (
        SELECT CAST(CONCAT(
            LEFT(legacyLetterLogs.ProcessFolder, processFolders.SpaceIndex - 1),
            ' ',
            REPLACE(SUBSTRING(legacyLetterLogs.ProcessFolder, processFolders.SpaceIndex + 1, LEN(legacyLetterLogs.ProcessFolder)), '-', ':')) AS DATETIME2) AS ProcessDate
    ) AS processDates
    WHERE NOT EXISTS (
        SELECT 1
        FROM SiteLetterLogs AS alreadyInserted
        WHERE alreadyInserted.WaterSupplierId = waterSuppliers.Id
            AND alreadyInserted.SiteId = sites.Id
            AND alreadyInserted.ProcessDate = processDates.ProcessDate
            AND alreadyInserted.FileName = legacyLetterLogs.FileName
    )
    GROUP BY
        waterSuppliers.Id,
        sites.Id,
        processDates.ProcessDate,
        legacyLetterLogs.ProcessFolder,
        legacyLetterLogs.WaterSupplierFolder,
        legacyLetterLogs.FileName

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
