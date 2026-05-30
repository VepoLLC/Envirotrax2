import { PageInfo } from "./page-info";
import { Query } from "./query";

export interface DownloadConfig {
    fileName?: string;
    columns: DownloadColumn[];
    endpoint: DownloadEndpoint;
    suppoertedFormats?: FileFormat[]
    selectedFormat?: FileFormat;
}

export type FileFormat = 'CSV' | 'Excel' | 'PDF';

export interface DownloadColumn {
    field: string;
    caption?: string;
}

export interface DownloadEndpoint {
    method: 'GET' | 'POST' | 'PUT' | 'DELETE',
    url: string;
    pageInfo?: PageInfo;
    query?: Query;
}