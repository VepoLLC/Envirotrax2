BEGIN TRAN

BEGIN TRY

    INSERT INTO MigrationSkippedSites (SiteId, SourceTable, Reason)
    SELECT legacySites.ID, 'CsiBackflowSites', 'No matching water supplier'
    FROM Vepo.dbo.CsiBackflowSites AS legacySites
    LEFT JOIN WaterSuppliers
        ON WaterSuppliers.LegacyRecordId = legacySites.WaterSupplierID
    LEFT JOIN MigrationSkippedSites AS alreadySkipped
        ON alreadySkipped.SiteId = legacySites.ID AND alreadySkipped.SourceTable = 'CsiBackflowSites'
    WHERE WaterSuppliers.Id IS NULL
        AND alreadySkipped.SiteId IS NULL

    INSERT INTO Sites
        (LegacyRecordId, WaterSupplierId, SubArea, AccountNumber, BusinessName, PropertyType,
         StreetNumber, StreetName, PropertyNumber, City, StateId, ZipCode,
         MailingCompanyName, MailingContactName, MailingStreetNumber, MailingStreetName, MailingNumber, MailingCity, MailingStateId, MailingZipCode,
         MailingPhoneNumber, MailingEmailAddress, FogGeneratorPhoneNumber, FogGeneratorEmailAddress, Comments,
         NeedsCsiInspection, CsiRenewalDate, NeedsBackflowLetter, BackflowLetterDate,
         NeedsFogInspection, FogInspectionExpirationDate, NeedsFogPermit, FogPermitExpirationDate,
         LastTripTicketDate, TripTicketInterval, IsFeeExempt, RainFreezeSensorType,
         HasKnownBackflowAssemblies, HasOnSiteSewageFacility, HasWaterWell, HasAuxWaterSupply, HasFireSystem, FireSeparateWater,
         GreaseTrapType, HasGritTrap, HasReclaimed, HasIrrigation, IrrigationSeparateWater,
         HasDomesticPremisesIsolation, RequiresDomesticPremisesIsolation, InvalidMailingAddress, OutOfArea,
         FacilityType, FacilityMap, BackflowScheduleMonth,
         GisLatitude, GisLongitude, GisStatus, GisDate, GisAreaId, GisOutOfArea, GisOutOfAreaCheckDate,
         ImportSiteId, ImportSiteId2, ImportId,
         ExcludeFromBackflowMailing, ExcludeFromCsiMailing, NeedsValidation, ValidationOnHold, BypassPropertyNumberValidation,
         UnknownAssemblyLettersSent, UnknownAssembliesLetterCount, UnknownAssembliesLetterStartDate,
         CustomData1, CustomBooleanData1,
         UserAccountAssignmentId, CsiAccountAssignmentId, BackflowAccountAssignmentId, FogAccountAssignmentId,
         NeedsRenewalCheck, CsiAccountAssignmentDate, BackflowAccountAssignmentDate, FogAccountAssignmentDate,
         Active, CreatedById, CreatedTime, UpdatedById, UpdatedTime)
    SELECT
        legacySites.ID, waterSuppliers.Id, legacySites.SubArea, legacySites.AccountNumber, legacySites.PropertyBusinessName, legacySites.PropertyType,
        legacySites.PropertyStreetNumber, legacySites.PropertyStreetName, legacySites.PropertyNumber, legacySites.PropertyCity, propertyStates.Id, legacySites.PropertyZIP,
        legacySites.MailingCompanyName, legacySites.MailingContactName, legacySites.MailingStreetNumber, legacySites.MailingStreetName, legacySites.MailingNumber, legacySites.MailingCity, mailingStates.Id, legacySites.MailingZIP,
        legacySites.MailingPhoneNumber, legacySites.MailingEmailAddress, legacySites.FogGeneratorPhoneNumber, legacySites.FogGeneratorEmailAddress, legacySites.Comments,
        legacySites.NeedsCsiInspection, legacySites.CsiRenewalDate, legacySites.NeedsBackflowLetter, legacySites.BackflowLetterDate,
        legacySites.NeedsFogInspection, legacySites.FogInspectionExpirationDate, legacySites.NeedsFogPermit, legacySites.FogPermitExpirationDate,
        legacySites.LastTripTicketDate, legacySites.TripTicketInterval, legacySites.IsFeeExempt, legacySites.RainFreezeSensorType,
        legacySites.HasKnownBackflowAssemblies, legacySites.HasOnSiteSewageFacility, legacySites.HasWaterWell, legacySites.HasAuxWaterSupply, legacySites.HasFireSystem, legacySites.FireSeparateWater,
        legacySites.HasGreaseTrap, legacySites.HasGritTrap, legacySites.HasReclaimed, legacySites.HasIrrigation, legacySites.IrrigationSeparateWater,
        legacySites.HasDomesticPremisesIsolation, legacySites.RequiresDomesticPremisesIsolation, legacySites.InvalidMailingAddress, legacySites.OutOfArea,
        CASE legacySites.FacilityType
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
        legacySites.FacilityMap, legacySites.BackflowScheduleMonth,
        legacySites.GisLatitude, legacySites.GisLongitude, legacySites.GisStatus, legacySites.GisDate, legacySites.GisAreaID, legacySites.GisOutOfArea, legacySites.GisOutOfAreaCheckDate,
        legacySites.ImportSiteID, legacySites.ImportSiteID2, legacySites.ImportID,
        legacySites.ExcludeFromBackflowMailing, legacySites.ExcludeFromCsiMailing, legacySites.NeedsValidation, legacySites.ValidationOnHold, legacySites.BypassPropertyNumberValidation,
        legacySites.UnknownAssemblyLettersSent, legacySites.UnknownAssembliesLetterCount, legacySites.UnknownAssembliesLetterStartDate,
        legacySites.CustomData1, legacySites.CustomBooleanData1,
        userAccountAssignmentUsers.UserId, csiAccountAssignmentUsers.UserId, backflowAccountAssignmentUsers.UserId, fogAccountAssignmentUsers.UserId,
        legacySites.NeedsRenewalCheck, legacySites.CsiAccountAssignmentDate, legacySites.BackflowAccountAssignmentDate, legacySites.FogAccountAssignmentDate,
        legacySites.Active,
        NULL, legacySites.CreationDate,
        lastModifiedByUsers.Id, legacySites.LastModifiedDate
    FROM Vepo.dbo.CsiBackflowSites AS legacySites
    INNER JOIN WaterSuppliers AS waterSuppliers
        ON waterSuppliers.LegacyRecordId = legacySites.WaterSupplierID
    LEFT JOIN States AS propertyStates
        ON propertyStates.Code = legacySites.PropertyState
    LEFT JOIN States AS mailingStates
        ON mailingStates.Code = legacySites.MailingState
    LEFT JOIN AspNetUsers AS userAccountAssignmentAppUsers
        ON userAccountAssignmentAppUsers.Email = legacySites.UserAccountAssignment
    LEFT JOIN WaterSupplierUsers AS userAccountAssignmentUsers
        ON userAccountAssignmentUsers.UserId = userAccountAssignmentAppUsers.Id AND userAccountAssignmentUsers.WaterSupplierId = waterSuppliers.Id
    LEFT JOIN AspNetUsers AS csiAccountAssignmentAppUsers
        ON csiAccountAssignmentAppUsers.Email = legacySites.CsiAccountAssignment
    LEFT JOIN WaterSupplierUsers AS csiAccountAssignmentUsers
        ON csiAccountAssignmentUsers.UserId = csiAccountAssignmentAppUsers.Id AND csiAccountAssignmentUsers.WaterSupplierId = waterSuppliers.Id
    LEFT JOIN AspNetUsers AS backflowAccountAssignmentAppUsers
        ON backflowAccountAssignmentAppUsers.Email = legacySites.BackflowAccountAssignment
    LEFT JOIN WaterSupplierUsers AS backflowAccountAssignmentUsers
        ON backflowAccountAssignmentUsers.UserId = backflowAccountAssignmentAppUsers.Id AND backflowAccountAssignmentUsers.WaterSupplierId = waterSuppliers.Id
    LEFT JOIN AspNetUsers AS fogAccountAssignmentAppUsers
        ON fogAccountAssignmentAppUsers.Email = legacySites.FogAccountAssignment
    LEFT JOIN WaterSupplierUsers AS fogAccountAssignmentUsers
        ON fogAccountAssignmentUsers.UserId = fogAccountAssignmentAppUsers.Id AND fogAccountAssignmentUsers.WaterSupplierId = waterSuppliers.Id
    LEFT JOIN AspNetUsers AS lastModifiedByUsers
        ON lastModifiedByUsers.Email = legacySites.LastModifiedBy
    WHERE NOT EXISTS (
        SELECT 1
        FROM Sites AS alreadyInserted
        WHERE alreadyInserted.LegacyRecordId = legacySites.ID
    )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
