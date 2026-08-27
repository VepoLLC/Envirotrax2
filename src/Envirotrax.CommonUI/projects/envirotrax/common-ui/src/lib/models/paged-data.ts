import { PageInfo } from "./page-info";

export interface PagedData<T> {
    pageInfo: PageInfo;
    data: T[];
}