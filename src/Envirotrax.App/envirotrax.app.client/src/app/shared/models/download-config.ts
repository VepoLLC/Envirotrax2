import { PageInfo } from "./page-info";
import { Query } from "./query";

export interface DownloadConfig<T extends string = string> {
    fileName?: string;
    categories?: ColumnCategory<T>[];
    columns: DownloadColumn<T>[];
    endpoint: DownloadEndpoint;
    pdfEndpoint?: DownloadEndpoint;
    suppoertedFormats?: FileFormat[]
    selectedFormat?: FileFormat;
    csvDelimiter?: CsvDelimiter;
}

export type CsvDelimiter = ',' | '|' | 'tab';

export type FileFormat = 'CSV' | 'Excel' | 'PDF' | 'XML';

export interface ColumnCategory<T extends string = string> {
    name?: T;
    caption?: string;
    isSelected: boolean;
}

export interface DownloadColumn<T extends string = string> {
    field: string;
    caption?: string;
    category?: T;
}

export interface DownloadEndpoint {
    method: 'GET' | 'POST' | 'PUT' | 'DELETE',
    url: string;
    pageInfo?: PageInfo;
    query?: Query;
}