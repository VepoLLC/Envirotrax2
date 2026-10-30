import { Query } from "./query";

export enum TableSettingType {
    BackflowTests = 0
}

export interface TableSetting {
    id?: number;
    tableType?: TableSettingType;
    name?: string;
    // A TableSettingFilters object as JSON.
    filters?: string;
}

// What a search template saves: the table's query, plus any extra parameters a table needs to repeat its search.
export interface TableSettingFilters {
    query: Query;
}
