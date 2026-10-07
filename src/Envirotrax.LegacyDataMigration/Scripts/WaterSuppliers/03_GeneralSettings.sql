BEGIN TRAN

BEGIN TRY

    INSERT INTO GeneralSettings
        (WaterSupplierId, PrivacyRequired, NewSitesLocked, WiseGuys, BackflowTesting, CsiInspections, FogProgram, AdministrativeOnly,
         BpatsRequireInsurance, BpatsRequireInsuranceAmount, BpatsRequireIrrigationLicense,
         CsiInspectorsRequireInsurance, CsiInspectorsRequireInsuranceAmount,
         FogTransportersRequireInsurance, FogTransportersRequireInsuranceAmount,
         FogVehiclesRequirePermit, FogVehiclesRequireInspection,
         LockBpatRegistrations, LockCsiRegistrations, LockFogInspectorRegistrations, LockFogTransporterRegistrations,
         BackflowCommercialTestFee, BackflowCommercialTestFeeWsShare, BackflowResidentialTestFee, BackflowResidentialTestFeeWsShare,
         CsiCommercialInspectionFee, CsiCommercialInspectionFeeWsShare, CsiResidentialInspectionFee, CsiResidentialInspectionFeeWsShare,
         FogTransportFee, FogTransportFeeWsShare,
         RequireBackflowTestImages, RequireCsiInspectionImages,
         RedactMailingInfo)
    SELECT
        newWaterSuppliers.Id, WaterSuppliers.PrivacyRequired, WaterSuppliers.UseSiteForWaterSupplierAssignment, WaterSuppliers.ProgramTypeWISE, WaterSuppliers.ProgramTypeBackflow,
        WaterSuppliers.ProgramTypeCSI, WaterSuppliers.ProgramTypeFOG, WaterSuppliers.ProgramTypeAdministrativeOnly,
        WaterSuppliers.SaveBPATRequiresInsurance, WaterSuppliers.SaveBPATInsuranceCoverage, WaterSuppliers.SaveBPATRequiresIrrigationLicense,
        WaterSuppliers.CsiRequiresInsurance, WaterSuppliers.CsiInsuranceCoverage,
        WaterSuppliers.FogRequiresInsurance, WaterSuppliers.FogInsuranceCoverage,
        WaterSuppliers.FogVehiclesRequirePermit, WaterSuppliers.FogVehiclesRequireInspection,
        WaterSuppliers.LockBpatRegistrations, WaterSuppliers.LockCsiRegistrations,
        WaterSuppliers.LockFogInspectorRegistrations, WaterSuppliers.LockFogTransporterRegistrations,
        WaterSuppliers.BackflowCommercialTestFee, WaterSuppliers.BackflowCommercialTestFeeShare,
        WaterSuppliers.BackflowResidentialTestFee, WaterSuppliers.BackflowResidentialTestFeeShare,
        WaterSuppliers.CsiCommercialInspectionFee, WaterSuppliers.CsiCommercialInspectionFeeShare,
        WaterSuppliers.CsiResidentialInspectionFee, WaterSuppliers.CsiResidentialInspectionFeeShare,
        WaterSuppliers.FogTransporterFee, WaterSuppliers.FogTransporterFeeShare,
        WaterSuppliers.RequireBackflowTestImages, WaterSuppliers.RequireCsiInspectionImages,
        -- V2 only checks the supplier's own setting, but V1 also redacted when either of the supplier's
        -- masters had it turned on. Carry over the value V1 actually applied, so nothing changes for
        -- existing clients.
        CASE
            WHEN WaterSuppliers.RedactMailingInfo = 1
                OR masterWaterSuppliers.RedactMailingInfo = 1
                OR secondMasterWaterSuppliers.RedactMailingInfo = 1
            THEN 1
            ELSE 0
        END
    FROM Vepo.dbo.WaterSuppliers
    INNER JOIN WaterSuppliers AS newWaterSuppliers
        ON newWaterSuppliers.LegacyRecordId = WaterSuppliers.ID
    LEFT JOIN Vepo.dbo.WaterSuppliers AS masterWaterSuppliers
        ON masterWaterSuppliers.ID = WaterSuppliers.MasterWaterSupplierID
    LEFT JOIN Vepo.dbo.WaterSuppliers AS secondMasterWaterSuppliers
        ON secondMasterWaterSuppliers.ID = WaterSuppliers.MasterWaterSupplierID2
    WHERE NOT EXISTS (
        SELECT 1
        FROM GeneralSettings AS alreadyInserted
        WHERE alreadyInserted.WaterSupplierId = newWaterSuppliers.Id
    )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
