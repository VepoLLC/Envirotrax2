import { CsiImpendingType, CsiNonCompliantType, CsiPastDueType } from './csi-settings-enums';

export interface CsiSettings {
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

    impendingLettersBackgroundColor?: string;
    impendingLettersForegroundColor?: string;
    impendingLettersBorderColor?: string;
    pastDueLettersBackgroundColor?: string;
    pastDueLettersForegroundColor?: string;
    pastDueLettersBorderColor?: string;
    nonCompliantLettersBackgroundColor?: string;
    nonCompliantLettersForegroundColor?: string;
    nonCompliantLettersBorderColor?: string;
}
