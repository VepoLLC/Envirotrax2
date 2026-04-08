
import { BackflowExpiredType, BackflowExpiringType, BackflowNonCompliantType, BackflowOutOfServiceType, BackflowTestingMethodType } from "./backflow-testing-settings-enum";

export interface BackflowSettings {
    id?: number;

    testingMethod?: BackflowTestingMethodType;
    gracePeriodDays?: number | null;
    adjustBackflowCreepingDates?: boolean;
    newInstallationsRequireApproval?: boolean;
    replacementsRequireApproval?: boolean;
    detectorAssembliesRequireMeterReading?: boolean;
    outOfServiceRequiresApproval?: boolean;
    outOfServiceType: BackflowOutOfServiceType;   
    requireBackflowTestImages?: boolean;

    expiringNotice1?: BackflowExpiringType;
    expiringNotice2?: BackflowExpiringType;
    expiredNotice1?: BackflowExpiredType;
    expiredNotice2?: BackflowExpiredType;
    backflowNonCompliant1?: BackflowNonCompliantType;
    backflowNonCompliant2?: BackflowNonCompliantType;

    showWaterMeterNumber?: boolean;
    showRainSensor?: boolean;
    showOSSF?: boolean;
    showPermitNumber?: boolean;

    expiringLettersBackgroundColor?: string;
    expiringLettersForegroundColor?: string;
    expiringLettersBorderColor?: string;
    expiredLettersBackgroundColor?: string;
    expiredLettersForegroundColor?: string;
    expiredLettersBorderColor?: string;
    nonCompliantLettersBackgroundColor?: string;
    nonCompliantLettersForegroundColor?: string;
    nonCompliantLettersBorderColor?: string;
}
