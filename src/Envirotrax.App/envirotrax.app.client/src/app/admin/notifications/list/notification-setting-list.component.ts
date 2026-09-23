import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { CellTemplateData, ColumnType, ModalHelperService, TableColumn, ToastService } from "@envirotrax/common-ui";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { NotificationSetting } from "../../../shared/models/notifications/notification-setting";
import { NotificationSettingService } from "../../../shared/services/notifications/notification-setting.service";
import { NotificationOptionsService } from "../../../shared/services/notifications/notification-options.service";
import { AuthService } from "../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../shared/models/permission-type";
import { NotificationInterval } from "../../../shared/enums/notification-interval.enum";
import { NotificationDeliveryType } from "../../../shared/enums/notification-delivery-type.enum";
import { EditNotificationSettingComponent } from "../edit/edit-notification-setting.component";
import { AppContainerHelperService } from "../../../shared/services/helpers/app-contaner-helper.service";

@Component({
    standalone: false,
    templateUrl: './notification-setting-list.component.html'
})
export class NotificationSettingListComponent implements OnInit {
    public table: TableViewModel<NotificationSettingRow> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        }
    };

    public canModify: boolean = false;

    @ViewChild('descriptionCell', { static: true })
    private descriptionCellTemplate!: TemplateRef<CellTemplateData<NotificationSettingRow>>;

    @ViewChild('propertyTypeCell', { static: true })
    private propertyTypeCellTemplate!: TemplateRef<CellTemplateData<NotificationSettingRow>>;

    @ViewChild('filtersCell', { static: true })
    private filtersCellTemplate!: TemplateRef<CellTemplateData<NotificationSettingRow>>;

    @ViewChild('hazardTypesCell', { static: true })
    private hazardTypesCellTemplate!: TemplateRef<CellTemplateData<NotificationSettingRow>>;

    @ViewChild('intervalCell', { static: true })
    private intervalCellTemplate!: TemplateRef<CellTemplateData<NotificationSettingRow>>;

    constructor(
        private readonly _service: NotificationSettingService,
        private readonly _options: NotificationOptionsService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _authService: AuthService,
        private readonly _containerHelper: AppContainerHelperService
    ) { }

    public async ngOnInit(): Promise<void> {
        this._containerHelper.setContainerVisibility(false);

        this.canModify = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.Notifications);

        this.table.columns = this.getColumns();

        await this.getSettings();
    }

    private getColumns(): TableColumn<NotificationSettingRow>[] {
        return [
            {
                field: 'description',
                caption: 'Description/Account',
                cellTemplate: this.descriptionCellTemplate,
                type: ColumnType.other,
                queryColumnExcluded: true,
                rowCssClass: 'align-top'
            },
            {
                field: 'reasonForTest',
                caption: 'Backflow Test Type',
                type: ColumnType.text,
                queryColumnExcluded: true,
                rowCssClass: 'align-top'
            },
            {
                field: 'propertyType',
                caption: 'Property Type',
                cellTemplate: this.propertyTypeCellTemplate,
                type: ColumnType.other,
                queryColumnExcluded: true,
                rowCssClass: 'align-top'
            },
            {
                field: 'filters',
                caption: 'Notification Filters',
                cellTemplate: this.filtersCellTemplate,
                type: ColumnType.other,
                queryColumnExcluded: true,
                rowCssClass: 'align-top'
            },
            {
                field: 'hazardTypes',
                caption: 'Hazard Types',
                cellTemplate: this.hazardTypesCellTemplate,
                type: ColumnType.other,
                queryColumnExcluded: true,
                rowCssClass: 'align-top'
            },
            {
                field: 'interval',
                caption: 'Interval/Type',
                cellTemplate: this.intervalCellTemplate,
                type: ColumnType.other,
                queryColumnExcluded: true,
                rowCssClass: 'align-top'
            }
        ];
    }

    private createRow(setting: NotificationSetting): NotificationSettingRow {
        return {
            setting: setting,
            account: setting.user?.emailAddress ?? '',
            reasonForTest: this._options.getReasonForTestText(setting.reasonForTest),
            interval: this._options.getIntervalText(setting.interval),
            deliveryType: this._options.getDeliveryTypeText(setting.deliveryType)
        };
    }

    public async getSettings(): Promise<void> {
        try {
            this.table.isLoading = true;

            const settings = await this._service.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );

            this.table.items = {
                pageInfo: settings.pageInfo,
                data: settings.data.map(setting => this.createRow(setting))
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    public add(): void {
        const newSetting: NotificationSetting = {
            color: '#ffffff',
            reasonForTest: null,
            propertyTypeAny: true,
            filterAny: true,
            filterSubmissionDaysExceededDays: 0,
            hazardTypeAny: true,
            interval: NotificationInterval.Immediate,
            deliveryType: NotificationDeliveryType.Email
        };

        this.showEditModal('Create New Notification', newSetting);
    }

    public edit(row: NotificationSettingRow): void {
        this.showEditModal('Edit Notification', { id: row.setting.id });
    }

    private showEditModal(title: string, setting: NotificationSetting): void {
        this._modalHelper.show<NotificationSetting>(EditNotificationSettingComponent, {
            title: title,
            model: setting,
            size: ModalSize.extraLarge
        }).result().subscribe(() => this.getSettings());
    }

    public delete(row: NotificationSettingRow): void {
        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(async () => {
                try {
                    this.table.isLoading = true;

                    await this._service.delete(row.setting.id!);
                    this._toastService.successFullyDeleted('Notification Setting');

                    await this.getSettings();
                } finally {
                    this.table.isLoading = false;
                }
            });
    }
}

interface NotificationSettingRow {
    setting: NotificationSetting;
    account: string;
    reasonForTest: string;
    interval: string;
    deliveryType: string;
}
