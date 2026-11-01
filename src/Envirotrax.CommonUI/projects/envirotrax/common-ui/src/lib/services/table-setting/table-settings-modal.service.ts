import { Injectable } from "@angular/core";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { Observable } from "rxjs";
import { TableSettingDataSource, TableSettingFilters } from "../../models/table-setting";
import { ModalHelperService } from "../helpers/modal-helper.service";
import { TableSettingsComponent, TableSettingsConfig } from "../../components/data-components/table-settings/table-settings.component";

@Injectable({
    providedIn: 'root'
})
export class TableSettingsModalService {
    constructor(private readonly _modalHelper: ModalHelperService) {

    }

    /**
     * Opens the Search Templates dialog of a table. The dialog loads and saves templates through dataSource, new and
     * updated templates save currentFilters, and the returned observable emits the filters of the template the user selects.
     */
    public showSearchTemplates<TFilters extends TableSettingFilters>(
        dataSource: TableSettingDataSource,
        tableType: number,
        currentFilters: TFilters
    ): Observable<TFilters> {
        return this._modalHelper.show<TableSettingsConfig, TFilters>(TableSettingsComponent, {
            title: 'Search Templates',
            size: ModalSize.large,
            model: {
                dataSource: dataSource,
                tableType: tableType,
                currentFilters: currentFilters
            }
        }).result();
    }
}
