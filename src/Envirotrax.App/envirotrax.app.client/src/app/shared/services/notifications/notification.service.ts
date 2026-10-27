import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { Notification } from "../../models/notifications/notification";
import { PagedData } from "../../models/paged-data";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { DownloadEndpoint } from "../../models/download-config";

@Injectable({
    providedIn: 'root'
})
export class NotificationService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public getEndpoint(): DownloadEndpoint {
        return {
            method: 'GET',
            url: this._urlResolver.resolveUrl('/api/notifications')
        };
    }

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<Notification>> {
        const url = this._urlResolver.resolveUrl('/api/notifications');

        const observable = this._http.get<PagedData<Notification>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return lastValueFrom(observable);
    }

    public setHidden(id: number, hidden: boolean): Promise<Notification> {
        const url = this._urlResolver.resolveUrl(`/api/notifications/${id}/hidden`);
        return lastValueFrom(this._http.put<Notification>(url, { hidden }));
    }
}
