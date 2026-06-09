import { FreeTextSearchSettings, TableColumn, TableCustomAction } from "@envirotrax/common-ui";
import { PagedData } from "./paged-data";
import { Query } from "./query";

export interface TableViewModel<T> {
    items?: PagedData<T>;
    query: Query;
    columns?: TableColumn<T>[];
    customActions?: TableCustomAction<T>[];
    freeTextSearch?: FreeTextSearchSettings;
    isLoading?: boolean;
}