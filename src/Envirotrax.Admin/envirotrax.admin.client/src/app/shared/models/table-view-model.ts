import { PagedData, Query, TableColumn, TableCustomAction, FreeTextSearchSettings } from "@envirotrax/common-ui";

export interface TableViewModel<T> {
    items?: PagedData<T>;
    query: Query;
    columns?: TableColumn<T>[];
    customActions?: TableCustomAction<T>[];
    freeTextSearch?: FreeTextSearchSettings;
    isLoading?: boolean;
}