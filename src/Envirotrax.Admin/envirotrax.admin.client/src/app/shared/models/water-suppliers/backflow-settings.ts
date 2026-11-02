import { LetterImageFitType } from './letter-image-fit-type';

export enum BackflowTestingMethodType {
    USC = 0,
    ASSE = 1,
    TREEO = 2
}

export enum BackflowOutOfServiceType {
    VepoManaged = 0,
    WaterSupplierManaged = 1
}

export enum BackflowExpiringType {
    Off = 0,
    TwoMonths = 1,
    NextMonth = 2,
    ThisMonth = 3
}

export enum BackflowExpiredType {
    Off = 0,
    LastMonth = 1,
    TwoMonthsAgo = 2,
    ThreeMonthsAgo = 3,
    ThisMonth = 4
}

export enum BackflowNonCompliantType {
    Off = 0,
    ThirtyPlusDays = 1,
    SixtyPlusDays = 2,
    NinetyPlusDays = 3,
    OneTwentyPlusDays = 4,
    OneFiftyPlusDays = 5,
    OneEightyPlusDays = 6,
    PastDueLastMonth = 7,
    ExpiredTwoMonthsAgo = 8
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

    expiringNotice1?: BackflowExpiringType;
    expiringNotice2?: BackflowExpiringType;
    expiredNotice1?: BackflowExpiredType;
    expiredNotice2?: BackflowExpiredType;
    backflowNonCompliant1?: BackflowNonCompliantType;
    backflowNonCompliant2?: BackflowNonCompliantType;
    nonCompliantPrint1?: boolean;
    nonCompliantPrint2?: boolean;
    unknownAssembliesNotice?: boolean;

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
    unknownAssembliesLettersBackgroundColor?: string;
    unknownAssembliesLettersForegroundColor?: string;
    unknownAssembliesLettersBorderColor?: string;

    noticeBodyFont?: string | null;
    noticeBodyFontSize?: number | null;
    noticeImageFitType?: LetterImageFitType;
    noticeImageFitSize?: number;
    expiringTitle?: string | null;
    expiringMessage?: string | null;
    expiredTitle?: string | null;
    expiredMessage?: string | null;
    nonCompliantTitle?: string | null;
    nonCompliantMessage?: string | null;
    unknownAssembliesTitle?: string | null;
    unknownAssembliesMessage?: string | null;
}
