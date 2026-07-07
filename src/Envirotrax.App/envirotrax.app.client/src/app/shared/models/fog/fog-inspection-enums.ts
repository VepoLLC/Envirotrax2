export enum FogInspectionResult {
    Passed = 0,
    Failed = 1
}

export enum FogReasonForInspection {
    Scheduled = 0,
    Unscheduled = 1,
    Complaint = 2
}

export const fogReasonForInspectionLabels: Record<FogReasonForInspection, string> = {
    [FogReasonForInspection.Scheduled]: 'Scheduled',
    [FogReasonForInspection.Unscheduled]: 'Unscheduled',
    [FogReasonForInspection.Complaint]: 'Complaint'
};
