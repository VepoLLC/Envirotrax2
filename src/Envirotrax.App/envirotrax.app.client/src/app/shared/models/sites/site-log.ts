import { Site } from './site';
import { SiteLogType } from './site-log-type.enum';
import { SiteLogReviewDateStatus } from './site-log-review-date-status.enum';

export interface SiteLogAssembly {
    id?: number;
    serialNumber?: string;
    manufacturer?: string;
    model?: string;
    size?: string;
    deviceType?: string;
}

export interface SiteLogCreatedBy {
    id?: number;
    email?: string;
}

export interface SiteLog {
    id?: number;
    site?: Site;
    logType?: SiteLogType;
    noteText?: string;
    reviewDate?: string | null;
    assemblyId?: number | null;
    assembly?: SiteLogAssembly | null;
    fileAttachmentName?: string | null;
    fileAttachmentPath?: string | null;
    url?: string | null;
    skipFile?: boolean;
    reviewDateStatus?: SiteLogReviewDateStatus;
    createdTime?: string;
    createdBy?: SiteLogCreatedBy;
}
