export enum CsiInspectionReason {
    NewConstruction = 0,
    ExistingServiceContaminantHazardsSuspected = 1,
    MajorRenovationOrExpansion = 2
}

export const csiInspectionReasonLabels: Record<CsiInspectionReason, string> = {
    [CsiInspectionReason.NewConstruction]: 'New Construction',
    [CsiInspectionReason.ExistingServiceContaminantHazardsSuspected]: 'Existing Service – Contaminant Hazards Suspected',
    [CsiInspectionReason.MajorRenovationOrExpansion]: 'Major Renovation or Expansion'
};
