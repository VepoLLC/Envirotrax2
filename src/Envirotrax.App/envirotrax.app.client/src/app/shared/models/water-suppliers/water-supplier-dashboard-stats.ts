export interface WaterSupplierDashboardStats {
    //Property Logs
    pastDuePropertyLogCount: number;
    expiringPropertyLogCount: number;
    allPropertyLogCount: number;

    // Accounts
    wiseGuyCount: number;
    csiInspectorCount: number;
    bpatCount: number;
    fogTransporterCount: number;
    fogInspectorCount: number;

    // Licensing
    unverifiedLicenseCount: number;
    expiredLicenseCount: number;
    expiringLicenseCount: number;

    // Required Validations
    insurancePolicyCount: number;
    testGaugeCount: number;
    transporterRegistrationCount: number;
}
