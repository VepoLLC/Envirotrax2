export class LetterCounts {
    expiringNotice1: number = 0;
    expiringNotice2: number = 0;
    expiredNotice1: number = 0;
    expiredNotice2: number = 0;
    backflowNonCompliant1: number = 0;
    backflowNonCompliant2: number = 0;
    unknownAssembliesNotice: number = 0;

    impendingNotice1: number = 0;
    impendingNotice2: number = 0;
    pastDueNotice1: number = 0;
    pastDueNotice2: number = 0;
    csiNonCompliant1: number = 0;
    csiNonCompliant2: number = 0;
}

export class GeneratedLetters {
    counts: LetterCounts = new LetterCounts();
    emailCount: number = 0;
    fileName?: string | null;
    fileContent?: string | null;
}
