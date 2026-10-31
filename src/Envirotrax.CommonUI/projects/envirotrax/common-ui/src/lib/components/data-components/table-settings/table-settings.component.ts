import { Component, OnInit } from "@angular/core";
import { ModalReference, ModalSize } from "@developer-partners/ngx-modal-dialog";
import { TableSetting, TableSettingFilters, TableSettingType } from "../../../models/table-setting";
import { TableSettingService } from "../../../services/table-setting/table-setting.service";
import { ModalHelperService } from "../../../services/helpers/modal-helper.service";
import { ToastService } from "../../../services/toast/toast.service";
import { TableColumn } from "../table/table-models";
import { ColumnType } from "../sorting-filtering/query-view-model";
import { EditTableSettingComponent, EditTableSettingConfig } from "./edit-table-setting.component";

export interface TableSettingsConfig {
    tableType: TableSettingType;
    currentFilters: TableSettingFilters;
}

// Lists the logged-in user's search templates of one table. Selecting one closes the dialog with its filters.
@Component({
    templateUrl: './table-settings.component.html',
    standalone: false
})
export class TableSettingsComponent implements OnInit {
    public isLoading: boolean = false;
    public settings?: TableSetting[];
    public canAdd?: boolean;

    public readonly columns: TableColumn<TableSetting>[] = [
        {
            field: 'name',
            caption: 'Template Name',
            type: ColumnType.text
        }
    ];

    private readonly _config: TableSettingsConfig;

    constructor(
        private readonly _modalReference: ModalReference<TableSettingsConfig, TableSettingFilters>,
        private readonly _tableSettingService: TableSettingService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) {
        this._config = _modalReference.config.model!;
    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;

            const [settings, canAdd] = await Promise.all([
                this._tableSettingService.getAllMy(this._config.tableType),
                this._tableSettingService.canAddMy()
            ]);

            this.settings = settings;
            this.canAdd = canAdd;
        } finally {
            this.isLoading = false;
        }
    }

    private async getSettings(): Promise<void> {
        try {
            this.isLoading = true;

            this.settings = await this._tableSettingService.getAllMy(this._config.tableType);
        } finally {
            this.isLoading = false;
        }
    }

    public select(setting: TableSetting): void {
        const filters: TableSettingFilters = setting.filters
            ? JSON.parse(setting.filters)
            : { query: {} };

        this._modalReference.closeSuccess(filters);
    }

    public add(): void {
        this.showEditModal('Create New Search Template', {
            tableType: this._config.tableType
        });
    }

    public edit(setting: TableSetting): void {
        this.showEditModal('Edit Search Template', { ...setting });
    }

    private showEditModal(title: string, setting: TableSetting): void {
        this._modalHelper.show<EditTableSettingConfig, TableSetting>(EditTableSettingComponent, {
            title: title,
            size: ModalSize.large,
            model: {
                setting: setting,
                currentFilters: this._config.currentFilters
            }
        }).result().subscribe(() => this.getSettings());
    }

    public delete(setting: TableSetting): void {
        this._modalHelper.confirm({
            messages: [`Are you sure that you want to delete the search template "${setting.name}"?`]
        }).result().subscribe(async () => {
            try {
                this.isLoading = true;

                await this._tableSettingService.delete(setting.id!);
                this._toastService.successFullyDeleted('Search Template');

                await this.getSettings();
            } finally {
                this.isLoading = false;
            }
        });
    }
}
