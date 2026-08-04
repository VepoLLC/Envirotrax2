BEGIN TRAN

BEGIN TRY

    INSERT INTO Envirotrax2Dev.dbo.GeneralSettings
        (WaterSupplierId, PrivacyRequired, NewSitesLocked, WiseGuys, BackflowTesting, CsiInspections, FogProgram, AdministrativeOnly,
         BpatsRequireInsurance, BpatsRequireInsuranceAmount, BpatsRequireIrrigationLicense,
         CsiInspectorsRequireInsurance, CsiInspectorsRequireInsuranceAmount,
         FogTransportersRequireInsurance, FogTransportersRequireInsuranceAmount,
         FogVehiclesRequirePermit, FogVehiclesRequireInspection,
         LockBpatRegistrations, LockCsiRegistrations, LockFogInspectorRegistrations, LockFogTransporterRegistrations,
         BackflowCommercialTestFee, BackflowCommercialTestFeeWsShare, BackflowResidentialTestFee, BackflowResidentialTestFeeWsShare,
         CsiCommercialInspectionFee, CsiCommercialInspectionFeeWsShare, CsiResidentialInspectionFee, CsiResidentialInspectionFeeWsShare,
         FogTransportFee, FogTransportFeeWsShare,
         RequireBackflowTestImages, RequireCsiInspectionImages)
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
        WaterSuppliers.RequireBackflowTestImages, WaterSuppliers.RequireCsiInspectionImages
    FROM WaterSuppliers
    INNER JOIN Envirotrax2Dev.dbo.WaterSuppliers AS newWaterSuppliers
        ON newWaterSuppliers.LegacyRecordId = WaterSuppliers.ID
    WHERE NOT EXISTS (
        SELECT 1
        FROM Envirotrax2Dev.dbo.GeneralSettings AS alreadyInserted
        WHERE alreadyInserted.WaterSupplierId = newWaterSuppliers.Id
    )

    COMMIT TRAN

END TRY
BEGIN CATCH
    ROLLBACK TRAN;
    THROW;
END CATCH
