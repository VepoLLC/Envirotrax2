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
