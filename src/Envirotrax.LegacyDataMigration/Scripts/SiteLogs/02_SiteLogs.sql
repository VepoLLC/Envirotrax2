BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedSiteLogs (SiteLogId, SourceTable, Reason)
    SELECT legacySiteLogs.ID, 'CsiBackflowSiteLog', 'No matching migrated site'
    FROM Vepo.dbo.CsiBackflowSiteLog AS legacySiteLogs
    LEFT JOIN Sites
        ON Sites.LegacyRecordId = legacySiteLogs.SiteID
    LEFT JOIN MigrationSkippedSiteLogs AS alreadySkipped
        ON alreadySkipped.SiteLogId = legacySiteLogs.ID
        AND alreadySkipped.SourceTable = 'CsiBackflowSiteLog'
    WHERE Sites.Id IS NULL
        AND alreadySkipped.SiteLogId IS NULL

    INSERT INTO SiteLogs
        (LegacyRecordId, LegacyAssemblyRecordId, WaterSupplierId, SiteId, LogType,
         NoteText, ReviewDate, AssemblyId,
         FileAttachmentName, FileAttachmentPath, SkipFile,
         CreatedById, CreatedTime)
    SELECT
        legacySiteLogs.ID,
        NULLIF(legacySiteLogs.AssemblyID, 0),
        sites.WaterSupplierId,
        sites.Id,
        -- V1 has a fourth log type (FileAttachment = 3) that V2 does not. It falls back to Note,
        -- which is how V2 renders an attachment-only log anyway.
        CASE legacySiteLogs.Type
            WHEN 1 THEN 1
            WHEN 2 THEN 2
            ELSE 0
        END,
        NULLIF(CAST(legacySiteLogs.Note AS VARCHAR(MAX)), ''),
        -- V1 stores ReviewDate NOT NULL and defaults it to the creation date for notes, where it is
        -- never displayed. Carrying that over would make every migrated note look overdue in V2.
        CASE WHEN legacySiteLogs.Type IN (1, 2) THEN legacySiteLogs.ReviewDate END,
        NULL,
        legacySiteLogs.FileAttachmentName,
        -- V1 keeps no path. It rebuilds the URL from the record id at render time:
        -- SharedDriveUtility.FileServerAddress + "/site_log/" + GetFolderNumber(ID) + "/" + ID + "." + FileAttachmentType
        CASE
            WHEN legacySiteLogs.FileAttachmentName IS NULL THEN NULL
            ELSE 'https://iofiles.envirotrax.com/site_log/'
                 + CAST((legacySiteLogs.ID / 10000) * 10000 AS VARCHAR(20))
                 + '/' + CAST(legacySiteLogs.ID AS VARCHAR(20))
                 + '.' + ISNULL(legacySiteLogs.FileAttachmentType, '')
        END,
        -- Migrated attachments live on the old shared drive, not Azure, so SAS generation must be
        -- skipped for every one of them regardless of what the V1 column says.
        CASE WHEN legacySiteLogs.FileAttachmentName IS NULL THEN 0 ELSE 1 END,
        createdByUsers.Id,
        legacySiteLogs.CreationDate
    FROM Vepo.dbo.CsiBackflowSiteLog AS legacySiteLogs
    INNER JOIN Sites AS sites
        ON sites.LegacyRecordId = legacySiteLogs.SiteID
    -- NULLIF keeps the 21k blank authors from matching the one legacy account whose UserID is also
    -- blank, and MIN guarantees one row per legacy log even if two users share an email address.
    OUTER APPLY
    (
        SELECT MIN(AspNetUsers.Id) AS Id
        FROM AspNetUsers
        WHERE AspNetUsers.Email = NULLIF(legacySiteLogs.UserID, '')
    ) AS createdByUsers
    WHERE NOT EXISTS (
        SELECT 1
        FROM SiteLogs AS alreadyInserted
        WHERE alreadyInserted.LegacyRecordId = legacySiteLogs.ID
    )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
