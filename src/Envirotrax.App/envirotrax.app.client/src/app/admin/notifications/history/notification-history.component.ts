import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { formatDate } from "@angular/common";
import { NgForm } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { CellTemplateData, ColumnType, InputOption, ModalHelperService, TableColumn } from "@envirotrax/common-ui";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { Query, QueryProperty } from "../../../shared/models/query";
import { DownloadColumn, DownloadConfig } from "../../../shared/models/download-config";
import { MAX_PAGE_SIZE } from "../../../shared/models/page-info";
import { Notification, NotificationTest } from "../../../shared/models/notifications/notification";
import { NotificationSetting } from "../../../shared/models/notifications/notification-setting";
import { NotificationInterval } from "../../../shared/enums/notification-interval.enum";
import { PropertyType } from "../../../shared/enums/property-type.enum";
import { NotificationService } from "../../../shared/services/notifications/notification.service";
import { NotificationFlagOption, NotificationOptionsService } from "../../../shared/services/notifications/notification-options.service";
import { UserService } from "../../../shared/services/water-suppliers/user.service";
import { BackflowTestOptionsService } from "../../../shared/services/backflow/backflow-test-options.service";
import { DownloadService } from "../../../shared/services/download.service";
import { AuthService } from "../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../shared/models/permission-type";
import { AppContainerHelperService } from "../../../shared/services/helpers/app-contaner-helper.service";
import { NotificationDetailsModalComponent } from "./notification-details-modal.component";

const Locale = 'en-US';
const DisplayDateFormat = 'MM/dd/yyyy';
const QueryDateFormat = 'yyyy-MM-dd';

