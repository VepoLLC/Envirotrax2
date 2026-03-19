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
