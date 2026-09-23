export interface BackflowComplianceReport {
    totalActive: number;
    compliant: number;
    nonCompliant: number;
    compliantPercentage: number;
    nonCompliantPercentage: number;
    requirements: BackflowComplianceRequirement[];
}

export interface BackflowComplianceRequirement {
    propertyType: string;
    assemblyType: string;
    hazardType: string;
    hasSiteOssf: boolean;
    auxWaterSupply: boolean;
    renewalYears: number;
    active: number;
    compliant: number;
    percentage: number;
}
