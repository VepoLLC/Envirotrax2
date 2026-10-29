BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedFogInspections (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyInspections.ID, legacyInspections.InspectorID, 'FogInspections', 'No matching migrated water supplier'
    FROM Vepo.dbo.FogInspections AS legacyInspections
    LEFT JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyInspections.WaterSupplierID
    LEFT JOIN MigrationSkippedFogInspections AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyInspections.ID
        AND alreadySkipped.SourceTable = 'FogInspections'
    WHERE WaterSuppliers.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    -- SiteId is NOT NULL in V2, so an inspection whose site did not migrate cannot be represented.
    INSERT INTO MigrationSkippedFogInspections (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyInspections.ID, legacyInspections.InspectorID, 'FogInspections', 'No matching migrated site'
    FROM Vepo.dbo.FogInspections AS legacyInspections
    INNER JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyInspections.WaterSupplierID
    LEFT JOIN Sites
        ON Sites.LegacyRecordId = legacyInspections.SiteID
    LEFT JOIN MigrationSkippedFogInspections AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyInspections.ID
        AND alreadySkipped.SourceTable = 'FogInspections'
    WHERE Sites.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    -- ProfessionalId and InspectorId are NOT NULL in V2, so an inspection whose inspector login matches
    -- no migrated professional user cannot be represented. ProfessionalUsers.LegacyUserId already
    -- carries the V1 login, and 04_ProfessionalUsers.sql resolved sub-accounts to their company.
    INSERT INTO MigrationSkippedFogInspections (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyInspections.ID, legacyInspections.InspectorID, 'FogInspections', 'No migrated professional user for the inspector login'
    FROM Vepo.dbo.FogInspections AS legacyInspections
    INNER JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyInspections.WaterSupplierID
    INNER JOIN Sites
        ON Sites.LegacyRecordId = legacyInspections.SiteID
    LEFT JOIN MigrationSkippedFogInspections AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyInspections.ID
        AND alreadySkipped.SourceTable = 'FogInspections'
    WHERE alreadySkipped.LegacyRecordId IS NULL
        AND NOT EXISTS
        (
            SELECT 1
            FROM ProfessionalUsers AS professionalUsers
            WHERE professionalUsers.LegacyUserId = NULLIF(legacyInspections.InspectorID, '')
        )

    -- V2 assigns the inspection a new Id. LegacyRecordId keeps the V1 record id, and it is what links
    -- the row back to V1: every guard below, the image upload and any later migration read it.
    INSERT INTO FogInspections
        -- Identity
        (LegacyRecordId, WaterSupplierId, SiteId, InspectionDate, SubmissionId,

        -- Property / Location fields
         PropertyBusinessName, PropertyType, PropertyStreetNumber, PropertyStreetName, PropertyNumber, PropertyCity,
         PropertyStateId, PropertyZip,

        -- Mailing / Contact fields
         MailingCompanyName, MailingContactName, MailingStreetNumber, MailingStreetName, MailingNumber, MailingCity,
         MailingStateId, MailingZip, MailingPhoneNumber, MailingEmailAddress,

        -- Inspector fields
         ProfessionalId, InspectorId, InspectorCompanyName, InspectorJobTitle, InspectorContactName, InspectorAddress,
         InspectorCity, InspectorState, InspectorZip, InspectorWorkNumber, InspectorCellNumber, InspectorFaxNumber,

        -- FOG specific fields
         FogGeneratorPhoneNumber, FogGeneratorEmailAddress, FacilityType, ReasonForInspection,

        -- Interceptor fields
         InterceptorType, InterceptorOtherDescription, InterceptorCapacity, InterceptorCapacityType,
         InterceptorLocationDescription, InterceptorLatitude, InterceptorLongitude, InterceptorComments,

        -- Condition fields
         Maintained, Accessible, PastOverflow,

        -- Chamber fields
         InletChamberWettingHeight, InletChamberGreaseBlanket, InletChamberSediments, OutletChamberWettingHeight,
         OutletChamberGreaseBlanket, OutletChamberSediments, InletTeeIntact, OutletTeeIntact, InletTeeVisible,
         OutletTeeVisible,

        -- Sampling fields
         SampledFrom, SamplingPointAccessible, SamplingPointClean,

        -- Capacity / Result
         InletTotalCapacityPercent, OutletTotalCapacityPercent, TotalCapacityPercent, InspectionResult,

        -- Signature / Comments
         SignatureContactName, SignatureDate, Comments,

        -- Images
         ExteriorImagePath, InteriorImagePath, SignatureImagePath, LegacyExteriorImagePath, LegacyInteriorImagePath,
         HasLegacySignature,

        -- Trip ticket fields
         LastTripTicketDate, TripTicketInterval,

        -- Validation
         NeedsValidation, ValidationNewSite, ValidationSiteInformationChanged, ValidationOnHold,

        -- Payment / Transaction
         TransactionId, TransactionDate, Amount, AmountShare, EmailPdf,

        -- Audit
         CreatedById, CreatedTime, UpdatedById, UpdatedTime, DeletedById, DeletedTime)
    SELECT
        -- Identity
        legacyInspections.ID, waterSuppliers.Id, sites.Id, legacyInspections.InspectionDate,
        legacyInspections.SubmissionID,

        -- Property / Location fields
        legacyInspections.PropertyBusinessName, ISNULL(legacyInspections.PropertyType, 0),
        legacyInspections.PropertyStreetNumber, legacyInspections.PropertyStreetName,
        legacyInspections.PropertyNumber, legacyInspections.PropertyCity, propertyStates.Id,
        legacyInspections.PropertyZIP,

        -- Mailing / Contact fields
        legacyInspections.MailingCompanyName, legacyInspections.MailingContactName,
        legacyInspections.MailingStreetNumber, legacyInspections.MailingStreetName,
        legacyInspections.MailingNumber, legacyInspections.MailingCity, mailingStates.Id,
        legacyInspections.MailingZIP, legacyInspections.MailingPhoneNumber,
        legacyInspections.MailingEmailAddress,

        -- Inspector fields. V1 never stores a job title on a FOG inspection (FogInspection.FromReader and
        -- inspection_submit.aspx.vb leave it out), so the V2 snapshot of it stays empty.
        inspectors.ProfessionalId, inspectors.UserId, legacyInspections.InspectorCompanyName, NULL,
        legacyInspections.InspectorContactName, legacyInspections.InspectorAddress,
        legacyInspections.InspectorCity, legacyInspections.InspectorState,
        legacyInspections.InspectorZIP, legacyInspections.InspectorWorkNumber,
        legacyInspections.InspectorCellNumber, legacyInspections.InspectorFaxNumber,

        -- FOG specific fields
        legacyInspections.FogGeneratorPhoneNumber, legacyInspections.FogGeneratorEmailAddress,
        -- V1 stores the facility type as the text of the inspection form's dropdown, V2 as an enum. Same
        -- table as Scripts/Sites/02_Sites.sql: the inspection form only ever offered the enum's own names,
        -- and the extra spellings below come from sites, so they cost nothing and keep the two in step.
        CASE legacyInspections.FacilityType
            WHEN 'School/University' THEN 5 WHEN 'Schools' THEN 5 WHEN 'School [not FWISD]' THEN 5
            WHEN 'Private School' THEN 5 WHEN 'College/University' THEN 5 WHEN 'Middle School' THEN 5
            WHEN 'School/ Sevice Centers' THEN 5
            WHEN 'Fast Food Establishment' THEN 2 WHEN 'Fast Fodd Restaurant' THEN 2 WHEN 'Fast Food Restaurant' THEN 2
            WHEN 'Restaurant' THEN 1 WHEN 'Restaurant/Bars' THEN 1 WHEN 'Restaurant/Deli' THEN 1
            WHEN 'Car Wash' THEN 4 WHEN 'Carwash' THEN 4
            WHEN 'Grocery Store' THEN 6 WHEN 'Super Markets' THEN 6
            WHEN 'Convenience Store' THEN 7
            WHEN 'Assisted Living Center' THEN 8 WHEN 'Assisted Living Facility' THEN 8 WHEN 'Assisted Living/Retirement' THEN 8
            WHEN 'Medical Facility' THEN 9 WHEN 'Medical Office' THEN 9 WHEN 'Hospital' THEN 9 WHEN 'Clinic' THEN 9
            WHEN 'Hotel/Motel' THEN 3
            WHEN 'Industrial' THEN 10 WHEN 'General - Industry' THEN 10
            WHEN 'City Owned Facility' THEN 11
            ELSE 0
        END,
        -- Value for value: Scheduled 0, Unscheduled 1, Complaint 2 in both (the dropdown index minus one).
        ISNULL(legacyInspections.ReasonForInspection, 0),

        -- Interceptor fields. V1 keeps the coordinates as text and V2 as numbers; NULLIF stops a blank
        -- one from converting to 0, which would pin the interceptor to latitude/longitude 0,0.
        legacyInspections.InterceptorType, legacyInspections.InterceptorOtherDescription,
        ISNULL(legacyInspections.InterceptorCapacity, 0), ISNULL(legacyInspections.InterceptorCapacityType, 0),
        legacyInspections.InterceptorLocationDescription,
        TRY_CAST(NULLIF(legacyInspections.InterceptorLatitude, '') AS FLOAT),
        TRY_CAST(NULLIF(legacyInspections.InterceptorLongitude, '') AS FLOAT),
        legacyInspections.InterceptorComments,

        -- Condition fields
        ISNULL(legacyInspections.Maintained, 0), ISNULL(legacyInspections.Accessible, 0),
        ISNULL(legacyInspections.PastOverflow, 0),

        -- Chamber fields
        legacyInspections.InletChamberWettingHeight, legacyInspections.InletChamberGreaseBlanket,
        legacyInspections.InletChamberSediments, legacyInspections.OutletChamberWettingHeight,
        legacyInspections.OutletChamberGreaseBlanket, legacyInspections.OutletChamberSediments,
        ISNULL(legacyInspections.InletTeeIntact, 0), ISNULL(legacyInspections.OutletTeeIntact, 0),
        legacyInspections.InletTeeVisible, legacyInspections.OutletTeeVisible,

        -- Sampling fields
        legacyInspections.SampledFrom, ISNULL(legacyInspections.SamplingPointAccessible, 0),
        ISNULL(legacyInspections.SamplingPointClean, 0),

        -- Capacity / Result
        ISNULL(legacyInspections.InletTotalCapacityPercent, 0), ISNULL(legacyInspections.OutletTotalCapacityPercent, 0),
        ISNULL(legacyInspections.TotalCapacityPercent, 0), ISNULL(legacyInspections.InspectionResult, 0),

        -- Signature / Comments
        legacyInspections.SignatureContactName, legacyInspections.SignatureDate, legacyInspections.Comments,

        -- Images. The three V2 paths are left NULL on purpose: FogInspectionService fills each one in only
        -- once its upload to Azure has returned, so they double as the "still pending" flag.
        NULL, NULL, NULL,
        -- V1 stores no path for a photo. Every page that shows one rebuilds it from the record id as
        -- fog_inspections/<ID rounded down to 10000>/<ID>-ext.jpg (or -int.jpg) and shows it whenever
        -- OutsideImageType (or InsideImageType) is not NULL, always as a JPG whatever that column holds
        -- (FogInspection.vb, fog_inspectors/inspection_submit.aspx.vb, water_suppliers/fog_inspection.aspx.vb).
        -- The same test is used here; whether the file is really still on that server is settled when
        -- FogInspectionService tries to fetch it.
        CASE
            WHEN legacyInspections.OutsideImageType IS NOT NULL
            THEN CONCAT('fog_inspections/', CAST(FLOOR(legacyInspections.ID / 10000) * 10000 AS VARCHAR(20)), '/',
                        CAST(legacyInspections.ID AS VARCHAR(20)), '-ext.jpg')
        END,
        CASE
            WHEN legacyInspections.InsideImageType IS NOT NULL
            THEN CONCAT('fog_inspections/', CAST(FLOOR(legacyInspections.ID / 10000) * 10000 AS VARCHAR(20)), '/',
                        CAST(legacyInspections.ID AS VARCHAR(20)), '-int.jpg')
        END,
        CASE WHEN DATALENGTH(legacyInspections.Signature) > 0 THEN 1 END,

        -- Trip ticket fields. V1 keeps the trip ticket schedule on the site, not on an inspection, and
        -- the sites migration has already carried it over there.
        NULL, 0,

        -- Validation
        ISNULL(legacyInspections.NeedsValidation, 0), ISNULL(legacyInspections.ValidationNewSite, 0),
        ISNULL(legacyInspections.ValidationSiteInformationChanged, 0), ISNULL(legacyInspections.ValidationOnHold, 0),

        -- Payment / Transaction. V1 marks an unpaid inspection with an empty TransactionID, V2 with NULL,
        -- and some V2 checks only test for NULL - the report download among them - so '' must not survive.
        -- V1 has no "email PDF" flag on a FOG inspection.
        NULLIF(LTRIM(RTRIM(legacyInspections.TransactionID)), ''), legacyInspections.TransactionDate,
        ISNULL(legacyInspections.Amount, 0), ISNULL(legacyInspections.AmountShare, 0), 0,

        -- Audit. CreatedTime is deliberately not defaulted: there is no honest stand-in for when an
        -- inspection was recorded, so a NULL there should stop the run.
        NULL, legacyInspections.CreationDate, NULL, NULL, NULL, NULL
    FROM Vepo.dbo.FogInspections AS legacyInspections
    INNER JOIN WaterSuppliers AS waterSuppliers
        ON waterSuppliers.LegacyRecordId = legacyInspections.WaterSupplierID
    INNER JOIN Sites AS sites
        ON sites.LegacyRecordId = legacyInspections.SiteID
    -- V1 names the inspector by login and their company by MasterInspectorID (empty when the inspector
    -- is the company). One login can belong to more than one company in V2, and the composite foreign
    -- key only admits one pairing, so the company the inspection was filed under is preferred, then
    -- a pairing that really is a FOG inspector; ordering by id makes the remaining choice repeatable.
    CROSS APPLY
    (
        SELECT TOP 1 professionalUsers.ProfessionalId, professionalUsers.UserId
        FROM ProfessionalUsers AS professionalUsers
        INNER JOIN Professionals AS professionals
            ON professionals.Id = professionalUsers.ProfessionalId
        WHERE professionalUsers.LegacyUserId = NULLIF(legacyInspections.InspectorID, '')
        ORDER BY
            CASE
                WHEN professionals.LegacyUserId = ISNULL(NULLIF(legacyInspections.MasterInspectorID, ''), legacyInspections.InspectorID)
                THEN 0
                ELSE 1
            END,
            CASE WHEN professionalUsers.IsFogInspector = 1 THEN 0 ELSE 1 END,
            professionalUsers.ProfessionalId
    ) AS inspectors
    -- V1 stores a state code on the inspection itself, the same way the sites migration reads one.
    LEFT JOIN States AS propertyStates
        ON propertyStates.Code = legacyInspections.PropertyState
    LEFT JOIN States AS mailingStates
        ON mailingStates.Code = legacyInspections.MailingState
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM FogInspections AS alreadyInserted
        WHERE alreadyInserted.LegacyRecordId = legacyInspections.ID
    )

    -- An inspection skipped by an earlier run, say because its site had not been migrated yet, goes in
    -- once that is fixed. Its entry no longer describes anything, so it is cleared to keep this table
    -- a list of what is really missing.
    DELETE MigrationSkippedFogInspections
    FROM MigrationSkippedFogInspections
    INNER JOIN FogInspections
        ON FogInspections.LegacyRecordId = MigrationSkippedFogInspections.LegacyRecordId
    WHERE MigrationSkippedFogInspections.SourceTable = 'FogInspections'

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
