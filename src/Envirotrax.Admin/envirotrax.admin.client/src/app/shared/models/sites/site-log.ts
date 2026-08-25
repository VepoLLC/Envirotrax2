export enum SiteLogType {
    Note = 0,
    Reminder = 1,
    CompletedReminder = 2
}

export const siteLogTypeLabels: Record<SiteLogType, string> = {
    [SiteLogType.Note]: 'Note',
    [SiteLogType.Reminder]: 'Reminder',
    [SiteLogType.CompletedReminder]: 'Completed Reminder'
};

export class SiteLog {
    id?: number;
    logType?: SiteLogType;
    noteText?: string;
    reviewDate?: string;
    assemblyId?: number;
    fileAttachmentName?: string;
    createdTime?: string;
    createdBy?: { id?: number; email?: string };
}
