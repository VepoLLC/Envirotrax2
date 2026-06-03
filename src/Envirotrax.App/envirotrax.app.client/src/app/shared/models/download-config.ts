import { PageInfo } from "./page-info";
import { Query } from "./query";

export interface DownloadConfig {
    fileName?: string;
    columnCategories?: ColumnCategory[];
    columns: DownloadColumn[];
    endpoint: DownloadEndpoint;
    pdfEndpoint?: DownloadEndpoint;
    suppoertedFormats?: FileFormat[]
    selectedFormat?: FileFormat;
}

export type FileFormat = 'CSV' | 'Excel' | 'PDF' | 'XML';

export interface ColumnCategory {
    field?: string;
    caption?: string;
    isSelected: boolean
}

export interface DownloadColumn {
    field: string;
    caption?: string;
    category?: string;
}

export interface DownloadEndpoint {
    method: 'GET' | 'POST' | 'PUT' | 'DELETE',
    url: string;
    pageInfo?: PageInfo;
    query?: Query;
}