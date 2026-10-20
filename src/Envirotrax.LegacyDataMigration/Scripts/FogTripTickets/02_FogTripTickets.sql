BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedFogTripTickets (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyTickets.ID, legacyTickets.TransporterID, 'FogTripTickets', 'No matching migrated water supplier'
    FROM Vepo.dbo.FogTripTickets AS legacyTickets
    LEFT JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyTickets.WaterSupplierID
    LEFT JOIN MigrationSkippedFogTripTickets AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyTickets.ID
        AND alreadySkipped.SourceTable = 'FogTripTickets'
    WHERE WaterSuppliers.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    -- SiteId is NOT NULL in V2, so a ticket whose site did not migrate cannot be represented.
    INSERT INTO MigrationSkippedFogTripTickets (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyTickets.ID, legacyTickets.TransporterID, 'FogTripTickets', 'No matching migrated site'
    FROM Vepo.dbo.FogTripTickets AS legacyTickets
    INNER JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyTickets.WaterSupplierID
    LEFT JOIN Sites
        ON Sites.LegacyRecordId = legacyTickets.SiteID
    LEFT JOIN MigrationSkippedFogTripTickets AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyTickets.ID
        AND alreadySkipped.SourceTable = 'FogTripTickets'
    WHERE Sites.Id IS NULL
        AND alreadySkipped.LegacyRecordId IS NULL

    -- ProfessionalId is NOT NULL in V2, but TransporterId is not, so a ticket only needs its company. It
    -- is skipped when neither the transporter login nor the company it was filed under (MasterTransporterID,
    -- empty when the transporter is the company) has been migrated. These are the same two lookups the
    -- insert below makes.
    INSERT INTO MigrationSkippedFogTripTickets (LegacyRecordId, LegacyUserId, SourceTable, Reason)
    SELECT legacyTickets.ID, legacyTickets.TransporterID, 'FogTripTickets', 'No migrated transporter or transporter company for the ticket'
    FROM Vepo.dbo.FogTripTickets AS legacyTickets
    INNER JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacyTickets.WaterSupplierID
    INNER JOIN Sites
        ON Sites.LegacyRecordId = legacyTickets.SiteID
    LEFT JOIN MigrationSkippedFogTripTickets AS alreadySkipped
        ON alreadySkipped.LegacyRecordId = legacyTickets.ID
        AND alreadySkipped.SourceTable = 'FogTripTickets'
    WHERE alreadySkipped.LegacyRecordId IS NULL
        AND NOT EXISTS
        (
            SELECT 1
            FROM ProfessionalUsers AS professionalUsers
            WHERE professionalUsers.LegacyUserId = NULLIF(legacyTickets.TransporterID, '')
        )
        AND NOT EXISTS
        (
            SELECT 1
            FROM Professionals AS professionals
            WHERE professionals.LegacyUserId = ISNULL(NULLIF(legacyTickets.MasterTransporterID, ''), legacyTickets.TransporterID)
        )

    -- V2 assigns the ticket a new Id. LegacyRecordId keeps the V1 record id and is what links the row back
    -- to V1: every guard here and the signature upload read it.
    -- Every text column is copied as it is, without LEFT: V1 declares each one no wider than its V2
    -- column (checked against a restored V1 copy), so nothing can be cut, and a value that did not fit
    -- would stop the run instead of being shortened silently.
    INSERT INTO FogTripTickets
        -- Identity
        (LegacyRecordId, WaterSupplierId, SubmissionId, SiteId,

        -- Property / Generator
         PropertyBusinessName, PropertyType, PropertyStreetNumber, PropertyStreetName, PropertyNumber, PropertyCity,
         PropertyStateId, PropertyZip, FogGeneratorPhoneNumber, FogGeneratorEmailAddress, FogGeneratorContactName,

        -- Transporter
         ProfessionalId, TransporterId, TransporterLicenseNumber, TransporterLicenseExpiration, TransporterCompanyName,
         TransporterContactName, TransporterAddress, TransporterCity, TransporterState, TransporterZip,
         TransporterWorkNumber, TransporterCellNumber, TransporterFaxNumber, TransporterEmailAddress,
         TransporterSignaturePath, TransporterSignatureDate, GeneratorContactName, GeneratorSignaturePath,
         GeneratorSignatureDate,

        -- Interceptor
         InterceptorType, InterceptorOtherDescription, InterceptorCapacity, InterceptorCapacityType,
         InterceptorWasteRemovedAmount, InterceptorWasteRemovedType, InterceptorWasteRemovedAmountGallons,
         InterceptorWasteRemovedAmountCubicFeet, InterceptorWasteRemovedDate,

        -- Vehicle
         VehicleId, VehicleLicensePlateNumber, VehicleManufacturer, VehicleYear, VehicleCapacity, VehicleCapacityType,
         VehicleStickerNumber, VehiclePermitNumber,

        -- Receiver / Disposal Site
         ReceiverSiteId, ReceiverDisposalSiteId, ReceiverCompanyName, ReceiverContactName, ReceiverAddress, ReceiverCity,
         ReceiverState, ReceiverZip, ReceiverPhoneNumber, ReceiverEmailAddress, ReceiverRegistrationNumber,
         ReceiverPermitNumber, ReceiverWasteDeliveredDate, ReceiverSignaturePath, ReceiverSignatureDate,

        -- Status
         PickupCompleted, Completed, Disapproved, ApprovalDate, ApprovedById,

        -- Validation
         NeedsValidation, ValidationOnHold, ValidatedBy, ValidationClearedDate, ValidationLockedDate, ValidationNewSite,
         ValidationSiteInformationChanged, ValidationReceiverInformationChanged, ValidationNotes,

        -- Payment / Transaction
         TransactionId, TransactionDate, Amount, AmountShare, WsPaidDate, SalesRepPaidDate,

        -- Email / Comments
         EmailPdf, EmailStatus, Comments,

        -- Signatures still to copy into Azure Storage
         HasLegacyTransporterSignature, HasLegacyGeneratorSignature, HasLegacyReceiverSignature,

        -- Audit
         CreatedById, CreatedTime, UpdatedById, UpdatedTime, DeletedById, DeletedTime)
    SELECT
        -- Identity
        legacyTickets.ID, waterSuppliers.Id, legacyTickets.SubmissionID, sites.Id,

        -- Property / Generator
        legacyTickets.PropertyBusinessName, legacyTickets.PropertyType, legacyTickets.PropertyStreetNumber,
        legacyTickets.PropertyStreetName, legacyTickets.PropertyNumber, legacyTickets.PropertyCity, propertyStates.Id,
        legacyTickets.PropertyZIP, legacyTickets.FogGeneratorPhoneNumber, legacyTickets.FogGeneratorEmailAddress,
        legacyTickets.FogGeneratorContactName,

        -- Transporter. The transporter user is preferred, which keeps the company/user pairing the composite
        -- foreign key needs; when the user did not migrate, the ticket still goes in under its company.
        COALESCE(transporters.ProfessionalId, companies.Id), transporters.UserId, legacyTickets.TransporterLicenseNumber,
        legacyTickets.TransporterLicenseExpiration, legacyTickets.TransporterCompanyName,
        legacyTickets.TransporterContactName, legacyTickets.TransporterAddress, legacyTickets.TransporterCity,
        legacyTickets.TransporterState, legacyTickets.TransporterZIP, legacyTickets.TransporterWorkNumber,
        legacyTickets.TransporterCellNumber, legacyTickets.TransporterFaxNumber, legacyTickets.TransporterEmailAddress,
        -- The three signature paths are left NULL on purpose: FogTripTicketService fills each one in only once
        -- its upload to Azure has returned, so they double as the "still pending" flag.
        NULL,
        -- V1 has no transporter signature date. It copies the transporter's signature onto the ticket when the
        -- ticket is submitted (transporters/trip_ticket_submit.aspx.vb), and V2 stamps the date at that same
        -- moment, so the ticket's creation date is the honest equivalent.
        CASE WHEN DATALENGTH(legacyTickets.TransporterSignature) > 0 THEN legacyTickets.CreationDate END,
        legacyTickets.GeneratorContactName, NULL, legacyTickets.GeneratorSignatureDate,

        -- Interceptor. The capacity types carry over value for value (Gallons 0, Cubic Yards 1). A few V1 rows
        -- hold -1, the form's "not selected", which V1 shows as no unit; it is kept rather than turned into one.
        legacyTickets.InterceptorType, legacyTickets.InterceptorOtherDescription, legacyTickets.InterceptorCapacity,
        legacyTickets.InterceptorCapacityType, legacyTickets.InterceptorWasteRemovedAmount,
        legacyTickets.InterceptorWasteRemovedType, legacyTickets.InterceptorWasteRemovedAmountGallons,
        legacyTickets.InterceptorWasteRemovedAmountCubicFeet, legacyTickets.InterceptorWasteRemovedDate,

        -- Vehicle. V1 deletes a vehicle outright, so a ticket can outlive the vehicle it names. VehicleId is then
        -- NULL, and the snapshot columns still say which vehicle it was.
        vehicles.Id, legacyTickets.VehicleLicensePlateNumber, legacyTickets.VehicleManufacturer,
        legacyTickets.VehicleYear, legacyTickets.VehicleCapacity, legacyTickets.VehicleCapacityType,
        legacyTickets.VehicleStickerNumber, legacyTickets.VehiclePermitNumber,

        -- Receiver / Disposal Site. V2 reads only ReceiverDisposalSiteId; ReceiverSiteId is left NULL, as V2
        -- leaves it on the tickets it creates.
        NULL, disposalSites.Id, legacyTickets.ReceiverCompanyName, legacyTickets.ReceiverContactName,
        legacyTickets.ReceiverAddress, legacyTickets.ReceiverCity, legacyTickets.ReceiverState,
        legacyTickets.ReceiverZIP, legacyTickets.ReceiverPhoneNumber, legacyTickets.ReceiverEmailAddress,
        legacyTickets.ReceiverRegistrationNumber, legacyTickets.ReceiverPermitNumber,
        legacyTickets.ReceiverWasteDeliveredDate, NULL, legacyTickets.ReceiverSignatureDate,

        -- Status
        legacyTickets.PickupCompleted, legacyTickets.Completed, legacyTickets.Disapproved, legacyTickets.ApprovalDate,
        approvers.UserId,

        -- Validation
        legacyTickets.NeedsValidation, legacyTickets.ValidationOnHold, legacyTickets.ValidatedBy,
        legacyTickets.ValidationClearedDate, legacyTickets.ValidationLockedDate, legacyTickets.ValidationNewSite,
        legacyTickets.ValidationSiteInformationChanged, legacyTickets.ValidationReceiverInformationChanged,
        legacyTickets.ValidationNotes,

        -- Payment / Transaction. V1 marks an unpaid ticket with an empty TransactionID, V2 with NULL, and
        -- some V2 checks only test for NULL, so '' must not survive.
        NULLIF(LTRIM(RTRIM(legacyTickets.TransactionID)), ''), legacyTickets.TransactionDate, legacyTickets.Amount,
        legacyTickets.AmountShare, legacyTickets.WsPaidDate, legacyTickets.SalesRepPaidDate,

        -- Email / Comments
        legacyTickets.EmailPDF, legacyTickets.EmailStatus, legacyTickets.Comments,

        -- Signatures still to copy into Azure Storage
        CASE WHEN DATALENGTH(legacyTickets.TransporterSignature) > 0 THEN 1 END,
        CASE WHEN DATALENGTH(legacyTickets.GeneratorSignature) > 0 THEN 1 END,
        CASE WHEN DATALENGTH(legacyTickets.ReceiverSignature) > 0 THEN 1 END,

        -- Audit. CreatedTime is deliberately not defaulted: there is no honest stand-in for when a ticket was
        -- recorded, so a NULL there should stop the run. LastModifiedDate is only a modification when
        -- LastModifiedBy says who made it; otherwise it is the insert default, and V2 leaves UpdatedTime NULL
        -- on a ticket nobody changed. V1 deletes a ticket outright, so none is deleted here.
        NULL, legacyTickets.CreationDate, lastModifiedByUsers.Id,
        CASE WHEN legacyTickets.LastModifiedBy <> '' THEN legacyTickets.LastModifiedDate END, NULL, NULL
    FROM Vepo.dbo.FogTripTickets AS legacyTickets
    INNER JOIN WaterSuppliers AS waterSuppliers
        ON waterSuppliers.LegacyRecordId = legacyTickets.WaterSupplierID
    INNER JOIN Sites AS sites
        ON sites.LegacyRecordId = legacyTickets.SiteID
    -- V1 names the transporter by login and their company by MasterTransporterID (empty when the
    -- transporter is the company). One login can belong to more than one company in V2, and the composite
    -- foreign key only admits one pairing, so the company the ticket was filed under is preferred, then a
    -- pairing that really is a FOG transporter; ordering by id makes the remaining choice repeatable.
    OUTER APPLY
    (
        SELECT TOP 1 professionalUsers.ProfessionalId, professionalUsers.UserId
        FROM ProfessionalUsers AS professionalUsers
        INNER JOIN Professionals AS professionals
            ON professionals.Id = professionalUsers.ProfessionalId
        WHERE professionalUsers.LegacyUserId = NULLIF(legacyTickets.TransporterID, '')
        ORDER BY
            CASE
                WHEN professionals.LegacyUserId = ISNULL(NULLIF(legacyTickets.MasterTransporterID, ''), legacyTickets.TransporterID)
                THEN 0
                ELSE 1
            END,
            CASE WHEN professionalUsers.IsFogTransporter = 1 THEN 0 ELSE 1 END,
            professionalUsers.ProfessionalId
    ) AS transporters
    -- The company the ticket was filed under, used when the transporter user did not migrate.
    -- Professionals.LegacyUserId is unique (03_Professionals.sql), so this is at most one row.
    OUTER APPLY
    (
        SELECT professionals.Id
        FROM Professionals AS professionals
        WHERE professionals.LegacyUserId = ISNULL(NULLIF(legacyTickets.MasterTransporterID, ''), legacyTickets.TransporterID)
    ) AS companies
    LEFT JOIN FogVehicles AS vehicles
        ON vehicles.LegacyRecordId = legacyTickets.VehicleID
    LEFT JOIN FogDisposalSites AS disposalSites
        ON disposalSites.LegacyRecordId = legacyTickets.ReceiverSiteID
    -- V1 stores a state code on the ticket itself, the same way the sites migration reads one.
    LEFT JOIN States AS propertyStates
        ON propertyStates.Code = legacyTickets.PropertyState
    -- V1 records the approving supplier user by login (water_suppliers/fog_trip_ticket.aspx.vb). NULLIF keeps
    -- a blank login from matching the one legacy account whose email is blank.
    OUTER APPLY
    (
        SELECT MIN(waterSupplierUsers.UserId) AS UserId
        FROM AspNetUsers
        INNER JOIN WaterSupplierUsers AS waterSupplierUsers
            ON waterSupplierUsers.UserId = AspNetUsers.Id
            AND waterSupplierUsers.WaterSupplierId = waterSuppliers.Id
        WHERE AspNetUsers.Email = NULLIF(legacyTickets.ApprovedBy, '')
    ) AS approvers
    -- LastModifiedBy is a login too, written only when a supplier user approves or disapproves the ticket
    -- (water_suppliers/fog_trip_ticket.aspx.vb) and blank otherwise. Read the same way the sites migration
    -- reads it.
    OUTER APPLY
    (
        SELECT MIN(AspNetUsers.Id) AS Id
        FROM AspNetUsers
        WHERE AspNetUsers.Email = NULLIF(legacyTickets.LastModifiedBy, '')
    ) AS lastModifiedByUsers
    WHERE COALESCE(transporters.ProfessionalId, companies.Id) IS NOT NULL
        AND NOT EXISTS
        (
            SELECT 1
            FROM FogTripTickets AS alreadyInserted
            WHERE alreadyInserted.LegacyRecordId = legacyTickets.ID
        )

    -- A ticket skipped by an earlier run, say because its site had not been migrated yet, goes in once that
    -- is fixed. Its entry no longer describes anything, so it is cleared to keep this table a list of what
    -- is really missing.
    DELETE MigrationSkippedFogTripTickets
    FROM MigrationSkippedFogTripTickets
    INNER JOIN FogTripTickets
        ON FogTripTickets.LegacyRecordId = MigrationSkippedFogTripTickets.LegacyRecordId
    WHERE MigrationSkippedFogTripTickets.SourceTable = 'FogTripTickets'

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
