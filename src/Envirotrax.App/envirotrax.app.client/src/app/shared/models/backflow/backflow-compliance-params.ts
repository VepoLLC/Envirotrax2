// Query-param key contract for the Tab 2 (Current Compliance Status) "View" drill-down.
// Shared between the producer (BackflowCurrentComplianceTabComponent.viewRequirement, which builds the
// URL) and the consumer (BackflowTestListComponent.applyComplianceFilter, which reads it), so the two
// stay in sync and a rename can't silently break the drill-down.
export const BackflowComplianceParams = {
    mode: 'comp',
    propertyType: 'pt',
    deviceType: 'dt',
    hazardType: 'ht',
    ossf: 'ossf',
    auxWater: 'aws',
    ignoreLast30Days: 'ilt'
} as const;
