import { State } from '../lookup/state';
import { PropertyType } from '../sites/site';
import { WaterSupplier } from '../water-suppliers/water-supplier';

export enum BackflowPaymentStatus {
    Paid = 1,
    Unpaid = 2
}

export enum BackflowTestResult {
    Pass = 0,
    Fail = 1,
    PassAfterRepairs = 2
}

export enum BackflowReasonForTest {
    AnnualTest = 0,
    NewInstallation = 1,
    ExistingInstallation = 2,
    Replacement = 3,
    Repair = 4,
    AnnualTestAfterRepairs = 5
}

export enum BackflowDeviceType {
    DC = 'DC',
    DCD = 'DCD',
    DCD2 = 'DCD2',
    RP = 'RP',
    RPPD = 'RPPD',
    RPPD2 = 'RPPD2',
    PVB = 'PVB',
    SVB = 'SVB',
    AG = 'AG'
}

export const backflowDeviceTypeDescriptions: Record<string, string> = {
    [BackflowDeviceType.DC]: 'Double Check Valve',
    [BackflowDeviceType.DCD]: 'Double Check Detector',
    [BackflowDeviceType.DCD2]: 'Double Check Detector Type II',
    [BackflowDeviceType.RP]: 'Reduced Pressure Principle',
    [BackflowDeviceType.RPPD]: 'Reduced Pressure Principle Detector',
    [BackflowDeviceType.RPPD2]: 'Reduced Pressure Principle Detector Type II',
    [BackflowDeviceType.PVB]: 'Pressure Vacuum Breaker',
    [BackflowDeviceType.SVB]: 'Spill-Resistant Pressure Vacuum Breaker',
    [BackflowDeviceType.AG]: 'Air Gap'
};

export class BackflowTest {
    id?: number;
    waterSupplier?: WaterSupplier;
    accountNumber?: string;
    createdTime?: string;
    testDate?: string;
    expirationDate?: string;
    testResult?: BackflowTestResult;
    isCurrent?: boolean;
    outOfService?: boolean;
    disapproved?: boolean;
    rejected?: boolean;
    transactionId?: string;
    serialNumber?: string;
    serialNumber2?: string;
    manufacturer?: string;
    model?: string;
    size?: string;
    deviceType?: string;
    hazardType?: string;
    locationDescription?: string;
    propertyType?: PropertyType;
    propertyBusinessName?: string;
    propertyStreetNumber?: string;
    propertyStreetName?: string;
    propertyNumber?: string;
    propertyCity?: string;
    propertyState?: State;
    propertyZip?: string;
    bpatCompanyName?: string;
    bpatContactName?: string;
    bpatLicenseNumber?: string;
}

export class BackflowTestReviewer {
    id?: number;
    contactName?: string;
    emailAddress?: string;
}

export class BackflowTestSite {
    id?: number;
    accountNumber?: string;
}

export class BackflowTestBpat {
    id?: number;
    emailAddress?: string;
    contactName?: string;
}

export class BackflowTestCounts {
    recordLogCount?: number;
}

export class BackflowTestDetails {
    id?: number;
    waterSupplier?: WaterSupplier;
    site?: BackflowTestSite;
    bpat?: BackflowTestBpat;
    submissionId?: string;
    accountNumber?: string;

    isCurrent?: boolean;
    outOfService?: boolean;
    outOfServiceDate?: string;
    disapproved?: boolean;
    rejected?: boolean;
    needsValidation?: boolean;
    forceRenewal?: boolean;
    forceRenewalYears?: number;
    backflowScheduleMonth?: number;
    renewalRequired?: boolean;
    needsRenewalCheck?: boolean;

    approvalDate?: string;
    approvedBy?: BackflowTestReviewer;
    rejectedDate?: string;
    rejectedBy?: BackflowTestReviewer;
    rejectedReason?: string;

    validationNewSite?: boolean;
    validationSiteInformationChanged?: boolean;
    validationUnknownSerialNumber?: boolean;
    validationDeviceInformationChanged?: boolean;
    validationNotes?: string;

    testDate?: string;
    expirationDate?: string;
    installationDate?: string;

    transactionId?: string;
    transactionDate?: string;
    amount?: number;
    amountShare?: number;

    bpatCompanyName?: string;
    bpatJobTitle?: string;
    bpatContactName?: string;
    bpatAddress?: string;
    bpatCity?: string;
    bpatState?: State;
    bpatZip?: string;
    bpatWorkNumber?: string;
    bpatCellNumber?: string;
    bpatLicenseNumber?: string;
    bpatLicenseExpiration?: string;

