export interface GeneralSettings {
    id?: number;
    waterSupplierId?: number;

    // Program Settings - Checkboxes
    privacyRequired?: boolean;
    newSitesLocked?: boolean;
    administrativeOnly?: boolean;
    wiseGuys?: boolean;
    backflowTesting?: boolean;
    csiInspections?: boolean;
    fogProgram?: boolean;

    // Insurance and License Requirements
    bpatsRequireInsurance?: boolean;
    bpatsRequireIrrigationLicense?: boolean;
    csiInspectorsRequireInsurance?: boolean;
    fogTransportersRequireInsurance?: boolean;
    fogVehiclesRequirePermit?: boolean;
    fogVehiclesRequireInspection?: boolean;

    // Lock Registration Options
    lockBpatRegistrations?: boolean;
    lockCsiRegistrations?: boolean;
    lockFogInspectorRegistrations?: boolean;
    lockFogTransporterRegistrations?: boolean;

    // Insurance Amount Fields
    bpatsRequireInsuranceAmount?: number;
    csiInspectorsRequireInsuranceAmount?: number;
    fogTransportersRequireInsuranceAmount?: number;

    // Submission Fees - Total Fee and WS Share
    backflowCommercialTestFee?: number;
    backflowCommercialTestFeeWsShare?: number;
    backflowResidentialTestFee?: number;
    backflowResidentialTestFeeWsShare?: number;
    csiCommercialInspectionFee?: number;
    csiCommercialInspectionFeeWsShare?: number;
    csiResidentialInspectionFee?: number;
    csiResidentialInspectionFeeWsShare?: number;
    fogTransportFee?: number;
    fogTransportFeeWsShare?: number;
}
