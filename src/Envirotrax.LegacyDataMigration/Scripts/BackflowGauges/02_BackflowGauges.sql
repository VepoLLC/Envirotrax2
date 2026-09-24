BEGIN TRAN

BEGIN TRY

    -- One-time cleanup for databases loaded by the first version of this script, which only filled
    -- LegacyFilePath in when SaveBpatGauges.ImageStored was 1. Nothing in V1 ever writes that column:
    -- it is only ever read, when deciding whether to redirect to the file server or fall back to the
    -- FileData bytes (water_suppliers/gauge_view.aspx.vb). Both pages that add or re-calibrate a gauge
    -- write the report to the shared drive and set FileType, and neither touches ImageStored or
    -- FileData (save_bpats/gauges.aspx.vb), so the flag is set on a minority of old rows while the
    -- report of every other gauge is on the file server all the same. Without this the INSERT below
    -- would skip those rows forever, because their LegacyRecordId already exists.
    -- Self-disabling: it only touches rows whose LegacyFilePath is still NULL.
    UPDATE BackflowGauges
    SET LegacyFilePath = CONCAT('gauges/', CAST(FLOOR(gauges.ID / 10000) * 10000 AS VARCHAR(20)), '/', CAST(gauges.ID AS VARCHAR(20)),
                                CASE WHEN LOWER(LTRIM(RTRIM(gauges.FileType))) = '.pdf' THEN '.pdf' ELSE '.jpg' END)
    FROM BackflowGauges
    INNER JOIN Vepo.dbo.SaveBpatGauges AS gauges
        ON gauges.ID = BackflowGauges.LegacyRecordId
    WHERE BackflowGauges.LegacyRecordId IS NOT NULL
        AND BackflowGauges.LegacyFilePath IS NULL
        -- A gauge whose file has already been uploaded through V2 keeps it: FilePath is only ever
        -- filled in once the blob is really there, so a row that has one is not waiting for anything.
        AND BackflowGauges.FilePath IS NULL
        AND NULLIF(LTRIM(RTRIM(gauges.FileType)), '') IS NOT NULL

    -- The same run also clears the entries the first version wrote for those gauges, because they are
    -- now known to be wrong: the report was never missing, only unreachable behind the ImageStored gate.
    DELETE FROM MigrationSkippedBackflowGauges
    FROM MigrationSkippedBackflowGauges
    INNER JOIN Vepo.dbo.SaveBpatGauges AS gauges
        ON gauges.ID = MigrationSkippedBackflowGauges.LegacyRecordId
    WHERE MigrationSkippedBackflowGauges.SourceTable = 'SaveBpatGauges'
        AND MigrationSkippedBackflowGauges.Reason = 'Gauge has no file on the legacy file server'
        AND NULLIF(LTRIM(RTRIM(gauges.FileType)), '') IS NOT NULL

    INSERT INTO MigrationSkippedBackflowGauges (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT gauges.ID, gauges.BpatID, 'SaveBpatGauges', 'No professional account for the gauge'
    FROM Vepo.dbo.SaveBpatGauges AS gauges
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM MigrationLegacyProfessionalAccounts AS accounts
        WHERE accounts.LegacyUserId = gauges.BpatID
            AND accounts.SourceTable = 'SaveBpats'
    )
    AND NOT EXISTS
    (
        SELECT 1
        FROM MigrationSkippedBackflowGauges AS skipped
        WHERE skipped.LegacyRecordId = gauges.ID
            AND skipped.SourceTable = 'SaveBpatGauges'
    )

    -- FileType is what says a report was uploaded: V1 writes it in the same statement that creates the
    -- gauge, and writes the file to the shared drive straight after. ImageStored is deliberately not
    -- tested here - see the cleanup above. Whether the file is still on that server is settled when
    -- BackflowGaugeService tries to fetch it, which records the ones that are gone.
    INSERT INTO MigrationSkippedBackflowGauges (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT gauges.ID, gauges.BpatID, 'SaveBpatGauges', 'Gauge has no file on the legacy file server'
    FROM Vepo.dbo.SaveBpatGauges AS gauges
    WHERE NULLIF(LTRIM(RTRIM(gauges.FileType)), '') IS NULL
        AND NOT EXISTS
        (
            SELECT 1
            FROM MigrationSkippedBackflowGauges AS skipped
            WHERE skipped.LegacyRecordId = gauges.ID
                AND skipped.SourceTable = 'SaveBpatGauges'
        )

    -- A gauge names its owner by login, and that login can be a sub-account. V2 hangs gauges off the
    -- company, so the login is resolved through the staging table that 02_LegacyAccounts.sql fills in:
    -- it already holds the rule that the company is MasterBpatID when there is one and the account's
    -- own login otherwise. Ranking guards against a login appearing twice in SaveBpats.
    ;WITH AccountsWithoutDuplicates AS
    (
        SELECT accounts.*,
            ROW_NUMBER() OVER (PARTITION BY accounts.LegacyUserId ORDER BY accounts.LegacyRecordId) AS AccountRank
        FROM MigrationLegacyProfessionalAccounts AS accounts
        WHERE accounts.SourceTable = 'SaveBpats'
    )
    INSERT INTO BackflowGauges
        (LegacyRecordId, ProfessionalId, Manufacturer, Model, SerialNumber, LastCalibrationDate,
         IsPortable, IsManaged, FilePath, LegacyFilePath, CreatedById, CreatedTime)
    SELECT
        gauges.ID,
        professionals.Id,
        -- V2 requires all three, V1 does not. Empty keeps the gauge itself, which is what a test is
        -- recorded against, rather than dropping it over a blank field.
        LEFT(ISNULL(gauges.Manufacturer, ''), 100),
        LEFT(ISNULL(gauges.Model, ''), 100),
        LEFT(ISNULL(gauges.SerialNumber, ''), 50),
        gauges.LastCalibrationDate,
        -- V1 stores NonPotable, V2 stores the opposite reading of the same question: its list shows
        -- "Potable" when IsPortable is true (backflow/testers/details/gauge/list). The V2 name is a
        -- misspelling of potable and has nothing to do with the gauge being portable, so the value has
        -- to be turned round here or every gauge arrives with its water type reversed.
        CASE WHEN ISNULL(gauges.NonPotable, 0) = 1 THEN 0 ELSE 1 END,
        ISNULL(gauges.IsManaged, 0),
        -- Left NULL on purpose: it is the flag BackflowGaugeService uses to find the gauges whose file
        -- still has to be moved, and it is filled in only once the upload to Azure has returned.
        NULL,
        -- V1 stored gauge files in folders derived from the record id: gauges/<ID rounded down to
        -- 10000>/<ID>.<pdf|jpg>, and anything that was not a PDF was converted to a JPG on upload
        -- (save_bpats/gauges.aspx.vb, water_suppliers/gauge_view.aspx.vb).
        CASE
            WHEN NULLIF(LTRIM(RTRIM(gauges.FileType)), '') IS NOT NULL
            THEN CONCAT('gauges/', CAST(FLOOR(gauges.ID / 10000) * 10000 AS VARCHAR(20)), '/', CAST(gauges.ID AS VARCHAR(20)),
                        -- Trimmed and lowered before the comparison: a stray space would otherwise send
                        -- a PDF down the image branch and name the blob .jpg, which is the wrong file.
                        CASE WHEN LOWER(LTRIM(RTRIM(gauges.FileType))) = '.pdf' THEN '.pdf' ELSE '.jpg' END)
        END,
        NULL,
        -- V1 does not always fill CreationDate in when a gauge is registered. The account the gauge
        -- belongs to is the closest date that is certainly not later than the gauge itself.
        COALESCE(gauges.CreationDate, accounts.CreationDate, GETUTCDATE())
    FROM Vepo.dbo.SaveBpatGauges AS gauges
    INNER JOIN AccountsWithoutDuplicates AS accounts
        ON accounts.LegacyUserId = gauges.BpatID
        AND accounts.AccountRank = 1
    INNER JOIN Professionals AS professionals
        ON professionals.LegacyUserId = accounts.LegacyCompanyUserId
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM BackflowGauges AS alreadyInserted
        WHERE alreadyInserted.LegacyRecordId = gauges.ID
    )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
