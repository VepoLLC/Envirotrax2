export enum SiteLogType {
    Note = 0,
    Reminder = 1,
    CompletedReminder = 2
}

export interface SiteLogAssembly {
    id?: number | null;
    serialNumber?: string;
    manufacturer?: string;
    model?: string;
    size?: string;
    deviceType?: string;
}

export interface SiteLog {
    id?: number;
    logType?: SiteLogType;
    noteText?: string | null;
    reviewDate?: string | null;
    assembly?: SiteLogAssembly | null;
    skipFile?: boolean;
    createdTime?: string;
}

export interface SiteLogRow extends SiteLog {
    assemblyDescription: string;
}

export interface SiteLogEditModel {
    siteId: number;
    waterSupplierId: number;
    id?: number;
}
