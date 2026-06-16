import { TemplateRef, Type } from "@angular/core";
import { QueryColumn } from "../sorting-filtering/query-view-model";
import { ComparisonOperator } from "../../../models/query";

export interface TableColumn<T> extends QueryColumn {
    isTableColumnExcluded?: boolean;
    queryColumnExcluded?: boolean;
    isInvisible?: boolean;
    cellTemplate?: TemplateRef<CellTemplateData<T>>;
    cellComponent?: Type<any>;
    headerCssClass?: string;
    rowCssClass?: string;
    isDownloadExcluded?: boolean;
}

export interface CellTemplateData<T> {
    rowData?: T;
    column?: TableColumn<T>;
}

export interface TableCustomAction<T> {
    text: string;
    iconClass?: string;
    action: (record: T) => Promise<void> | void;
    hideAction?: (row: T) => boolean;
}

export enum ColumnSize {
    OneUnit = 'col-md-1',
    TwoUnits = 'col-md-2',
    ThreeUnits = 'col-md-3',
    FourUnit = 'col-md-4',
    FiveUnits = 'col-md-5',
    SixUnits = 'col-md-6',
    SevenUnit = 'col-md-7',
    EightUnits = 'col-md-8',
    NineUnits = 'col-md-9',
    TenUnit = 'col-md-10',
    ElevenUnits = 'col-md-11',
    TwelveUnits = 'col-md-12'
}

export interface FreeTextSearchSettings {
    placeholder?: string;
    text?: string;
    searchQuery: FreeTextQuery[];
}

export interface FreeTextQuery {
    field?: string;
    operator?: ComparisonOperator;
    logicalOperator?: 'And' | 'Or';
    multiWordSearch?: boolean;
    placeholder?: string;
}