import { Site } from "./site";
import { SiteLogType } from "./site-log-type.enum";
import { SiteLogReviewDateStatus } from "./site-log-review-date-status.enum";

export interface PropertyLogAssembly {
    id?: number;
    serialNumber?: string;
    manufacturer?: string;
    model?: string;
    size?: string;
    deviceType?: string;
}

export interface PropertyLogCreatedBy {
    id?: number;
    email?: string;
}

export interface PropertyLog {
    id?: number;
    site?: Site;
    logType?: SiteLogType;
    noteText?: string;
    reviewDate?: string | null;
    reviewDateStatus?: SiteLogReviewDateStatus;
    assemblyId?: number | null;
    assembly?: PropertyLogAssembly | null;
    fileAttachmentName?: string | null;
    skipFile?: boolean;
    createdById?: number | null;
    createdTime?: string;
    createdBy?: PropertyLogCreatedBy;
}
