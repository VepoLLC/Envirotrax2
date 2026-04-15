export enum BackflowTestResult {
    Pass = 0,
    Fail = 1,
    PassAfterRepairs = 2
}

export enum BackflowReasonForTest {
    AnnualTest = 0,
    NewInstallation = 1,
    Relocation = 2,
    Replacement = 3,
    Repair = 4,
    AnnualTestAfterRepairs = 5
}
