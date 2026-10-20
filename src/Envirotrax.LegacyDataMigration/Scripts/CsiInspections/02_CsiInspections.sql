BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedCsiInspections (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyInspections.ID, legacyInspections.InspectorID, 'CsiInspections', 'No matching migrated water supplier'
    FROM Vepo.dbo.CsiInspections AS legacyInspections
    LEFT JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyInspections.WaterSupplierID
    LEFT JOIN MigrationSkippedCsiInspections AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyInspections.ID
        AND alreadySkipped.SourceTable = 'CsiInspections'
    WHERE WaterSuppliers.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    INSERT INTO MigrationSkippedCsiInspections (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyInspections.ID, legacyInspections.InspectorID, 'CsiInspections', 'No matching migrated site'
    FROM Vepo.dbo.CsiInspections AS legacyInspections
    INNER JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyInspections.WaterSupplierID
    LEFT JOIN Sites
        ON Sites.LegacyRecordId = legacyInspections.SiteID
    LEFT JOIN MigrationSkippedCsiInspections AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyInspections.ID
        AND alreadySkipped.SourceTable = 'CsiInspections'
    WHERE Sites.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    -- V2 requires a professional inspector. Inspections entered by water supplier staff, by a bulk import
    -- or with a blank login have none, so they are recorded here instead.
    INSERT INTO MigrationSkippedCsiInspections (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyInspections.ID, legacyInspections.InspectorID, 'CsiInspections', 'No migrated professional user for the inspector login'
    FROM Vepo.dbo.CsiInspections AS legacyInspections
    INNER JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyInspections.WaterSupplierID
    INNER JOIN Sites
        ON Sites.LegacyRecordId = legacyInspections.SiteID
    LEFT JOIN MigrationSkippedCsiInspections AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyInspections.ID
        AND alreadySkipped.SourceTable = 'CsiInspections'
    WHERE alreadySkipped.LegacyRecordId IS NULL
        AND NOT EXISTS
        (
            SELECT 1
            FROM ProfessionalUsers AS professionalUsers
            WHERE professionalUsers.LegacyUserId = NULLIF(legacyInspections.InspectorID, '')
        )

    INSERT INTO CsiInspections
        (LegacyRecordId, WaterSupplierId, SiteId, InspectionDate, SubmissionId,

         PropertyBusinessName, PropertyType, PropertyStreetNumber, PropertyStreetName, PropertyNumber, PropertyCity,
         PropertyStateId, PropertyZip,

         MailingCompanyName, MailingContactName, MailingStreetNumber, MailingStreetName, MailingNumber, MailingCity,
         MailingStateId, MailingZip, MailingPhoneNumber, MailingEmailAddress,

         ProfessionalId, InspectorId, InspectorLicenseNumber, InspectorLicenseType, InspectorCompanyName, InspectorJobTitle,
         InspectorContactName, InspectorAddress, InspectorCity, InspectorState, InspectorZip, InspectorWorkNumber,
         InspectorCellNumber, InspectorFaxNumber,

         ReasonForInspection, Compliance1, Compliance2, Compliance3, Compliance4, Compliance5, Compliance6,

         MaterialServiceLineLead, MaterialServiceLineCopper, MaterialServiceLinePVC, MaterialServiceLineOther,
         MaterialServiceLineOtherDescription, MaterialSolderLead, MaterialSolderLeadFree, MaterialSolderSolventWeld,
         MaterialSolderOther, MaterialSolderOtherDescription,

         Disapproved, DisapprovedReason,

         AiOssf, AiWaterWell, AiFireSystem, AiFireSystem2, AiGreaseTrap, AiSandGrit, AiReclaimedWater, AiIrrigationSystem,
         AiIrrigationSystem2, AiHasDomesticPremisesIsolation, AiRequiresDomesticPremisesIsolation,

         Comments, NeedsValidation, ValidationNewSite, ValidationSiteInformationChanged,

         TransactionId, TransactionDate, Amount, AmountShare, EmailPdf,

         CreatedById, CreatedTime, UpdatedById, UpdatedTime)
    SELECT
        legacyInspections.ID, waterSuppliers.Id, sites.Id, legacyInspections.InspectionDate,
        LEFT(legacyInspections.SubmissionID, 50),

        LEFT(legacyInspections.PropertyBusinessName, 100), ISNULL(legacyInspections.PropertyType, 0),
        LEFT(legacyInspections.PropertyStreetNumber, 50), LEFT(legacyInspections.PropertyStreetName, 100),
        LEFT(legacyInspections.PropertyNumber, 50), LEFT(legacyInspections.PropertyCity, 50), propertyStates.Id,
        LEFT(legacyInspections.PropertyZIP, 20),

        LEFT(legacyInspections.MailingCompanyName, 100), LEFT(legacyInspections.MailingContactName, 100),
        LEFT(legacyInspections.MailingStreetNumber, 50), LEFT(legacyInspections.MailingStreetName, 100),
        LEFT(legacyInspections.MailingNumber, 50), LEFT(legacyInspections.MailingCity, 50), mailingStates.Id,
        LEFT(legacyInspections.MailingZIP, 20), LEFT(legacyInspections.MailingPhoneNumber, 50),
        LEFT(legacyInspections.MailingEmailAddress, 100),

        inspectors.ProfessionalId, inspectors.UserId, LEFT(legacyInspections.InspectorLicenseNumber, 50),
        LEFT(legacyInspections.InspectorLicenseType, 50), LEFT(legacyInspections.InspectorCompanyName, 100),
        LEFT(legacyInspections.InspectorJobTitle, 100), LEFT(legacyInspections.InspectorContactName, 100),
        LEFT(legacyInspections.InspectorAddress, 200), LEFT(legacyInspections.InspectorCity, 50),
        LEFT(legacyInspections.InspectorState, 50), LEFT(legacyInspections.InspectorZIP, 20),
        LEFT(legacyInspections.InspectorWorkNumber, 50), LEFT(legacyInspections.InspectorCellNumber, 50),
        LEFT(legacyInspections.InspectorFaxNumber, 50),

        ISNULL(legacyInspections.ReasonForInspection, 0), ISNULL(legacyInspections.Compliance1, 0),
        ISNULL(legacyInspections.Compliance2, 0), ISNULL(legacyInspections.Compliance3, 0),
        ISNULL(legacyInspections.Compliance4, 0),
        -- V1 did not ask question 5 before 2016 and ignores it when deciding pass/fail. V2 computes
        -- InspectionResult from all six answers, so it is set here to keep the V1 result.
        CASE
            WHEN legacyInspections.CreationDate < '20160101' THEN 1
            ELSE ISNULL(legacyInspections.Compliance5, 0)
        END,
        ISNULL(legacyInspections.Compliance6, 0),

        ISNULL(legacyInspections.MaterialServiceLineLead, 0), ISNULL(legacyInspections.MaterialServiceLineCopper, 0),
        ISNULL(legacyInspections.MaterialServiceLinePVC, 0), ISNULL(legacyInspections.MaterialServiceLineOther, 0),
        LEFT(legacyInspections.MaterialServiceLineOtherDescription, 200), ISNULL(legacyInspections.MaterialSolderLead, 0),
        ISNULL(legacyInspections.MaterialSolderLeadFree, 0), ISNULL(legacyInspections.MaterialSolderSolventWeld, 0),
        ISNULL(legacyInspections.MaterialSolderOther, 0), LEFT(legacyInspections.MaterialSolderOtherDescription, 200),

        ISNULL(legacyInspections.Disapproved, 0), legacyInspections.DisapprovedReason,

        ISNULL(legacyInspections.AiOssf, 0), ISNULL(legacyInspections.AiWaterWell, 0),
        ISNULL(legacyInspections.AiFireSystem, 0), ISNULL(legacyInspections.AiFireSystem2, 0),
        ISNULL(legacyInspections.AiGreaseTrap, 0), ISNULL(legacyInspections.AiSandGrit, 0),
        ISNULL(legacyInspections.AiReclaimedWater, 0), ISNULL(legacyInspections.AiIrrigationSystem, 0),
        ISNULL(legacyInspections.AiIrrigationSystem2, 0), ISNULL(legacyInspections.AiHasDomesticPremisesIsolation, 0),
        ISNULL(legacyInspections.AiRequiresDomesticPremisesIsolation, 0),

        NULLIF(CAST(legacyInspections.Comments AS NVARCHAR(MAX)), ''), ISNULL(legacyInspections.NeedsValidation, 0),
        ISNULL(legacyInspections.ValidationNewSite, 0), ISNULL(legacyInspections.ValidationSiteInformationChanged, 0),

        -- An empty TransactionID is an unpaid inspection still in the inspector's cart.
        LEFT(NULLIF(legacyInspections.TransactionID, ''), 100), legacyInspections.TransactionDate,
        ISNULL(legacyInspections.Amount, 0), ISNULL(legacyInspections.AmountShare, 0),
        ISNULL(legacyInspections.EmailPDF, 0),

        inspectors.UserId, legacyInspections.CreationDate, modifiedByUsers.Id, legacyInspections.LastModifiedDate
    FROM Vepo.dbo.CsiInspections AS legacyInspections
    INNER JOIN WaterSuppliers AS waterSuppliers
        ON waterSuppliers.LegacyRecordId = legacyInspections.WaterSupplierID
    INNER JOIN Sites AS sites
        ON sites.LegacyRecordId = legacyInspections.SiteID
    -- One login can belong to more than one company, so the company the inspection was submitted under
    -- (MasterInspectorID, or the login itself for a company owner) is preferred.
    CROSS APPLY
    (
        SELECT TOP 1 professionalUsers.ProfessionalId, professionalUsers.UserId
        FROM ProfessionalUsers AS professionalUsers
        INNER JOIN Professionals AS professionals
            ON professionals.Id = professionalUsers.ProfessionalId
        WHERE professionalUsers.LegacyUserId = NULLIF(legacyInspections.InspectorID, '')
        ORDER BY
            CASE
                WHEN professionals.LegacyUserId = ISNULL(NULLIF(legacyInspections.MasterInspectorID, ''), legacyInspections.InspectorID) THEN 0
                ELSE 1
            END,
            professionalUsers.ProfessionalId,
            professionalUsers.UserId
    ) AS inspectors
    LEFT JOIN States AS propertyStates
        ON propertyStates.Code = legacyInspections.PropertyState
    LEFT JOIN States AS mailingStates
        ON mailingStates.Code = legacyInspections.MailingState
    OUTER APPLY
    (
        SELECT MIN(AspNetUsers.Id) AS Id
        FROM AspNetUsers
        WHERE AspNetUsers.Email = NULLIF(legacyInspections.LastModifiedBy, '')
    ) AS modifiedByUsers
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM CsiInspections AS alreadyInserted
        WHERE alreadyInserted.LegacyRecordId = legacyInspections.ID
    )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
