import { LetterImageFitType } from './letter-image-fit-type';

export enum CsiImpendingType {
    Off = 0,
    TwoMonths = 1,
    NextMonth = 2,
    ThisMonth = 3
}

export enum CsiPastDueType {
    Off = 0,
    LastMonth = 1,
    TwoMonthsAgo = 2,
    ThreeMonthsAgo = 3,
    ThisMonth = 4
}

export enum CsiNonCompliantType {
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

export class CsiSettings {
    id?: number;

    modificationGracePeriodDays?: number | null;
    newlyCreatedBackflowTestExpirationDays?: number;
    requireInspectionImages?: boolean;

    impendingNotice1?: CsiImpendingType;
    impendingNotice2?: CsiImpendingType;
    pastDueNotice1?: CsiPastDueType;
    pastDueNotice2?: CsiPastDueType;
    nonCompliant1?: CsiNonCompliantType;
    nonCompliant2?: CsiNonCompliantType;
    nonCompliantPrint1?: boolean;
    nonCompliantPrint2?: boolean;

    impendingLettersBackgroundColor?: string;
    impendingLettersForegroundColor?: string;
    impendingLettersBorderColor?: string;
    pastDueLettersBackgroundColor?: string;
    pastDueLettersForegroundColor?: string;
    pastDueLettersBorderColor?: string;
    nonCompliantLettersBackgroundColor?: string;
    nonCompliantLettersForegroundColor?: string;
    nonCompliantLettersBorderColor?: string;

    noticeBodyFont?: string | null;
    noticeBodyFontSize?: number | null;
    noticeImageFitType?: LetterImageFitType;
    noticeImageFitSize?: number;
    impendingTitle?: string | null;
    impendingMessage?: string | null;
    pastDueTitle?: string | null;
    pastDueMessage?: string | null;
    nonCompliantTitle?: string | null;
    nonCompliantMessage?: string | null;
}
