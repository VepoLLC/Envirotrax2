export interface CsiSettings {
    id?: number;

  modificationGracePeriodDays?: number | null;

    newlyCreatedBackflowTestExpirationDays?: number;
    requireInspectionImages?: boolean;

    impendingNotice1?: number;
    impendingNotice2?: number;

    pastDueNotice1?: number;
    pastDueNotice2?: number;

    nonCompliant1?: number;
    nonCompliant2?: number;

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
