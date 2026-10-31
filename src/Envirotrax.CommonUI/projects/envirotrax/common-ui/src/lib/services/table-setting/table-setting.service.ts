import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { lastValueFrom, Observable } from "rxjs";
import { TableSetting, TableSettingFilters, TableSettingType } from "../../models/table-setting";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { ModalHelperService } from "../helpers/modal-helper.service";
import { TableSettingsComponent, TableSettingsConfig } from "../../components/data-components/table-settings/table-settings.component";

@Injectable({
    providedIn: 'root'
})
export class TableSettingService {
    constructor(
        private readonly _http: HttpClient,
        private readonly _urlResolver: UrlResolverService,
        private readonly _modalHelper: ModalHelperService
    ) {

    }

    public getAllMy(tableType: TableSettingType): Promise<TableSetting[]> {
        const url = this._urlResolver.resolveUrl('/api/table-settings');
        const params = new HttpParams().append('tableType', tableType);

        return lastValueFrom(this._http.get<TableSetting[]>(url, { params }));
    }

    // Whether the logged-in user can save table settings in the current water supplier account.
    public canAddMy(): Promise<boolean> {
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

    /**
     * Opens the Search Templates dialog of a table. New and updated templates save currentFilters, and the
     * returned observable emits the filters of the template the user selects.
     */
    public showSearchTemplates<TFilters extends TableSettingFilters>(tableType: TableSettingType, currentFilters: TFilters): Observable<TFilters> {
        return this._modalHelper.show<TableSettingsConfig, TFilters>(TableSettingsComponent, {
            title: 'Search Templates',
            size: ModalSize.large,
            model: {
                tableType: tableType,
                currentFilters: currentFilters
            }
        }).result();
    }
}
