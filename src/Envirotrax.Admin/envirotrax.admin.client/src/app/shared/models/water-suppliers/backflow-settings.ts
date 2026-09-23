
export enum BackflowTestingMethodType {
    USC = 0,
    ASSE = 1,
    TREEO = 2
}

export enum BackflowOutOfServiceType {
    VepoManaged = 0,
    WaterSupplierManaged = 1
}

export class BackflowSettings {
    id?: number;

    testingMethod?: BackflowTestingMethodType;
    gracePeriodDays?: number | null;
    adjustBackflowCreepingDates?: boolean;
    newInstallationsRequireApproval?: boolean;
    replacementsRequireApproval?: boolean;
    detectorAssembliesRequireMeterReading?: boolean;
    outOfServiceRequiresApproval?: boolean;
    outOfServiceType?: BackflowOutOfServiceType;
    requireBackflowTestImages?: boolean;

    showWaterMeterNumber?: boolean;
    showRainSensor?: boolean;
    showOSSF?: boolean;
    showPermitNumber?: boolean;
}