@Component({
    standalone: false,
    templateUrl: './notification-history.component.html'
})
export class NotificationHistoryComponent implements OnInit {
    public table: TableViewModel<NotificationRow> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        }
    };

    public canModify: boolean = false;
    public header: string = '';

    public filters: NotificationSearchFilters;
    public accountOptions: InputOption[] = [{ id: '', text: 'Any account' }];

    public readonly reasonForTestOptions: InputOption[];
    public readonly intervalOptions: InputOption[];
    public readonly flagGroups: NotificationFlagGroup[];

    public readonly downloadConfig: DownloadConfig;

    @ViewChild('colorCell', { static: true })
    private colorCellTemplate!: TemplateRef<CellTemplateData<NotificationRow>>;

    @ViewChild('dateCell', { static: true })
    private dateCellTemplate!: TemplateRef<CellTemplateData<NotificationRow>>;

    @ViewChild('descriptionCell', { static: true })
    private descriptionCellTemplate!: TemplateRef<CellTemplateData<NotificationRow>>;

    @ViewChild('propertyCell', { static: true })
    private propertyCellTemplate!: TemplateRef<CellTemplateData<NotificationRow>>;

    @ViewChild('assemblyCell', { static: true })
    private assemblyCellTemplate!: TemplateRef<CellTemplateData<NotificationRow>>;

    @ViewChild('actionsCell', { static: true })
    private actionsCellTemplate!: TemplateRef<CellTemplateData<NotificationRow>>;

    private readonly interval: NotificationInterval;
    private headerPrefix: string = '';

    constructor(
        private readonly _service: NotificationService,
        private readonly _options: NotificationOptionsService,
        private readonly _userService: UserService,
        private readonly _downloadService: DownloadService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _authService: AuthService,
        private readonly _containerHelper: AppContainerHelperService,
        private readonly _route: ActivatedRoute,
        backflowOptions: BackflowTestOptionsService
    ) {
        this.interval = this._route.snapshot.data['interval'];

        this.reasonForTestOptions = [...this.toFilterOptions(backflowOptions.submitReasonOptions), { id: '', text: 'Any' }];
        this.intervalOptions = [...this.toFilterOptions(this._options.intervalOptions), { id: '', text: 'Any' }];

        this.flagGroups = [
            { title: 'Property Type', cssClass: 'col-md-4 col-xl-2', anyField: 'propertyTypeAny', options: this._options.propertyTypeOptions },
            { title: 'Notification Filters', cssClass: 'col-md-4 col-xl-3', anyField: 'filterAny', options: this._options.filterOptions },
            { title: 'Hazard Types', cssClass: 'col-md-4 col-xl-3', anyField: 'hazardTypeAny', options: this._options.hazardTypeOptions }
        ];

        this.filters = this.createFilters();

        this.downloadConfig = {
            fileName: 'Notifications',
            endpoint: this._service.getEndpoint(),
            suppoertedFormats: ['CSV', 'Excel'],
            columns: this.buildDownloadColumns()
        };
    }

    public async ngOnInit(): Promise<void> {
        this._containerHelper.setContainerVisibility(false);

        this.canModify = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.Notifications);
        this.table.columns = this.getColumns();

        await this.loadAccounts();

        this.applyFilters();
        await this.getNotifications();
    }

    private toFilterOptions(options: InputOption[]): InputOption[] {
        return options.map(option => ({
            id: option.id == null ? '' : String(option.id),
            text: option.text
        }));
    }

    private createFilters(): NotificationSearchFilters {
        const params = this._route.snapshot.queryParamMap;
        const presetDate = this.parseDate(params.get('date'));
        const period = presetDate ? this.getClosedPeriod(presetDate) : this.getCurrentPeriod();

        return {
            userId: params.get('userId') ?? '',
            reasonForTest: '',
            interval: String(this.interval),
            fromDate: formatDate(period.from, QueryDateFormat, Locale),
            toDate: formatDate(period.to, QueryDateFormat, Locale),
            showHidden: false,
            criteria: {
                propertyTypeAny: true,
                filterAny: true,
                hazardTypeAny: true
            }
        };
    }

    private parseDate(value: string | null): Date | null {
        if (!value) {
            return null;
        }

        const [year, month, day] = value.split('-').map(Number);
        const date = new Date(year, month - 1, day);

        return isNaN(date.getTime()) ? null : date;
    }

    private getCurrentPeriod(): NotificationPeriod {
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        switch (this.interval) {
            case NotificationInterval.EndOfDay: {
                const yesterday = this.addDays(today, -1);
                return { from: yesterday, to: yesterday };
            }

            case NotificationInterval.EndOfWeek:
                return { from: this.addDays(today, -today.getDay()), to: today };

            case NotificationInterval.EndOfMonth:
                return {
                    from: new Date(today.getFullYear(), today.getMonth(), 1),
                    to: new Date(today.getFullYear(), today.getMonth() + 1, 0)
                };

            default:
                return { from: today, to: today };
        }
    }

    private getClosedPeriod(date: Date): NotificationPeriod {
        switch (this.interval) {
            case NotificationInterval.EndOfDay: {
                const previousDay = this.addDays(date, -1);
                return { from: previousDay, to: previousDay };
            }

            case NotificationInterval.EndOfWeek: {
                const weekEnd = this.addDays(date, -date.getDay());
                return { from: this.addDays(weekEnd, -7), to: this.addDays(weekEnd, -1) };
            }

            case NotificationInterval.EndOfMonth:
                return {
                    from: new Date(date.getFullYear(), date.getMonth() - 1, 1),
                    to: new Date(date.getFullYear(), date.getMonth(), 0)
                };

            default:
                return { from: date, to: date };
        }
    }

    private addDays(date: Date, days: number): Date {
        const result = new Date(date);
        result.setDate(result.getDate() + days);

        return result;
    }

    private async loadAccounts(): Promise<void> {
        try {
            this.table.isLoading = true;

            const users = await this._userService.getAll({ pageSize: MAX_PAGE_SIZE }, { sort: {}, filter: [] });

            const accounts = users.data
                .filter(user => user.id != null)
                .map(user => ({ id: String(user.id), text: user.emailAddress ?? user.contactName ?? String(user.id) }))
                .sort((first, second) => first.text.localeCompare(second.text));

            this.accountOptions = [{ id: '', text: 'Any account' }, ...accounts];
        } finally {
            this.table.isLoading = false;
        }
    }

    private getColumns(): TableColumn<NotificationRow>[] {
        return [
            {
                field: 'rowNumber',
                caption: '#',
                type: ColumnType.number,
                queryColumnExcluded: true,
                sortColumnExcluded: true,
                rowCssClass: 'align-middle'
            },
            this.templateColumn('', this.colorCellTemplate),
            this.templateColumn('Date', this.dateCellTemplate),
            this.templateColumn('Notification', this.descriptionCellTemplate),
            this.templateColumn('Property Description', this.propertyCellTemplate),
            this.templateColumn('Assembly Description', this.assemblyCellTemplate),
            this.templateColumn('', this.actionsCellTemplate)
        ];
    }

    private templateColumn(caption: string, template: TemplateRef<CellTemplateData<NotificationRow>>): TableColumn<NotificationRow> {
        return {
            field: '',
            caption: caption,
            type: ColumnType.other,
            cellTemplate: template,
            queryColumnExcluded: true,
            sortColumnExcluded: true,
            rowCssClass: 'align-middle'
        };
    }

    public async search(form: NgForm): Promise<void> {
        if (!form.valid) {
            return;
        }

        this.applyFilters();

        if (this.table.items?.pageInfo) {
            this.table.items.pageInfo.pageNumber = 1;
        }

        await this.getNotifications();
    }

    private applyFilters(): void {
        this.table.query = this.buildQuery();
        this.headerPrefix = this.buildHeaderPrefix();
    }

    private buildQuery(): Query {
        const filter: QueryProperty[] = [
            ...this.buildOptionalFilter('userId', this.filters.userId),
            ...this.buildOptionalFilter('reasonForTest', this.filters.reasonForTest),
            ...this.buildOptionalFilter('interval', this.filters.interval),
            ...this.buildFlagFilters(),
            this.buildDateFilter()
        ];

        if (!this.filters.showHidden) {
            filter.push(this.eqFilter('hidden', 'false'));
        }

        return { sort: {}, filter: filter };
    }

    private buildOptionalFilter(columnName: string, value: string): QueryProperty[] {
        return value ? [this.eqFilter(columnName, value)] : [];
    }

    private buildFlagFilters(): QueryProperty[] {
        return this.flagGroups
            .flatMap(group => this.getCheckedFields(group))
            .map(field => this.eqFilter(field, 'true'));
    }

    private getCheckedFields(group: NotificationFlagGroup): string[] {
        if (this.filters.criteria[group.anyField]) {
            return [];
        }

        return group.options
            .filter(option => this.filters.criteria[option.field])
            .map(option => option.field);
    }

    private buildDateFilter(): QueryProperty {
        return {
            columnName: 'createdTime',
            children: [
                { columnName: 'createdTime', value: this.filters.fromDate, comparisonOperator: 'Gte', logicalOperator: 'And' },
                { columnName: 'createdTime', value: this.filters.toDate, comparisonOperator: 'Lte', logicalOperator: 'And' }
            ]
        };
    }

    private eqFilter(columnName: string, value: string): QueryProperty {
        return { columnName: columnName, comparisonOperator: 'Eq', value: value };
    }

    private buildHeaderPrefix(): string {
        const interval = this.filters.interval ? `${this.findOptionText(this.intervalOptions, this.filters.interval)} ` : '';
        const account = this.filters.userId ? this.findOptionText(this.accountOptions, this.filters.userId) : 'All Accounts';

        const from = formatDate(this.filters.fromDate, DisplayDateFormat, Locale);
        const to = formatDate(this.filters.toDate, DisplayDateFormat, Locale);
        const dateRange = from === to ? `On ${from}` : `From ${from} - ${to}`;

        return `${interval}Notifications for ${account} - ${dateRange}`;
    }

    private findOptionText(options: InputOption[], id: string): string {
        return options.find(option => option.id === id)?.text ?? id;
    }

    public async getNotifications(): Promise<void> {
        try {
            this.table.isLoading = true;

            const notifications = await this._service.getAll(this.table.items?.pageInfo || {}, this.table.query);
            const pageNumber = notifications.pageInfo.pageNumber ?? 1;
            const pageSize = notifications.pageInfo.pageSize ?? notifications.data.length;
            const firstRowNumber = (pageNumber - 1) * pageSize + 1;

            this.table.items = {
                pageInfo: notifications.pageInfo,
                data: notifications.data.map((notification, index) => this.createRow(notification, firstRowNumber + index))
            };

            const totalItems = notifications.pageInfo.totalItems ?? 0;
            const results = totalItems === 1 ? 'Result' : 'Results';

            this.header = `${this.headerPrefix} - (${totalItems} ${results})`;
        } finally {
            this.table.isLoading = false;
        }
    }

    private createRow(notification: Notification, rowNumber: number): NotificationRow {
        return {
            notification: notification,
            rowNumber: rowNumber,
            account: notification.user?.emailAddress ?? '',
            property: this.createPropertyVm(notification),
            assembly: notification.test ? this.createAssemblyVm(notification.test) : null
        };
    }

    private createPropertyVm(notification: Notification): NotificationPropertyVm {
        const test = notification.test;

        if (!test) {
            return {
                iconCss: null,
                lines: notification.propertyDescription ? [notification.propertyDescription] : []
            };
        }

        const street = [test.propertyStreetNumber, test.propertyStreetName].filter(part => !!part).join(' ');
        const streetLine = test.propertyNumber ? `${street} #${test.propertyNumber}` : street;

        const stateZip = [test.propertyState?.code, test.propertyZip].filter(part => !!part).join(' ');
        const cityLine = [test.propertyCity, stateZip].filter(part => !!part).join(', ');

        return {
            siteId: test.siteId,
            iconCss: test.propertyType === PropertyType.Commercial ? 'fa-building text-danger' : 'fa-home text-primary',
            businessName: test.propertyBusinessName,
            lines: [streetLine, cityLine].filter(line => !!line)
        };
    }

    private createAssemblyVm(test: NotificationTest): NotificationAssemblyVm {
        const device = [test.manufacturer, test.model, test.size].filter(part => !!part).join(' ');
        const expirationDate = test.renewalRequired && test.expirationDate ? new Date(test.expirationDate) : null;

        return {
            description: test.deviceType ? `${device} - ${test.deviceType}` : device,
            serialNumber: test.serialNumber ?? '',
            testDate: test.testDate,
            expirationDate: expirationDate,
            expirationCssClass: this.getExpirationCssClass(expirationDate)
        };
    }

    private getExpirationCssClass(expirationDate: Date | null): string {
        if (!expirationDate) {
            return 'bg-secondary';
        }

        return expirationDate >= new Date() ? 'bg-success' : 'bg-danger';
    }

    public async toggleHidden(row: NotificationRow): Promise<void> {
        try {
            this.table.isLoading = true;

            await this._service.setHidden(row.notification.id!, !row.notification.hidden);
            await this.getNotifications();
        } finally {
            this.table.isLoading = false;
        }
    }

    public showDetails(row: NotificationRow): void {
        this._modalHelper.show<Notification>(NotificationDetailsModalComponent, {
            title: 'Notification Settings',
            model: row.notification,
            mode: 'disableFullScreen'
        });
    }

    public exportResults(): void {
        this._downloadService.showDownloadManager(this.downloadConfig, this.table.query);
    }

    // Column names follow V1's export so existing spreadsheets keep lining up.
    private buildDownloadColumns(): DownloadColumn[] {
        return [
            { field: 'createdTime', caption: 'CreationDate' },
            { field: 'user.emailAddress', caption: 'UserID' },
            { field: 'recordId', caption: 'RecordID' },
            { field: 'description', caption: 'Description' },
            { field: 'reasonForTest', caption: 'BackflowTestType' },
            ...this.buildFlagColumns([...this._options.propertyTypeOptions.map(option => option.field), 'propertyTypeAny']),
            ...this.buildFlagColumns([...this._options.filterOptions.map(option => option.field), 'filterAny'], 'Notification'),
            ...this.buildFlagColumns([...this._options.hazardTypeOptions.map(option => option.field), 'hazardTypeAny']),
            { field: 'interval', caption: 'NotificationInterval' },
            { field: 'deliveryType', caption: 'NotificationType' }
        ];
    }

    private buildFlagColumns(fields: (keyof NotificationSetting)[], captionPrefix: string = ''): DownloadColumn[] {
        return fields.map(field => ({
            field: field,
            caption: `${captionPrefix}${field.charAt(0).toUpperCase()}${field.slice(1)}`
        }));
    }
}

interface NotificationSearchFilters {
    userId: string;
    reasonForTest: string;
    interval: string;
    fromDate: string;
    toDate: string;
    showHidden: boolean;
    criteria: NotificationSetting;
}

interface NotificationFlagGroup {
    title: string;
    cssClass: string;
    anyField: keyof NotificationSetting;
    options: NotificationFlagOption[];
}

interface NotificationPeriod {
    from: Date;
    to: Date;
}

interface NotificationRow {
    notification: Notification;
    rowNumber: number;
    account: string;
    property: NotificationPropertyVm;
    assembly: NotificationAssemblyVm | null;
}

interface NotificationPropertyVm {
    siteId?: number | null;
    iconCss: string | null;
    businessName?: string | null;
    lines: string[];
}

interface NotificationAssemblyVm {
    description: string;
    serialNumber: string;
    testDate?: Date | null;
    expirationDate: Date | null;
    expirationCssClass: string;
}
