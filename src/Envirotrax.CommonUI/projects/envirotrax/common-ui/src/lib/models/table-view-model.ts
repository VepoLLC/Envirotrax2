import { FreeTextSearchSettings, TableColumn, TableCustomAction } from "../components/data-components/table/table.component";
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