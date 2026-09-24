BEGIN TRAN

BEGIN TRY

    -- One-time cleanup for databases loaded by the first version of this script, which wrote the
    -- legacy file server URL straight into FileAttachmentPath. That column may only ever hold an
    -- Azure blob path, so the location moves to LegacyFilePath minus the file server address and the
    -- row rejoins the set SiteLogService picks up. Without this the main INSERT below would skip those
    -- rows forever, because their LegacyRecordId already exists. A URL whose last segment has no
    -- usable extension had no file behind it on the old server, so it gets NULL and is recorded as a
    -- skipped attachment further down - exactly what a freshly migrated row with the same legacy data
    -- gets. Self-disabling: after this runs FileAttachmentPath is NULL, so the LIKE cannot match again.
    UPDATE SiteLogs
    SET LegacyFilePath = legacyPaths.RelativePath,
        FileAttachmentPath = NULL
    FROM SiteLogs
    CROSS APPLY
    (
        SELECT CASE
            WHEN CHARINDEX('.', REVERSE(SiteLogs.FileAttachmentPath)) > 1
                AND RIGHT(SiteLogs.FileAttachmentPath, CHARINDEX('.', REVERSE(SiteLogs.FileAttachmentPath)) - 1)
                    COLLATE Latin1_General_BIN2 NOT LIKE '%[^0-9A-Za-z]%'
            THEN LOWER(SUBSTRING(SiteLogs.FileAttachmentPath, CHARINDEX('/site_log/', SiteLogs.FileAttachmentPath) + 1, LEN(SiteLogs.FileAttachmentPath)))
        END AS RelativePath
    ) AS legacyPaths
    WHERE SiteLogs.LegacyRecordId IS NOT NULL
        AND SiteLogs.LegacyFilePath IS NULL
        AND SiteLogs.FileAttachmentPath LIKE 'http%://%/site_log/%'

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
         FileAttachmentName, FileAttachmentPath, LegacyFilePath, SkipFile,
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
        -- Filled in by SiteLogService once the file is really in Azure Storage. Staying NULL until then
        -- is what keeps the row in the pending set, so a run that dies half way resumes where it stopped.
        NULL,
        -- V1 keeps no path. It rebuilds the location from the record id at render time:
        -- SharedDriveUtility.FileServerAddress + "/site_log/" + GetFolderNumber(ID) + "/" + ID + "." + FileAttachmentType
        -- Only the part below the file server address is stored, because that address is being
        -- decommissioned. A file type that is missing, blank, or not a plain extension names no file
        -- the old server can serve, so those rows get NULL here and are recorded as skipped attachments.
        CASE
            WHEN legacySiteLogs.FileAttachmentName IS NULL THEN NULL
            WHEN NULLIF(LTRIM(RTRIM(legacySiteLogs.FileAttachmentType)), '') IS NULL THEN NULL
            WHEN LTRIM(RTRIM(legacySiteLogs.FileAttachmentType)) COLLATE Latin1_General_BIN2 LIKE '%[^0-9A-Za-z]%' THEN NULL
            ELSE 'site_log/'
                 + CAST((legacySiteLogs.ID / 10000) * 10000 AS VARCHAR(20))
                 + '/' + CAST(legacySiteLogs.ID AS VARCHAR(20))
                 + '.' + LOWER(LTRIM(RTRIM(legacySiteLogs.FileAttachmentType)))
        END,
        -- Every migrated attachment starts skipped, because its file is still on the legacy file
        -- server. SiteLogService clears the flag one row at a time, in the same save that writes
        -- FileAttachmentPath, so a file name only ever becomes a link once its file is in Azure.
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

    -- The log itself migrated fine. Only its attachment is unreachable, because there is no usable
    -- file type to rebuild the legacy file name from, so nothing will ever be downloaded for it and
    -- the file is lost when the old server goes away. Recorded here so that loss is counted and
    -- queryable instead of silently disappearing. Reading it back out of SiteLogs rather than out of
    -- the legacy table covers rows repaired by the UPDATE above as well as freshly inserted ones, and
    -- LegacyRecordId keeps V2 native logs - which may legitimately carry a file name with no path -
    -- out of it entirely.
    INSERT INTO MigrationSkippedSiteLogs (SiteLogId, SourceTable, Reason)
    SELECT SiteLogs.LegacyRecordId, 'CsiBackflowSiteLog.FileAttachment', 'Attachment has no usable file type'
    FROM SiteLogs
    LEFT JOIN MigrationSkippedSiteLogs AS alreadySkipped
        ON alreadySkipped.SiteLogId = SiteLogs.LegacyRecordId
        AND alreadySkipped.SourceTable = 'CsiBackflowSiteLog.FileAttachment'
    WHERE SiteLogs.LegacyRecordId IS NOT NULL
        AND SiteLogs.FileAttachmentName IS NOT NULL
        AND SiteLogs.LegacyFilePath IS NULL
        AND SiteLogs.FileAttachmentPath IS NULL
        AND alreadySkipped.SiteLogId IS NULL

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
