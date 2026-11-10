import { TableSettingFilters } from "@envirotrax/common-ui";

// What a backflow test search template saves. The payment status is sent as its own parameter instead of in
// the query filter, so the template keeps it next to the query.
export interface BackflowTestSearchFilters extends TableSettingFilters {
    paymentStatus?: string;
}
