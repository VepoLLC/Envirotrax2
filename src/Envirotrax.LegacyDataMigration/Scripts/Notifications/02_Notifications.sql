BEGIN TRAN

BEGIN TRY

    -- V1 reads a notification under either of two suppliers ("MasterWaterSupplierID = X OR
    -- WaterSupplierID = X", notification_settings.aspx.vb), but it is WaterSupplierID that says whose
    -- record triggered it. V2 dropped the master column outright (EN-184), so that is the one to carry.
    INSERT INTO MigrationSkippedNotifications (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyNotifications.ID, legacyNotifications.UserID, 'WaterSupplierNotifications', 'No matching migrated water supplier'
    FROM Vepo.dbo.WaterSupplierNotifications AS legacyNotifications
    LEFT JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyNotifications.WaterSupplierID
    LEFT JOIN MigrationSkippedNotifications AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyNotifications.ID
        AND alreadySkipped.SourceTable = 'WaterSupplierNotifications'
    WHERE WaterSuppliers.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    -- A notification is addressed to exactly one person and V2 makes UserId NOT NULL, so one whose
    -- recipient never became a supplier user of that supplier cannot be represented at all. V1 names
    -- that person by login, which is the email their AspNetUsers account was migrated under.
    INSERT INTO MigrationSkippedNotifications (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyNotifications.ID, legacyNotifications.UserID, 'WaterSupplierNotifications', 'No migrated supplier user for the recipient login'
    FROM Vepo.dbo.WaterSupplierNotifications AS legacyNotifications
    INNER JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyNotifications.WaterSupplierID
    LEFT JOIN MigrationSkippedNotifications AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyNotifications.ID
        AND alreadySkipped.SourceTable = 'WaterSupplierNotifications'
    WHERE alreadySkipped.LegacyRecordId IS NULL
        AND NOT EXISTS
        (
            SELECT 1
            FROM AspNetUsers
            INNER JOIN WaterSupplierUsers
                ON WaterSupplierUsers.UserId = AspNetUsers.Id
                AND WaterSupplierUsers.WaterSupplierId = WaterSuppliers.Id
            WHERE AspNetUsers.Email = NULLIF(legacyNotifications.UserID, '')
        )

    INSERT INTO Notifications
        (LegacyRecordId, WaterSupplierId, UserId, ModuleType, RecordId, Description, Color, ReasonForTest,
         PropertyTypeResidential, PropertyTypeCommercial, PropertyTypeAny,
         FilterFailedTest, FilterPassingTest, FilterUnknownSerialNumber, FilterInactiveProperty,
         FilterNonCompliance, FilterPotableNonPotableMismatch, FilterDuplicateTest, FilterOutOfService,
         FilterContainsRemarks, FilterBackflowNotProperlyInstalled, FilterFeeExempt,
         FilterHasOnSiteSewageFacility, FilterHasAuxWaterSupply,
         FilterSubmissionDaysExceeded, FilterSubmissionDaysExceededDays, FilterAny,
         HazardTypeAgriculturalFeedLot, HazardTypeDomesticPremisesIsolation, HazardTypeFireSystem,
         HazardTypeFireHydrantTemporaryConstruction, HazardTypeGasStationCarWash,
         HazardTypeIrrigationNonChemical, HazardTypeIrrigationChemicalFeed, HazardTypeLaundryCleaners,
         HazardTypeMedicalDentalLaboratoryMortuary, HazardTypeNailsSalonGrooming,
         HazardTypePoolRecreationAthletics, HazardTypeRestaurantVendingGrocery,
         HazardTypeFountainsGardenPondsWaterFeatures, HazardTypeWaterSoftener, HazardTypeOther, HazardTypeAny,
         [Interval], DeliveryType,
         PropertyDescription, RecordDescription, Hidden, SentTime,
         CreatedById, CreatedTime)
    SELECT
        legacyNotifications.ID,
        waterSuppliers.Id,
        recipients.UserId,
        -- Both enums carry over value for value: Backflow 0, CSI 1 (WaterSupplierNotification).
        legacyNotifications.ModuleType,
        -- A polymorphic pointer, read through ModuleType, and V2 puts no foreign key on it. It is
        -- carried over unchanged because EN-241 made the V2 row id equal the V1 record id, so these
        -- line up by themselves once the backflow test and CSI inspection migrations land. Until then
        -- the value is deliberately unresolved rather than remapped: see the README.
        legacyNotifications.RecordID,
        -- The V1 field is MaxLength="50", the same width as the V2 column, so LEFT only guards against
        -- a longer value having been posted past the browser. ISNULL keeps a NULL from failing the
        -- insert: V2 requires a description, and the V1 class defaults it to an empty string.
        LEFT(ISNULL(legacyNotifications.Description, ''), 50),
        -- V1 stores the colour as an index and turns it into a hex string only when rendering
        -- (WaterSupplierNotification.ColorString). V2 stores the hex string itself. The editor that
        -- produces these clamps anything outside 0-11 to 0, so ELSE only repeats that same fallback.
        CASE legacyNotifications.Color
            WHEN 0 THEN '#ffffff'
            WHEN 1 THEN '#e0e0e0'
            WHEN 2 THEN '#d0d0d0'
            WHEN 3 THEN '#ff0000'
            WHEN 4 THEN '#ff33cc'
            WHEN 5 THEN '#cc33ff'
            WHEN 6 THEN '#0000ff'
            WHEN 7 THEN '#00ccff'
            WHEN 8 THEN '#00ffcc'
            WHEN 9 THEN '#00ff00'
            WHEN 10 THEN '#ffff00'
            WHEN 11 THEN '#ff9900'
            ELSE '#ffffff'
        END,
        -- V1 has its own enum here (NewInstallation 0, ExistingInstallation 1, Replacement 2, Any 3)
        -- and matches a test with "BackflowTestType = ReasonForTest - 1"
        -- (WaterSupplierNotificationSetting.ValidateBackflowTest), so each value is one less than the
        -- BackflowReasonForTest it means. V2 has no Any member: it leaves ReasonForTest NULL to mean
        -- "do not filter on the reason", which is what Any did.
        CASE legacyNotifications.BackflowTestType
            WHEN 0 THEN 1
            WHEN 1 THEN 2
            WHEN 2 THEN 3
        END,
        legacyNotifications.PropertyTypeResidential,
        legacyNotifications.PropertyTypeCommercial,
        legacyNotifications.PropertyTypeAny,
        legacyNotifications.NotificationFilterFailedTest,
        legacyNotifications.NotificationFilterPassingTest,
        legacyNotifications.NotificationFilterUnknownSerialNumber,
        legacyNotifications.NotificationFilterInactiveProperty,
        legacyNotifications.NotificationFilterNonCompliance,
        legacyNotifications.NotificationFilterPotableNonPotableMismatch,
        legacyNotifications.NotificationFilterDuplicateTest,
        legacyNotifications.NotificationFilterOutOfService,
        legacyNotifications.NotificationFilterContainsRemarks,
        legacyNotifications.NotificationFilterBackflowNotProperlyInstalled,
        legacyNotifications.NotificationFilterFeeExempt,
        legacyNotifications.NotificationFilterHasOnSiteSewageFacility,
        legacyNotifications.NotificationFilterHasAuxWaterSupply,
        -- V1 misspells both of these columns as "Exeeded". V2 corrected the spelling.
        legacyNotifications.NotificationFilterSubmissionDaysExeeded,
        legacyNotifications.NotificationFilterSubmissionDaysExeededDays,
        legacyNotifications.NotificationFilterAny,
        legacyNotifications.HazardTypeAgriculturalFeedLot,
        legacyNotifications.HazardTypeDomesticPremisesIsolation,
        legacyNotifications.HazardTypeFireSystem,
        legacyNotifications.HazardTypeFireHydrantTemporaryConstruction,
        legacyNotifications.HazardTypeGasStationCarWash,
        legacyNotifications.HazardTypeIrrigationNonChemical,
        legacyNotifications.HazardTypeIrrigationChemicalFeed,
        legacyNotifications.HazardTypeLaundryCleaners,
        legacyNotifications.HazardTypeMedicalDentalLaboratoryMortuary,
        legacyNotifications.HazardTypeNailsSalonGrooming,
        legacyNotifications.HazardTypePoolRecreationAthletics,
        legacyNotifications.HazardTypeRestaurantVendingGrocery,
        -- The notifications table has no Fountains/Garden Ponds column: neither
        -- WaterSupplierNotification.Save nor its FromReader touches one, so V1 could never record the
        -- hazard on a notification even though the settings table does have it. 0 is what V1 means.
        0,
        legacyNotifications.HazardTypeWaterSoftener,
        legacyNotifications.HazardTypeOther,
        legacyNotifications.HazardTypeAny,
        legacyNotifications.NotificationInterval,
        legacyNotifications.NotificationType,
        LEFT(legacyNotifications.PropertyDescription, 200),
        legacyNotifications.RecordDescription,
        legacyNotifications.Hidden,
        -- V1 records no send time. Its scheduled jobs select by CreationDate and interval rather than
        -- stamping a row once it has gone out (scheduled/send_notification_email.aspx.vb), so there is
        -- nothing to carry and NULL says that honestly.
        NULL,
        -- V1 records who a notification is for, never who created it: the rows are raised by a test
        -- submission, not by a person. Leaving this NULL says so; the recipient is not the author.
        NULL,
        legacyNotifications.CreationDate
    FROM Vepo.dbo.WaterSupplierNotifications AS legacyNotifications
    INNER JOIN WaterSuppliers AS waterSuppliers
        ON waterSuppliers.LegacyRecordId = legacyNotifications.WaterSupplierID
    -- NULLIF stops a blank legacy login from matching the one legacy account whose email is also
    -- blank, and MIN guarantees one row per notification even if two accounts share an email address.
    CROSS APPLY
    (
        SELECT MIN(WaterSupplierUsers.UserId) AS UserId
        FROM AspNetUsers
        INNER JOIN WaterSupplierUsers
            ON WaterSupplierUsers.UserId = AspNetUsers.Id
            AND WaterSupplierUsers.WaterSupplierId = waterSuppliers.Id
        WHERE AspNetUsers.Email = NULLIF(legacyNotifications.UserID, '')
    ) AS recipients
    WHERE recipients.UserId IS NOT NULL
        AND NOT EXISTS (
            SELECT 1
            FROM Notifications AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = legacyNotifications.ID
        )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
