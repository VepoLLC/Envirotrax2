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

export enum BackflowTestingMethodType {
    USC = 0,
    ASSE = 1,
    TREEO = 2
}

export enum BackflowOutOfServiceType {
    VepoManaged = 0,
    WaterSupplierManaged = 1
}