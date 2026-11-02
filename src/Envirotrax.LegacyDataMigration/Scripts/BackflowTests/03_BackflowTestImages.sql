BEGIN TRAN

BEGIN TRY

    -- V1 stores no path for a test image. Every page that shows one (water_suppliers, save_bpats, csi and
    -- public_search backflow_test.aspx.vb) rebuilds it from the record id as
    -- backflow/<ID rounded down to 10000>/<ID>-ext.jpg, always as a JPG, and shows it when that file exists.
    -- The suffix names the image: -ext the assembly, -sn its serial number, -bext and -bsn the bypass
    -- assembly and its serial number, -ag the air gap. Which ones a page looks for depends on the device
    -- type: an air gap has only the air gap image, every other device the assembly and serial number images,
    -- and only a detector (DCD, DCD2, RPPD, RPPD2) the two bypass images as well.
    --
    -- A tester's upload is recorded in SaveBackflowDeviceImages as well as written to the file server
    -- (save_bpats/backflow_test_submit.aspx.vb, uploadImages): an image type that is not NULL means the
    -- file was written. A water supplier's upload (water_suppliers/backflow_test.aspx.vb and site.aspx.vb)
    -- writes only the file. The one trace it leaves is the save around it, which stamps LastModifiedBy, or
    -- ManualUserID on a test the supplier entered. So a test a supplier saved has every image of its device
    -- type queued, and BackflowTestService settles whether each one is really on the file server.
    UPDATE tests
    SET LegacyAssemblyImagePath =
            CASE
                WHEN legacyTests.DeviceType <> 'AG'
                    AND (legacyImages.HasAssemblyImage = 1 OR supplierSaves.SavedBySupplier = 1)
                THEN CONCAT(legacyFiles.PathPrefix, '-ext.jpg')
            END,
        LegacySerialNumberImagePath =
            CASE
                WHEN legacyTests.DeviceType <> 'AG'
                    AND (legacyImages.HasSerialNumberImage = 1 OR supplierSaves.SavedBySupplier = 1)
                THEN CONCAT(legacyFiles.PathPrefix, '-sn.jpg')
            END,
        LegacyBypassAssemblyImagePath =
            CASE
                WHEN legacyTests.DeviceType IN ('DCD', 'DCD2', 'RPPD', 'RPPD2')
                    AND (legacyImages.HasBypassAssemblyImage = 1 OR supplierSaves.SavedBySupplier = 1)
                THEN CONCAT(legacyFiles.PathPrefix, '-bext.jpg')
            END,
        LegacyBypassSerialNumberImagePath =
            CASE
                WHEN legacyTests.DeviceType IN ('DCD', 'DCD2', 'RPPD', 'RPPD2')
                    AND (legacyImages.HasBypassSerialNumberImage = 1 OR supplierSaves.SavedBySupplier = 1)
                THEN CONCAT(legacyFiles.PathPrefix, '-bsn.jpg')
            END,
        LegacyAirGapImagePath =
            CASE
                WHEN legacyTests.DeviceType = 'AG'
                    AND (legacyImages.HasAirGapImage = 1 OR supplierSaves.SavedBySupplier = 1)
                THEN CONCAT(legacyFiles.PathPrefix, '-ag.jpg')
            END,
        LegacyImagesUnrecorded = CASE WHEN supplierSaves.SavedBySupplier = 1 THEN 1 END,
        LegacyImagesQueued = 1
    FROM BackflowTests AS tests
    INNER JOIN Vepo.dbo.SaveBackflowDeviceTests AS legacyTests
        ON legacyTests.ID = tests.LegacyRecordId
    -- Grouped, because a few V1 tests have more than one row here.
    LEFT JOIN
    (
        SELECT TestID,
            MAX(CASE WHEN ExteriorImageType IS NOT NULL THEN 1 ELSE 0 END) AS HasAssemblyImage,
            MAX(CASE WHEN SerialImageType IS NOT NULL THEN 1 ELSE 0 END) AS HasSerialNumberImage,
            MAX(CASE WHEN BypassExteriorImageType IS NOT NULL THEN 1 ELSE 0 END) AS HasBypassAssemblyImage,
            MAX(CASE WHEN BypassSerialImageType IS NOT NULL THEN 1 ELSE 0 END) AS HasBypassSerialNumberImage,
            MAX(CASE WHEN AirGapImageType IS NOT NULL THEN 1 ELSE 0 END) AS HasAirGapImage
        FROM Vepo.dbo.SaveBackflowDeviceImages
        GROUP BY TestID
    ) AS legacyImages
        ON legacyImages.TestID = legacyTests.ID
    CROSS APPLY
    (
        SELECT CONCAT('backflow/', CAST((legacyTests.ID / 10000) * 10000 AS VARCHAR(20)), '/',
                      CAST(legacyTests.ID AS VARCHAR(20))) AS PathPrefix
    ) AS legacyFiles
    CROSS APPLY
    (
        SELECT CASE
                   WHEN ISNULL(legacyTests.LastModifiedBy, '') <> '' OR ISNULL(legacyTests.ManualUserID, '') <> ''
                   THEN 1
                   ELSE 0
               END AS SavedBySupplier
    ) AS supplierSaves
    WHERE tests.LegacyImagesQueued IS NULL

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