    propertyType?: PropertyType;
    propertyBusinessName?: string;
    propertyStreetNumber?: string;
    propertyStreetName?: string;
    propertyNumber?: string;
    propertyCity?: string;
    propertyState?: State;
    propertyZip?: string;

    mailingCompanyName?: string;
    mailingContactName?: string;
    mailingStreetNumber?: string;
    mailingStreetName?: string;
    mailingNumber?: string;
    mailingCity?: string;
    mailingState?: State;
    mailingZip?: string;

    deviceType?: string;
    manufacturer?: string;
    model?: string;
    size?: string;
    serialNumber?: string;
    unknownSerialNumber?: boolean;
    manufacturer2?: string;
    model2?: string;
    size2?: string;
    serialNumber2?: string;
    locationDescription?: string;
    hazardType?: string;
    hazardTypeOtherDescription?: string;

    testResult?: BackflowTestResult;
    jobNumber?: string;
    reasonForTest?: BackflowReasonForTest;
    replacementAssembly?: string;
    properlyInstalled?: boolean;
    nonPotable?: boolean;

    gaugeManufacturer?: string;
    gaugeModel?: string;
    gaugeSerialNumber?: string;
    gaugeLastCalibrationDate?: string;
    gaugeNonPotable?: boolean;

    initialTestDate?: string;

    initCV1HeldPSID?: number;
    initCV1ClosedTight?: boolean;
    initCV1Leaked?: boolean;
    initCV2HeldPSID?: number;
    initCV2ClosedTight?: boolean;
    initCV2Leaked?: boolean;
    initRVOpenedPSID?: number;
    initRVDidNotOpen?: boolean;
    initBCHeldPSID?: number;
    initBCClosedTight?: boolean;
    initBCLeaked?: boolean;
    initPvbAirInletOpenedPSID?: number;
    initPvbAirInletDidNotOpen?: boolean;
    initPvbAirInletFullyOpened?: boolean;
    initPvbCVHeldPSID?: number;
    initPvbCVLeaked?: boolean;

    initCV1HeldPSID2?: number;
    initCV1ClosedTight2?: boolean;
    initCV1Leaked2?: boolean;
    initCV2HeldPSID2?: number;
    initCV2ClosedTight2?: boolean;
    initCV2Leaked2?: boolean;
    initRVOpenedPSID2?: number;
    initRVDidNotOpen2?: boolean;

    repairCV1?: string;
    repairCV2?: string;
    repairRV?: string;
    repairBC?: string;
    repairCV12?: string;
    repairCV22?: string;
    repairRV2?: string;
    repairPvbAirInlet?: string;
    repairPvbCV?: string;

    repairCV1Details?: string;
    repairCV2Details?: string;
    repairRVDetails?: string;
    repairBCDetails?: string;
    repairCV1Details2?: string;
    repairCV2Details2?: string;
    repairRVDetails2?: string;
    repairPvbAirInletDetails?: string;
    repairPvbCVDetails?: string;

    repairTestDate?: string;

    finalCV1HeldPSID?: number;
    finalCV1ClosedTight?: boolean;
    finalCV2HeldPSID?: number;
    finalCV2ClosedTight?: boolean;
    finalRVOpenedPSID?: number;
    finalBCHeldPSID?: number;
    finalBCClosedTight?: boolean;
    finalPvbAirInletOpenedPSID?: number;
    finalPvbAirInletFullyOpened?: boolean;
    finalPvbCVHeldPSID?: number;

    finalCV1HeldPSID2?: number;
    finalCV1ClosedTight2?: boolean;
    finalCV2HeldPSID2?: number;
    finalCV2ClosedTight2?: boolean;
    finalRVOpenedPSID2?: number;

    meterNumber?: string;
    meterRegisters?: boolean;
    meterReadingBefore?: number;
    meterReadingAfter?: number;

    airGapValid?: boolean;

    ossf?: boolean;
    rainFreezeSensorInstalled?: boolean;
    rainFreezeSensorWorkingProperly?: boolean;
    permitNumber?: string;

    comments?: string;

    assemblyImageUrl?: string;
    serialNumberImageUrl?: string;
    bypassAssemblyImageUrl?: string;
    bypassSerialNumberImageUrl?: string;
    airGapImageUrl?: string;

    showRainSensor?: boolean;
    showOSSF?: boolean;
    showPermitNumber?: boolean;
}
