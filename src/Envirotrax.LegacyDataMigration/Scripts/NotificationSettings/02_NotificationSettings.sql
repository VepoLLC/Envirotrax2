BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedNotificationSettings (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacySettings.ID, legacySettings.UserID, 'WaterSupplierNotificationSettings', 'No matching migrated water supplier'
    FROM Vepo.dbo.WaterSupplierNotificationSettings AS legacySettings
    LEFT JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacySettings.WaterSupplierID
    LEFT JOIN MigrationSkippedNotificationSettings AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacySettings.ID
        AND alreadySkipped.SourceTable = 'WaterSupplierNotificationSettings'
    WHERE WaterSuppliers.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    -- A setting is delivered to exactly one person and V2 makes UserId NOT NULL, so a setting whose
    -- recipient never became a supplier user of that supplier cannot be represented at all. V1 names
    -- that person by login, which is the email their AspNetUsers account was migrated under.
    INSERT INTO MigrationSkippedNotificationSettings (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacySettings.ID, legacySettings.UserID, 'WaterSupplierNotificationSettings', 'No migrated supplier user for the recipient login'
    FROM Vepo.dbo.WaterSupplierNotificationSettings AS legacySettings
    INNER JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacySettings.WaterSupplierID
    LEFT JOIN MigrationSkippedNotificationSettings AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacySettings.ID
        AND alreadySkipped.SourceTable = 'WaterSupplierNotificationSettings'
    WHERE alreadySkipped.LegacyRecordId IS NULL
        AND NOT EXISTS
        (
            SELECT 1
            FROM AspNetUsers
            INNER JOIN WaterSupplierUsers
                ON WaterSupplierUsers.UserId = AspNetUsers.Id
                AND WaterSupplierUsers.WaterSupplierId = WaterSuppliers.Id
            WHERE AspNetUsers.Email = NULLIF(legacySettings.UserID, '')
        )

    INSERT INTO NotificationSettings
        (LegacyRecordId, WaterSupplierId, UserId, Description, Color, ReasonForTest,
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
         CreatedById, CreatedTime)
    SELECT
        legacySettings.ID,
        waterSuppliers.Id,
        recipients.UserId,
        -- The V1 field is MaxLength="50", the same width as the V2 column, so LEFT only guards against
        -- a longer value having been posted past the browser. ISNULL keeps a NULL from failing the
        -- insert: V2 requires a description, and the V1 class defaults it to an empty string.
        LEFT(ISNULL(legacySettings.Description, ''), 50),
        -- V1 stores the colour as an index and turns it into a hex string only when rendering
        -- (WaterSupplierNotificationSetting.ColorString). V2 stores the hex string itself. The page
        -- clamps anything outside 0-11 to 0 before saving, so ELSE only repeats that same fallback.
        CASE legacySettings.Color
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
        CASE legacySettings.BackflowTestType
            WHEN 0 THEN 1
            WHEN 1 THEN 2
            WHEN 2 THEN 3
        END,
        legacySettings.PropertyTypeResidential,
        legacySettings.PropertyTypeCommercial,
        legacySettings.PropertyTypeAny,
        legacySettings.NotificationFilterFailedTest,
        legacySettings.NotificationFilterPassingTest,
        legacySettings.NotificationFilterUnknownSerialNumber,
        legacySettings.NotificationFilterInactiveProperty,
        legacySettings.NotificationFilterNonCompliance,
        legacySettings.NotificationFilterPotableNonPotableMismatch,
        legacySettings.NotificationFilterDuplicateTest,
        legacySettings.NotificationFilterOutOfService,
        legacySettings.NotificationFilterContainsRemarks,
        legacySettings.NotificationFilterBackflowNotProperlyInstalled,
        legacySettings.NotificationFilterFeeExempt,
        legacySettings.NotificationFilterHasOnSiteSewageFacility,
        legacySettings.NotificationFilterHasAuxWaterSupply,
        -- V1 misspells both of these columns as "Exeeded". V2 corrected the spelling.
        legacySettings.NotificationFilterSubmissionDaysExeeded,
        legacySettings.NotificationFilterSubmissionDaysExeededDays,
        legacySettings.NotificationFilterAny,
        legacySettings.HazardTypeAgriculturalFeedLot,
        legacySettings.HazardTypeDomesticPremisesIsolation,
        legacySettings.HazardTypeFireSystem,
        legacySettings.HazardTypeFireHydrantTemporaryConstruction,
        legacySettings.HazardTypeGasStationCarWash,
        legacySettings.HazardTypeIrrigationNonChemical,
        legacySettings.HazardTypeIrrigationChemicalFeed,
        legacySettings.HazardTypeLaundryCleaners,
        legacySettings.HazardTypeMedicalDentalLaboratoryMortuary,
        legacySettings.HazardTypeNailsSalonGrooming,
        legacySettings.HazardTypePoolRecreationAthletics,
        legacySettings.HazardTypeRestaurantVendingGrocery,
        legacySettings.HazardTypeFountainsGardenPondsWaterFeatures,
        legacySettings.HazardTypeWaterSoftener,
        legacySettings.HazardTypeOther,
        legacySettings.HazardTypeAny,
        -- Both enums carry over value for value: Immediate 0, EndOfDay 1, EndOfWeek 2, EndOfMonth 3,
        -- and Email 0, SMS 1 (WaterSupplierNotification).
        legacySettings.NotificationInterval,
        legacySettings.NotificationType,
        -- V1 records who a setting is for, never who created it, so there is no author to carry over.
        -- Leaving this NULL says that honestly; filling it with the recipient would invent one.
        NULL,
        legacySettings.CreationDate
    FROM Vepo.dbo.WaterSupplierNotificationSettings AS legacySettings
    INNER JOIN WaterSuppliers AS waterSuppliers
        ON waterSuppliers.LegacyRecordId = legacySettings.WaterSupplierID
    -- NULLIF stops a blank legacy login from matching the one legacy account whose email is also
    -- blank, and MIN guarantees one row per setting even if two accounts share an email address.
    CROSS APPLY
    (
        SELECT MIN(WaterSupplierUsers.UserId) AS UserId
        FROM AspNetUsers
        INNER JOIN WaterSupplierUsers
            ON WaterSupplierUsers.UserId = AspNetUsers.Id
            AND WaterSupplierUsers.WaterSupplierId = waterSuppliers.Id
        WHERE AspNetUsers.Email = NULLIF(legacySettings.UserID, '')
    ) AS recipients
    WHERE recipients.UserId IS NOT NULL
        AND NOT EXISTS (
            SELECT 1
            FROM NotificationSettings AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = legacySettings.ID
        )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
