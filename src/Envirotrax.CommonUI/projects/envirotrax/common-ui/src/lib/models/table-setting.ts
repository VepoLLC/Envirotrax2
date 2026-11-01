import { Query } from "./query";

export interface TableSetting {
    id?: number;
    // A value of the hosting app's table setting type enum.
    tableType?: number;
    name?: string;
    // A TableSettingFilters object as JSON.
    filters?: string;
}

// What a search template saves: the table's query, plus any extra parameters a table needs to repeat its search.
export interface TableSettingFilters {
    query: Query;
}

/**
 * Where the Search Templates dialog loads and saves table settings. CommonUI calls no endpoint itself: each app
 * implements this against its own API, e.g. the App client for water supplier users.
 */
export interface TableSettingDataSource {
    getAll(tableType: number): Promise<TableSetting[]>;

    // Whether the logged-in user can save table settings here.
    canAdd(): Promise<boolean>;

    add(setting: TableSetting): Promise<TableSetting>;
    update(setting: TableSetting): Promise<TableSetting>;
    delete(id: number): Promise<TableSetting>;
}
