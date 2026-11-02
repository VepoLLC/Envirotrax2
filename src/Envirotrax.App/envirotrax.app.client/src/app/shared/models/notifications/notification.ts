import { NotificationSetting } from "./notification-setting";
import { State } from "../lookup/state";

export interface Notification extends NotificationSetting {
    moduleType?: number;
    recordId?: number;
    test?: NotificationTest | null;

    propertyDescription?: string | null;
    recordDescription?: string | null;

    hidden?: boolean;

    sentTime?: Date | null;
    createdTime?: Date;
}

export interface NotificationTest {
    id: number;
    siteId?: number | null;

    propertyBusinessName?: string | null;
    propertyType: number;
    propertyStreetNumber?: string | null;
    propertyStreetName?: string | null;
    propertyNumber?: string | null;
    propertyCity?: string | null;
    propertyState?: State | null;
    propertyZip?: string | null;

    manufacturer?: string | null;
    model?: string | null;
    size?: string | null;
    deviceType?: string | null;
    serialNumber?: string | null;

    testDate?: Date | null;
    renewalRequired: boolean;
    expirationDate?: Date | null;
    outOfService: boolean;
}
