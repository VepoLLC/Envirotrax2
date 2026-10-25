BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedBackflowTests (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyTests.ID, legacyTests.BpatID, 'SaveBackflowDeviceTests', 'No matching migrated water supplier'
    FROM Vepo.dbo.SaveBackflowDeviceTests AS legacyTests
    LEFT JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyTests.WaterSupplierID
    LEFT JOIN MigrationSkippedBackflowTests AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyTests.ID
        AND alreadySkipped.SourceTable = 'SaveBackflowDeviceTests'
    WHERE WaterSuppliers.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    -- ProfessionalId is NOT NULL in V2, so a test whose tester login matches no migrated professional
    -- user cannot be represented. ProfessionalUsers.LegacyUserId already carries the V1 login, and
    -- 04_ProfessionalUsers.sql resolved sub-accounts to their company, so one join settles both.
    INSERT INTO MigrationSkippedBackflowTests (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyTests.ID, ISNULL(NULLIF(legacyTests.BpatID, ''), legacyTests.InspectorID),
           'SaveBackflowDeviceTests', 'No migrated professional user for the tester login'
    FROM Vepo.dbo.SaveBackflowDeviceTests AS legacyTests
    INNER JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyTests.WaterSupplierID
    LEFT JOIN MigrationSkippedBackflowTests AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyTests.ID
        AND alreadySkipped.SourceTable = 'SaveBackflowDeviceTests'
    WHERE alreadySkipped.LegacyRecordId IS NULL
        AND NOT EXISTS
        (
            SELECT 1
            FROM ProfessionalUsers AS professionalUsers
            WHERE professionalUsers.LegacyUserId IN (NULLIF(legacyTests.BpatID, ''), NULLIF(legacyTests.InspectorID, ''))
        )

    -- The V1 record id becomes the V2 row id, the way sites and water suppliers are migrated (EN-241).
    -- Backflow tests are the entity that depends on it most: Notifications.RecordId and
    -- SiteLogs.LegacyAssemblyRecordId both point here by V1 id and resolve the moment this lands.
    SET IDENTITY_INSERT BackflowTests ON;

    INSERT INTO BackflowTests
        -- Identity
        (Id, LegacyRecordId, WaterSupplierId, SiteId, SubmissionId, JobNumber, ProfessionalId, BpatId, InspectorId,
         BpatLicenseNumber, BpatLicenseExpiration, BpatCompanyName, BpatContactName, BpatAddress, BpatCity, BpatStateId,
         BpatZip, BpatWorkNumber, BpatCellNumber,

        -- Property / Site fields
         AccountNumber, PropertyBusinessName, PropertyType, PropertyStreetNumber, PropertyStreetName, PropertyNumber,
         PropertyCity, PropertyStateId, PropertyZip,

        -- Mailing address
         MailingCompanyName, MailingContactName, MailingStreetNumber, MailingStreetName, MailingNumber, MailingCity,
         MailingStateId, MailingZip, MailingPhoneNumber, MailingEmailAddress, MailingAddress,

        -- Device info
         DeviceType, Manufacturer, Model, Size, SerialNumber, UnknownSerialNumber, Manufacturer2, Model2, Size2,
         SerialNumber2, LocationDescription, HazardType, HazardTypeOtherDescription,

        -- Test info
         ReasonForTest, ReplacementAssembly, InstallationDate, TestDate, InitialTestDate, RepairTestDate, FinalTestDate,
         ExpirationDate, TestResult, ProperlyInstalled, NonPotable,

        -- Gauge
         GaugeManufacturer, GaugeModel, GaugeSerialNumber, GaugeLastCalibrationDate, GaugeNonPotable,

        -- Optional fields
         MeterNumber, MeterReadingBefore, MeterRegisters, MeterReadingAfter, PermitNumber, Ossf, WaterMeterNumber,
         RainFreezeSensorInstalled, RainFreezeSensorWorkingProperly,

        -- Status
         Comments, IsCurrent, Disapproved, OutOfService, OutOfServiceDate,

        -- Approval / Payment
         ApprovalDate, ApprovedById, TransactionId, TransactionDate, Amount, AmountShare, EmailPdf,

        -- Rejection
         Rejected, RejectedById, RejectedDate, RejectedReason,

        -- Validation flags
         NeedsValidation, ValidationNewSite, ValidationSiteInformationChanged, ValidationUnknownSerialNumber,
         ValidationDeviceInformationChanged, ValidationNotes, ValidationReplacementOnHold, ValidationReplacementCleared,

        -- Initial test readings — main assembly
         InitCV1HeldPSID, InitCV1ClosedTight, InitCV1Leaked, InitCV2HeldPSID, InitCV2ClosedTight, InitCV2Leaked,
         InitRVOpenedPSID, InitRVDidNotOpen, InitBCHeldPSID, InitBCClosedTight, InitBCLeaked, InitPvbAirInletOpenedPSID,
         InitPvbAirInletDidNotOpen, InitPvbAirInletFullyOpened, InitPvbCVHeldPSID, InitPvbCVLeaked,

        -- Air gap
         AirGapValid, AirGapTestDate,

        -- Repairs
         RepairCV1, RepairCV1Details, RepairCV2, RepairCV2Details, RepairRV, RepairRVDetails, RepairBC, RepairBCDetails,

        -- Final test readings — main assembly
         FinalCV1HeldPSID, FinalCV1ClosedTight, FinalCV2HeldPSID, FinalCV2ClosedTight, FinalRVOpenedPSID, FinalBCHeldPSID,
         FinalBCClosedTight, FinalPvbAirInletOpenedPSID, FinalPvbAirInletFullyOpened, FinalPvbCVHeldPSID,

        -- Bypass assembly readings
         InitCV1HeldPSID2, InitCV1ClosedTight2, InitCV1Leaked2, InitCV2HeldPSID2, InitCV2ClosedTight2, InitCV2Leaked2,
         InitRVOpenedPSID2, InitRVDidNotOpen2, RepairCV12, RepairCV1Details2, RepairCV22, RepairCV2Details2, RepairRV2,
         RepairRVDetails2, RepairPvbAirInlet, RepairPvbCV, RepairPvbAirInletDetails, RepairPvbCVDetails, FinalCV1HeldPSID2,
         FinalCV1ClosedTight2, FinalCV2HeldPSID2, FinalCV2ClosedTight2, FinalRVOpenedPSID2,

        -- Images
         AssemblyImagePath, SerialNumberImagePath, BypassAssemblyImagePath, BypassSerialNumberImagePath, AirGapImagePath,

        -- Audit
         CreatedById, CreatedTime, UpdatedById, UpdatedTime, DeletedById, DeletedTime, NeedsRenewalCheck, RenewalRequired,
         BackflowScheduleMonth, ForceRenewal, ForceRenewalYears)
    SELECT
        -- Identity
        legacyTests.ID, legacyTests.ID, waterSuppliers.Id, sites.Id, LEFT(legacyTests.SubmissionID, 50),
        LEFT(legacyTests.JobNumber, 50), COALESCE(bpats.ProfessionalId, inspectors.ProfessionalId), bpats.UserId,
        CASE WHEN bpats.ProfessionalId IS NULL THEN inspectors.UserId END, LEFT(legacyTests.BpatLicenseNumber, 50),
        legacyTests.BpatLicenseExpiration, LEFT(legacyTests.BpatCompanyName, 100), LEFT(legacyTests.BpatContactName, 100),
        LEFT(legacyTests.BpatAddress, 200), LEFT(legacyTests.BpatCity, 50), bpatStates.Id, LEFT(legacyTests.BpatZIP, 20),
        LEFT(legacyTests.BpatWorkNumber, 50), LEFT(legacyTests.BpatCellNumber, 50),

        -- Property / Site fields
        NULL, LEFT(legacyTests.PropertyBusinessName, 100), ISNULL(legacyTests.PropertyType, 0),
        LEFT(legacyTests.PropertyStreetNumber, 50), LEFT(legacyTests.PropertyStreetName, 100),
        LEFT(legacyTests.PropertyNumber, 50), LEFT(legacyTests.PropertyCity, 50), propertyStates.Id,
        LEFT(legacyTests.PropertyZIP, 20),

        -- Mailing address
        LEFT(legacyTests.MailingCompanyName, 100), LEFT(legacyTests.MailingContactName, 100),
        LEFT(legacyTests.MailingStreetNumber, 50), LEFT(legacyTests.MailingStreetName, 100),
        LEFT(legacyTests.MailingNumber, 50), LEFT(legacyTests.MailingCity, 50), mailingStates.Id,
        LEFT(legacyTests.MailingZIP, 20), LEFT(legacyTests.MailingPhoneNumber, 50),
        LEFT(legacyTests.MailingEmailAddress, 100), NULL,

        -- Device info
        LEFT(legacyTests.DeviceType, 50), LEFT(legacyTests.Manufacturer, 100), LEFT(legacyTests.Model, 100),
        LEFT(legacyTests.Size, 50), LEFT(legacyTests.SerialNumber, 100), ISNULL(legacyTests.UnknownSerialNumber, 0),
        LEFT(legacyTests.Manufacturer2, 100), LEFT(legacyTests.Model2, 100), LEFT(legacyTests.Size2, 50),
        LEFT(legacyTests.SerialNumber2, 100), LEFT(legacyTests.LocationDescription, 200), LEFT(legacyTests.HazardType, 100),
        LEFT(legacyTests.HazardTypeOtherDescription, 200),

        -- Test info
        ISNULL(legacyTests.ReasonForTest, 0), LEFT(legacyTests.ReplacementAssembly, 100), legacyTests.InstallationDate,
        legacyTests.TestDate, legacyTests.InitialTestDate, legacyTests.RepairTestDate, NULL, legacyTests.ExpirationDate,
        ISNULL(legacyTests.TestResult, 0), ISNULL(legacyTests.ProperlyInstalled, 0), ISNULL(legacyTests.NonPotable, 0),

        -- Gauge
        LEFT(legacyTests.GaugeManufacturer, 100), LEFT(legacyTests.GaugeModel, 100),
        LEFT(legacyTests.GaugeSerialNumber, 50), legacyTests.GaugeLastCalibrationDate,
        ISNULL(legacyTests.GaugeNonPotable, 0),

        -- Optional fields
        LEFT(legacyTests.MeterNumber, 50), legacyTests.MeterReadingBefore, ISNULL(legacyTests.MeterRegisters, 0),
        legacyTests.MeterReadingAfter, LEFT(legacyTests.PermitNumber, 50), legacyTests.OSSF,
        LEFT(legacyTests.WaterMeterNumber, 50), ISNULL(legacyTests.RainFreezeSensorInstalled, 0),
        ISNULL(legacyTests.RainFreezeSensorWorkingProperly, 0),

        -- Status
        legacyTests.Comments, ISNULL(legacyTests.IsCurrent, 0), ISNULL(legacyTests.Disapproved, 0),
        ISNULL(legacyTests.OutOfService, 0), NULL,

        -- Approval / Payment
        NULL, NULL, LEFT(legacyTests.TransactionID, 100), legacyTests.TransactionDate, ISNULL(legacyTests.Amount, 0),
        ISNULL(legacyTests.AmountShare, 0), legacyTests.EmailPDF,

        -- Rejection
        ISNULL(legacyTests.Rejected, 0), rejectedByUsers.UserId, legacyTests.RejectedDate, legacyTests.RejectedReason,

        -- Validation flags
        ISNULL(legacyTests.NeedsValidation, 0), ISNULL(legacyTests.ValidationNewSite, 0),
        ISNULL(legacyTests.ValidationSiteInformationChanged, 0), ISNULL(legacyTests.ValidationUnknownSerialNumber, 0),
        ISNULL(legacyTests.ValidationDeviceInformationChanged, 0), NULL, 0, 0,

        -- Initial test readings — main assembly
        legacyTests.InitCV1HeldPSID, ISNULL(legacyTests.InitCV1ClosedTight, 0), ISNULL(legacyTests.InitCV1Leaked, 0),
        legacyTests.InitCV2HeldPSID, ISNULL(legacyTests.InitCV2ClosedTight, 0), ISNULL(legacyTests.InitCV2Leaked, 0),
        legacyTests.InitRVOpenedPSID, ISNULL(legacyTests.InitRVDidNotOpen, 0), legacyTests.InitBCHeldPSID,
        ISNULL(legacyTests.InitBCClosedTight, 0), ISNULL(legacyTests.InitBCLeaked, 0),
        legacyTests.InitPvbAirInletOpenedPSID, ISNULL(legacyTests.InitPvbAirInletDidNotOpen, 0),
        legacyTests.InitPvbAirInletDidItFullyOpen, legacyTests.InitPvbCVHeldPSID, ISNULL(legacyTests.InitPvbCVLeaked, 0),

        -- Air gap
        ISNULL(legacyTests.AirGapValid, 0), legacyTests.AirGapTestDate,

        -- Repairs
        LEFT(legacyTests.RepairCV1, 200), LEFT(legacyTests.RepairCV1Details, 100), LEFT(legacyTests.RepairCV2, 200),
        LEFT(legacyTests.RepairCV2Details, 100), LEFT(legacyTests.RepairRV, 250), LEFT(legacyTests.RepairRVDetails, 100),
        LEFT(legacyTests.RepairBC, 200), LEFT(legacyTests.RepairBCDetails, 100),

        -- Final test readings — main assembly
        legacyTests.FinalCV1HeldPSID, ISNULL(legacyTests.FinalCV1ClosedTight, 0), legacyTests.FinalCV2HeldPSID,
        ISNULL(legacyTests.FinalCV2ClosedTight, 0), legacyTests.FinalRVOpenedPSID, legacyTests.FinalBCHeldPSID,
        ISNULL(legacyTests.FinalBCClosedTight, 0), legacyTests.FinalPvbAirInletOpenedPSID,
        legacyTests.FinalPvbAirInletDidItFullyOpen, legacyTests.FinalPvbCVHeldPSID,

        -- Bypass assembly readings
        legacyTests.InitCV1HeldPSID2, ISNULL(legacyTests.InitCV1ClosedTight2, 0), ISNULL(legacyTests.InitCV1Leaked2, 0),
        legacyTests.InitCV2HeldPSID2, ISNULL(legacyTests.InitCV2ClosedTight2, 0), ISNULL(legacyTests.InitCV2Leaked2, 0),
        legacyTests.InitRVOpenedPSID2, ISNULL(legacyTests.InitRVDidNotOpen2, 0), LEFT(legacyTests.RepairCV12, 200),
        LEFT(legacyTests.RepairCV1Details2, 100), LEFT(legacyTests.RepairCV22, 200),
        LEFT(legacyTests.RepairCV2Details2, 100), LEFT(legacyTests.RepairRV2, 250), LEFT(legacyTests.RepairRVDetails2, 100),
        LEFT(legacyTests.RepairPvbAirInlet, 200), LEFT(legacyTests.RepairPvbCV, 200),
        LEFT(legacyTests.RepairPvbAirInletDetails, 100), LEFT(legacyTests.RepairPvbCVDetails, 100),
        legacyTests.FinalCV1HeldPSID2, ISNULL(legacyTests.FinalCV1ClosedTight2, 0), legacyTests.FinalCV2HeldPSID2,
        ISNULL(legacyTests.FinalCV2ClosedTight2, 0), legacyTests.FinalRVOpenedPSID2,

        -- Images
        NULL, NULL, NULL, NULL, NULL,

        -- Audit
        NULL, legacyTests.CreationDate, NULL, NULL, NULL, NULL, 0, ISNULL(legacyTests.RenewalRequired, 0),
        ISNULL(legacyTests.BackflowScheduleMonth, 0), ISNULL(legacyTests.ForceRenewal, 0),
        ISNULL(legacyTests.ForceRenewalYears, 0)
    FROM Vepo.dbo.SaveBackflowDeviceTests AS legacyTests
    INNER JOIN WaterSuppliers AS waterSuppliers
        ON waterSuppliers.LegacyRecordId = legacyTests.WaterSupplierID
    -- Optional in V2: a test can outlive the site it was recorded against.
    LEFT JOIN Sites AS sites
        ON sites.LegacyRecordId = legacyTests.SiteID
    -- A test is recorded by a BPAT or by a CSI inspector, never both, and V2 gives them one shared
    -- ProfessionalId (BackflowTestConfiguration). TOP 1 rather than a plain join, because one login can
    -- belong to more than one company in V1 and the composite foreign key only admits one pairing;
    -- ordering makes the choice repeatable.
    OUTER APPLY
    (
        SELECT TOP 1 professionalUsers.ProfessionalId, professionalUsers.UserId
        FROM ProfessionalUsers AS professionalUsers
        WHERE professionalUsers.LegacyUserId = NULLIF(legacyTests.BpatID, '')
        ORDER BY professionalUsers.ProfessionalId, professionalUsers.UserId
    ) AS bpats
    OUTER APPLY
    (
        SELECT TOP 1 professionalUsers.ProfessionalId, professionalUsers.UserId
        FROM ProfessionalUsers AS professionalUsers
        WHERE professionalUsers.LegacyUserId = NULLIF(legacyTests.InspectorID, '')
        ORDER BY professionalUsers.ProfessionalId, professionalUsers.UserId
    ) AS inspectors
    -- V1 stores a state code on the test itself, the same way the sites migration reads one.
    LEFT JOIN States AS bpatStates
        ON bpatStates.Code = legacyTests.BpatState
    LEFT JOIN States AS propertyStates
        ON propertyStates.Code = legacyTests.PropertyState
    LEFT JOIN States AS mailingStates
        ON mailingStates.Code = legacyTests.MailingState
    -- NULLIF keeps a blank rejecting login from matching the one legacy account whose email is blank.
    OUTER APPLY
    (
        SELECT MIN(waterSupplierUsers.UserId) AS UserId
        FROM AspNetUsers
        INNER JOIN WaterSupplierUsers AS waterSupplierUsers
            ON waterSupplierUsers.UserId = AspNetUsers.Id
            AND waterSupplierUsers.WaterSupplierId = waterSuppliers.Id
        WHERE AspNetUsers.Email = NULLIF(legacyTests.RejectedBy, '')
    ) AS rejectedByUsers
    WHERE COALESCE(bpats.ProfessionalId, inspectors.ProfessionalId) IS NOT NULL
        AND NOT EXISTS (
            SELECT 1
            FROM BackflowTests AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = legacyTests.ID
        )

    SET IDENTITY_INSERT BackflowTests OFF;

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
