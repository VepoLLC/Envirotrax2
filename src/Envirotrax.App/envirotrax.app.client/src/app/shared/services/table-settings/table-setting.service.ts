import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { TableSetting, TableSettingDataSource } from "@envirotrax/common-ui";
import { UrlResolverService } from "../helpers/url-resolver.service";

// The logged-in water supplier user's table settings, used by the CommonUI Search Templates dialog.
@Injectable({
    providedIn: 'root'
})
export class TableSettingService implements TableSettingDataSource {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    public getAll(tableType: number): Promise<TableSetting[]> {
        const url = this._urlResolver.resolveUrl('/api/table-settings');
        const params = new HttpParams().append('tableType', tableType);

        return lastValueFrom(this._http.get<TableSetting[]>(url, { params }));
    }

    public canAdd(): Promise<boolean> {
        const url = this._urlResolver.resolveUrl('/api/table-settings/availability');
        return lastValueFrom(this._http.get<boolean>(url));
    }

    public add(setting: TableSetting): Promise<TableSetting> {
        const url = this._urlResolver.resolveUrl('/api/table-settings');
        return lastValueFrom(this._http.post<TableSetting>(url, setting));
    }

    public update(setting: TableSetting): Promise<TableSetting> {
        const url = this._urlResolver.resolveUrl(`/api/table-settings/${setting.id}`);
        return lastValueFrom(this._http.put<TableSetting>(url, setting));
    }

    public delete(id: number): Promise<TableSetting> {
        const url = this._urlResolver.resolveUrl(`/api/table-settings/${id}`);
        return lastValueFrom(this._http.delete<TableSetting>(url));
    }
}
