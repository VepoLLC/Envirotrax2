import { Component, ElementRef, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { NgForm } from "@angular/forms";
import { CellTemplateData, ColumnType, InputOption, TableColumn } from "@envirotrax/common-ui";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { PropertyLog } from "../../../shared/models/sites/property-log";
import { SiteLogType } from "../../../shared/models/sites/site-log-type.enum";
import { SiteLogReviewDateStatus } from "../../../shared/models/sites/site-log-review-date-status.enum";
import { ComparisonOperator, Query, QueryProperty } from "../../../shared/models/query";
import { WaterSupplierUser } from "../../../shared/models/users/water-supplier-user";
import { PropertyLogService } from "../../../shared/services/sites/property-log.service";
import { UserService } from "../../../shared/services/water-suppliers/user.service";
import { DownloadService } from "../../../shared/services/download.service";
import { PrintableTableService } from "../../../shared/services/printable-table.service";
import { DownloadColumn, DownloadConfig } from "../../../shared/models/download-config";
import { MAX_PAGE_SIZE } from "../../../shared/models/page-info";
import { AppContainerHelperService } from "../../../shared/services/helpers/app-contaner-helper.service";

type PropertyLogRow = PropertyLog & {
    rowNumber?: number;
};

@Component({
    standalone: false,
    templateUrl: './property-log-management.component.html'
})
export class PropertyLogManagementComponent implements OnInit {
    @ViewChild('numberTemplate', { static: true })
    public numberTemplate!: TemplateRef<CellTemplateData<PropertyLog>>;

    @ViewChild('propertyTemplate', { static: true })
    public propertyTemplate!: TemplateRef<CellTemplateData<PropertyLog>>;

    @ViewChild('mailingTemplate', { static: true })
    public mailingTemplate!: TemplateRef<CellTemplateData<PropertyLog>>;

    @ViewChild('accountNumberTemplate', { static: true })
    public accountNumberTemplate!: TemplateRef<CellTemplateData<PropertyLog>>;

    @ViewChild('logTemplate', { static: true })
    public logTemplate!: TemplateRef<CellTemplateData<PropertyLog>>;

    @ViewChild('reviewDateTemplate', { static: true })
    public reviewDateTemplate!: TemplateRef<CellTemplateData<PropertyLog>>;

    @ViewChild('viewSiteTemplate', { static: true })
    public viewSiteTemplate!: TemplateRef<CellTemplateData<PropertyLog>>;

    @ViewChild('printableSection')
    private _printableSection!: ElementRef;

    public readonly SiteLogType = SiteLogType;

    public readonly reviewDateStatusClasses: { [key: number]: string } = {
        [SiteLogReviewDateStatus.None]: '',
        [SiteLogReviewDateStatus.Overdue]: 'bg-danger',
        [SiteLogReviewDateStatus.DueSoon]: 'bg-warning text-dark',
        [SiteLogReviewDateStatus.Upcoming]: 'bg-success',
        [SiteLogReviewDateStatus.Completed]: 'bg-secondary'
    };

    public logType: string = '0';
    public sortBy: string = '0';
    public showMailing: boolean = false;
    public userAccountType: string = '0';
    public accountId: string = '';
    public accountNumber: string = '';
    public propertyStreet: string = '';

    public resultsHeaderPrefix: string = 'Property Logs';

    public logTypeOptions: InputOption[] = [
        { id: '0', text: 'Any log type' },
        { id: '1', text: 'Expired reviews' },
        { id: '2', text: 'Expiring reviews' }
    ];

    public sortOptions: InputOption[] = [
        { id: '0', text: 'Date Descending' },
        { id: '1', text: 'Date Ascending' }
    ];

    public userAccountTypeOptions: InputOption[] = [
        { id: '0', text: 'Log creator' },
        { id: '1', text: 'CSI compliance assignment' },
        { id: '2', text: 'Backflow compliance assignment' },
        { id: '3', text: 'FOG compliance assignment' }
    ];

    public accountOptions: InputOption[] = [{ id: '', text: 'Any account' }];

    public propertyTypes: InputOption[] = [
        { id: '', text: 'Any property type' },
        { id: '0', text: 'Residential' },
        { id: '1', text: 'Commercial' }
    ];

    public table: TableViewModel<PropertyLog> = {
        columns: [],
        query: { sort: { createdTime: 'Desc' }, filter: [] }
    };

    public downloadConfig: DownloadConfig;

    private panelFilters: QueryProperty[] = [];

    constructor(
        private readonly _propertyLogService: PropertyLogService,
        private readonly _userService: UserService,
        private readonly _downloadService: DownloadService,
        private readonly _printService: PrintableTableService,
        private readonly _containerHelper: AppContainerHelperService
    ) {
        this.downloadConfig = {
            fileName: 'Property Logs',
            endpoint: this._propertyLogService.getEndpoint(),
            suppoertedFormats: ['CSV', 'Excel'],
            columns: []
        };
    }

    public async ngOnInit(): Promise<void> {
        this._containerHelper.setContainerVisibility(false);

        this.table.columns = this.getColumns();

        await this.loadUsers();

        this.table.query = this.buildQuery();
        this.resultsHeaderPrefix = this.buildResultsHeaderPrefix();

        await this.getPropertyLogs();
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.panelFilters = queryProperties;
    }

    public async search(form: NgForm): Promise<void> {
        if (!form.valid) {
            return;
        }

        this.table.columns = this.getColumns();
        this.table.query = this.buildQuery();
        this.resultsHeaderPrefix = this.buildResultsHeaderPrefix();

        if (this.table.items?.pageInfo) {
            this.table.items.pageInfo.pageNumber = 1;
        }

        await this.getPropertyLogs();
    }

    public exportResults(): void {
        this.downloadConfig.columns = this.buildDownloadColumns();

        this._downloadService.showDownloadManager(this.downloadConfig, this.table.query);
    }

    public viewPrintableTable(): void {
        this._printService.open(this._printableSection.nativeElement);
    }

    public async getPropertyLogs(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._propertyLogService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );

            const pageInfo = this.table.items?.pageInfo;
            const offset = ((pageInfo?.pageNumber || 1) - 1) * (pageInfo?.pageSize || 0);

            (this.table.items?.data ?? []).forEach((log, index) => {
                (log as PropertyLogRow).rowNumber = offset + index + 1;
            });
        } finally {
            this.table.isLoading = false;
        }
    }

    private async loadUsers(): Promise<void> {
        const users = await this._userService.getAll(
            { pageSize: MAX_PAGE_SIZE },
            { sort: { contactName: 'Asc' }, filter: [] }
        );

        const accounts = (users.data ?? []).filter((user: WaterSupplierUser) => user.id != null);

        this.accountOptions.push(...accounts.map((user: WaterSupplierUser) => ({
            id: String(user.id),
            text: user.contactName ?? user.emailAddress ?? String(user.id)
        })));
    }

    private getColumns(): TableColumn<PropertyLog>[] {
        const columns: TableColumn<PropertyLog>[] = [
            this.templateColumn('', this.numberTemplate),
            this.templateColumn('Property Information', this.propertyTemplate),
            this.templateColumn('Account Number', this.accountNumberTemplate),
            this.templateColumn('Property Log', this.logTemplate),
            this.templateColumn('', this.reviewDateTemplate),
            this.templateColumn('', this.viewSiteTemplate)
        ];

        if (this.showMailing) {
            columns.splice(2, 0, this.templateColumn('Mailing Information', this.mailingTemplate));
        }

        return columns;
    }

    private templateColumn(caption: string, template: TemplateRef<CellTemplateData<PropertyLog>>): TableColumn<PropertyLog> {
        return {
            field: '',
            caption,
            type: ColumnType.other,
            queryColumnExcluded: true,
            cellTemplate: template,
            rowCssClass: 'align-top'
        };
    }

    private buildQuery(): Query {
        const filter = [
            ...this.panelFilters,
            ...this.buildLogTypeFilter(),
            ...this.buildAccountFilter(),
            ...this.buildAccountNumberFilter(),
            ...this.buildStreetFilter()
        ];

        return { sort: this.buildSort(), filter };
    }

    private buildSort(): { [key: string]: 'Asc' | 'Desc' } {
        const direction: 'Asc' | 'Desc' = this.sortBy === '0' ? 'Desc' : 'Asc';
        const field = this.logType === '0' ? 'createdTime' : 'reviewDate';

        return { [field]: direction };
    }

    private buildResultsHeaderPrefix(): string {
        if (this.logType === '1') {
            return 'Expired Property Logs';
        }

        if (this.logType === '2') {
            return 'Expiring Property Logs';
        }

        return 'Property Logs';
    }

    private buildLogTypeFilter(): QueryProperty[] {
        if (this.logType === '1') {
            return [
                this.eqFilter('logType', String(SiteLogType.Reminder)),
                this.rangeFilter('reviewDate', [{ value: this.nowIso(), op: 'Lt' }])
            ];
        }

        if (this.logType === '2') {
            return [
                this.eqFilter('logType', String(SiteLogType.Reminder)),
                this.rangeFilter('reviewDate', [
                    { value: this.nowIso(), op: 'Gte' },
                    { value: this.plusDaysIso(30), op: 'Lte' }
                ])
            ];
        }

        return [];
    }

    private buildAccountFilter(): QueryProperty[] {
        if (!this.accountId) {
            return [];
        }

        return [this.eqFilter(this.accountColumn(), this.accountId)];
    }

    private accountColumn(): string {
        switch (this.userAccountType) {
            case '1':
                return 'site.csiAccountAssignmentId';
            case '2':
                return 'site.backflowAccountAssignmentId';
            case '3':
                return 'site.fogAccountAssignmentId';
            default:
                return 'createdById';
        }
    }

    private buildAccountNumberFilter(): QueryProperty[] {
        const value = this.accountNumber.trim();

        if (!value) {
            return [];
        }

        return [this.eqFilter('site.accountNumber', value)];
    }

    private buildStreetFilter(): QueryProperty[] {
        const text = this.propertyStreet.trim();

        if (!text) {
            return [];
        }

        const match = text.match(/^(\d+)\s+(.*)$/);

        if (!match) {
            return [this.rangeFilter('site.streetName', [{ value: text, op: 'Ct' }])];
        }

        const filters = [this.eqFilter('site.streetNumber', match[1])];
        const streetName = match[2].trim();

        if (streetName) {
            filters.push(this.rangeFilter('site.streetName', [{ value: streetName, op: 'Ct' }]));
        }

        return filters;
    }

    private buildDownloadColumns(): DownloadColumn[] {
        const columns: DownloadColumn[] = [
            { field: 'site.accountNumber', caption: 'AccountNumber' },
            { field: 'site.propertyType', caption: 'PropertyType' },
            { field: 'site.businessName', caption: 'PropertyBusinessName' },
            { field: 'site.streetNumber', caption: 'PropertyStreetNumber' },
            { field: 'site.streetName', caption: 'PropertyStreetName' },
            { field: 'site.propertyNumber', caption: 'PropertyNumber' },
            { field: 'site.city', caption: 'PropertyCity' },
            { field: 'site.state.code', caption: 'PropertyState' },
            { field: 'site.zipCode', caption: 'PropertyZIP' }
        ];

        if (this.showMailing) {
            columns.push(
                { field: 'site.mailingCompanyName', caption: 'MailingCompanyName' },
                { field: 'site.mailingContactName', caption: 'MailingContactName' },
                { field: 'site.mailingStreetNumber', caption: 'MailingStreetNumber' },
                { field: 'site.mailingStreetName', caption: 'MailingStreetName' },
                { field: 'site.mailingNumber', caption: 'MailingNumber' },
                { field: 'site.mailingCity', caption: 'MailingCity' },
                { field: 'site.mailingState.code', caption: 'MailingState' },
                { field: 'site.mailingZipCode', caption: 'MailingZIP' },
                { field: 'site.mailingPhoneNumber', caption: 'MailingPhoneNumber' },
                { field: 'site.mailingEmailAddress', caption: 'MailingEmailAddress' }
            );
        }

        columns.push(
            { field: 'createdTime', caption: 'LogCreationDate' },
            { field: 'createdBy.email', caption: 'LogUserID' },
            { field: 'logType', caption: 'LogType' },
            { field: 'noteText', caption: 'LogNote' },
            { field: 'reviewDate', caption: 'LogReviewDate' },
            { field: 'assembly.serialNumber', caption: 'LogTaggedAssembly' },
            { field: 'url', caption: 'LogFileAttachment' }
        );

        return columns;
    }

    private eqFilter(columnName: string, value: string): QueryProperty {
        return { columnName, comparisonOperator: 'Eq', value };
    }

    private rangeFilter(columnName: string, comparisons: { value: string; op: ComparisonOperator }[]): QueryProperty {
        return {
            columnName,
            children: comparisons.map(comparison => ({
                columnName,
                value: comparison.value,
                comparisonOperator: comparison.op,
                logicalOperator: 'And' as const
            }))
        };
    }

    private nowIso(): string {
        return new Date().toISOString();
    }

    private plusDaysIso(days: number): string {
        const date = new Date();
        date.setDate(date.getDate() + days);

        return date.toISOString();
    }
}
