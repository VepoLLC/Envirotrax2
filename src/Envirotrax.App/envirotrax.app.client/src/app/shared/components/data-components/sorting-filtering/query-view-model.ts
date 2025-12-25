import { Query } from "../../../models/query";

export interface QueryViewModel {
    query: Query;
    columns: QueryColumn[];
    wasReset?: boolean;
}

export interface QueryColumn {
    field: string;
    caption: string;
    type: ColumnType;
    options?: QueryColumnOption[];
    tag?: QueryColumnTag
}

export enum ColumnType {
    text = 'text',
    number = 'number',
    date = 'date',
    other = 'other'
}

export interface QueryColumnOption {
    id: string;
    value: string;
}

export interface QueryColumnTag {
    location?: QueryPropertyLocation;
    specialColumn?: SpecialColumnType;
}

export enum SpecialColumnType {
    ActionsColumn
}

export enum QueryPropertyLocation {
    AdvancedSearch,
    FilterPanel,
    SearchBar
}