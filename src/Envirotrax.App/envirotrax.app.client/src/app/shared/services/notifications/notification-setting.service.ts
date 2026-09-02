import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { NotificationSetting } from "../../models/notifications/notification-setting";
import { PagedData } from "../../models/paged-data";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";

@Injectable({
    providedIn: 'root'
})
export class NotificationSettingService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<NotificationSetting>> {
        const url = this._urlResolver.resolveUrl('/api/notification-settings');

        const observable = this._http.get<PagedData<NotificationSetting>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return lastValueFrom(observable);
    }

    public add(setting: NotificationSetting): Promise<NotificationSetting> {
        const url = this._urlResolver.resolveUrl('/api/notification-settings');
        return lastValueFrom(this._http.post<NotificationSetting>(url, setting));
    }

    public update(setting: NotificationSetting): Promise<NotificationSetting> {
        const url = this._urlResolver.resolveUrl(`/api/notification-settings/${setting.id}`);
        return lastValueFrom(this._http.put<NotificationSetting>(url, setting));
    }

    public delete(id: number): Promise<NotificationSetting> {
        const url = this._urlResolver.resolveUrl(`/api/notification-settings/${id}`);
        return lastValueFrom(this._http.delete<NotificationSetting>(url));
    }
}
