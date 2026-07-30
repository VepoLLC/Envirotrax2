import { Component, ElementRef, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { NgForm } from "@angular/forms";
import { CellTemplateData, ColumnType, InputOption, TableColumn } from "@envirotrax/common-ui";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { BackflowCompliance } from "../../../shared/models/backflow/backflow-compliance";
import { BackflowTestResult } from "../../../shared/models/backflow/backflow-test-enums";
import { ComparisonOperator, Query, QueryProperty } from "../../../shared/models/query";
import { WaterSupplierUser } from "../../../shared/models/users/water-supplier-user";
import { DownloadConfig } from "../../../shared/models/download-config";
import { MAX_PAGE_SIZE } from "../../../shared/models/page-info";
import { PermissionAction, PermissionType } from "../../../shared/models/permission-type";
import { FacilityType } from "../../../shared/enums/facility-type.enum";
import { PropertyType } from "../../../shared/enums/property-type.enum";
import { BackflowTestService } from "../../../shared/services/backflow/backflow-test.service";
import { BackflowTestOptionsService } from "../../../shared/services/backflow/backflow-test-options.service";
import { SiteService } from "../../../shared/services/sites/site.service";
import { UserService } from "../../../shared/services/water-suppliers/user.service";
import { AuthService } from "../../../shared/services/auth/auth.service";
import { DownloadService } from "../../../shared/services/download.service";
import { PrintableTableService } from "../../../shared/services/printable-table.service";
import { AppContainerHelperService } from "../../../shared/services/helpers/app-contaner-helper.service";
import { PropertyLogCellComponent } from "../../../shared/components/data-components/table-cells/property-log-cell.component";

const DAY_MS = 86400000;

// A grid row: an expired assembly plus its site's logs, decorated with presentation-only fields.
type ComplianceRow = BackflowCompliance & {
    rowNumber?: number;
    isFirstOfGroup?: boolean;   // first row of a contiguous same-site group — site columns render here only
    canModify?: boolean;        // page-level Sites-modify permission, surfaced to the property-log cell
    daysExpired?: number;
    expiredClass?: string;
    assignedName?: string;
};

@Component({
    standalone: false,
    templateUrl: './backflow-compliance-management.component.html'
})
export class BackflowComplianceManagementComponent implements OnInit {
    @ViewChild('numberTemplate', { static: true })
    public numberTemplate!: TemplateRef<CellTemplateData<BackflowCompliance>>;

    @ViewChild('propertyTemplate', { static: true })
    public propertyTemplate!: TemplateRef<CellTemplateData<BackflowCompliance>>;

    @ViewChild('mailingTemplate', { static: true })
    public mailingTemplate!: TemplateRef<CellTemplateData<BackflowCompliance>>;

    @ViewChild('assignedToTemplate', { static: true })
    public assignedToTemplate!: TemplateRef<CellTemplateData<BackflowCompliance>>;

    @ViewChild('statusTemplate', { static: true })
    public statusTemplate!: TemplateRef<CellTemplateData<BackflowCompliance>>;

    @ViewChild('datesTemplate', { static: true })
    public datesTemplate!: TemplateRef<CellTemplateData<BackflowCompliance>>;

    @ViewChild('serialTemplate', { static: true })
    public serialTemplate!: TemplateRef<CellTemplateData<BackflowCompliance>>;

    @ViewChild('assemblyTemplate', { static: true })
    public assemblyTemplate!: TemplateRef<CellTemplateData<BackflowCompliance>>;

    @ViewChild('accountTemplate', { static: true })
    public accountTemplate!: TemplateRef<CellTemplateData<BackflowCompliance>>;

    @ViewChild('viewTemplate', { static: true })
    public viewTemplate!: TemplateRef<CellTemplateData<BackflowCompliance>>;

    @ViewChild('printableSection')
    private _printableSection!: ElementRef;

    public readonly BackflowTestResult = BackflowTestResult;
    public readonly PropertyType = PropertyType;

    public canModify: boolean = false;
    public resultsHeaderPrefix: string = 'Expired Assemblies';

    // Filter state
    public expiredFrom: string = 'any';
    public expiredTo: string = 'now';
    public customFromDate: string = '';
    public customToDate: string = '';
    public sortBy: string = '0';
    public groupBySite: boolean = true;
    public showMailing: boolean = false;
    public emailWhenAssigned: boolean = false;
    public propertyStreet: string = '';

    public users: WaterSupplierUser[] = [];
    public userOptions: InputOption[] = [{ id: '', text: 'Any account' }];

    public expiredFromOptions: InputOption[] = [
        { id: 'any', text: 'Any date' },
        { id: '7', text: '7 days ago' },
        { id: '30', text: '30 days ago' },
        { id: '60', text: '60 days ago' },
        { id: '90', text: '90 days ago' },
        { id: '120', text: '120 days ago' },
        { id: '180', text: '180 days ago' },
        { id: '365', text: '1 year ago' },
        { id: 'week', text: 'Beginning of week' },
        { id: 'month', text: 'Beginning of month' },
        { id: 'year', text: 'Beginning of year' },
        { id: 'custom', text: 'Custom date' }
    ];

    public expiredToOptions: InputOption[] = [
        { id: 'now', text: 'Now' },
        { id: '30', text: '30 days ago' },
        { id: '60', text: '60 days ago' },
        { id: '90', text: '90 days ago' },
        { id: '120', text: '120 days ago' },
        { id: '180', text: '180 days ago' },
        { id: 'custom', text: 'Custom date' }
    ];

    public sortOptions: InputOption[] = [
        { id: '0', text: 'Expiration Date - Oldest > Newest' },
        { id: '1', text: 'Expiration Date - Newest > Oldest' }
    ];

    public propertyTypes: InputOption[] = [
        { id: '', text: 'Any property type' },
        { id: String(PropertyType.Residential), text: 'Residential' },
        { id: String(PropertyType.Commercial), text: 'Commercial' }
    ];

    public yesNoOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: 'true', text: 'Yes' },
        { id: 'false', text: 'No' }
    ];

    public facilityTypes: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: String(FacilityType.Restaurant), text: 'Restaurant' },
        { id: String(FacilityType.FastFoodEstablishment), text: 'Fast Food Establishment' },
        { id: String(FacilityType.HotelMotel), text: 'Hotel / Motel' },
        { id: String(FacilityType.CarWash), text: 'Car Wash' },
        { id: String(FacilityType.SchoolUniversity), text: 'School / University' },
        { id: String(FacilityType.GroceryStore), text: 'Grocery Store' },
        { id: String(FacilityType.ConvenienceStore), text: 'Convenience Store' },
        { id: String(FacilityType.AssistedLivingFacility), text: 'Assisted Living Facility' },
        { id: String(FacilityType.MedicalFacility), text: 'Medical Facility' },
        { id: String(FacilityType.Industrial), text: 'Industrial' },
        { id: String(FacilityType.CityOwnedFacility), text: 'City Owned Facility' },
        { id: String(FacilityType.Other), text: 'Other' }
    ];

    public deviceTypeOptions: InputOption[];
    public hazardTypeOptions: InputOption[];

    public table: TableViewModel<BackflowCompliance> = {
        columns: [],
        query: { sort: { accountNumber: 'Asc', expirationDate: 'Asc' }, filter: [] }
    };

    public downloadConfig: DownloadConfig;

    private panelFilters: QueryProperty[] = [];
    private readonly today: number = new Date().setHours(0, 0, 0, 0);

    constructor(
        private readonly _backflowTestService: BackflowTestService,
        private readonly _options: BackflowTestOptionsService,
        private readonly _siteService: SiteService,
        private readonly _userService: UserService,
        private readonly _authService: AuthService,
        private readonly _downloadService: DownloadService,
        private readonly _printService: PrintableTableService,
        private readonly _containerHelper: AppContainerHelperService
    ) {
        this.deviceTypeOptions = this._options.deviceTypeFilterOptions;
        this.hazardTypeOptions = this._options.hazardTypeFilterOptions;

        this.downloadConfig = {
            fileName: 'Backflow Compliance',
            endpoint: this._backflowTestService.getBackflowComplianceEndpoint(),
            suppoertedFormats: ['CSV', 'Excel'],
            columns: [
                { field: 'accountNumber', caption: 'AccountNumber' },
                { field: 'propertyBusinessName', caption: 'PropertyBusinessName' },
                { field: 'propertyStreetNumber', caption: 'PropertyStreetNumber' },
                { field: 'propertyStreetName', caption: 'PropertyStreetName' },
                { field: 'propertyNumber', caption: 'PropertyNumber' },
                { field: 'propertyCity', caption: 'PropertyCity' },
                { field: 'propertyState.code', caption: 'PropertyState' },
                { field: 'propertyZip', caption: 'PropertyZIP' },
                { field: 'mailingCompanyName', caption: 'MailingCompanyName' },
                { field: 'mailingContactName', caption: 'MailingContactName' },
                { field: 'mailingStreetNumber', caption: 'MailingStreetNumber' },
                { field: 'mailingStreetName', caption: 'MailingStreetName' },
                { field: 'mailingCity', caption: 'MailingCity' },
                { field: 'mailingState.code', caption: 'MailingState' },
                { field: 'mailingZip', caption: 'MailingZIP' },
                { field: 'testResult', caption: 'TestResult' },
                { field: 'testDate', caption: 'TestDate' },
                { field: 'expirationDate', caption: 'ExpirationDate' },
                { field: 'deviceType', caption: 'DeviceType' },
                { field: 'manufacturer', caption: 'Manufacturer' },
                { field: 'model', caption: 'Model' },
                { field: 'size', caption: 'Size' },
                { field: 'serialNumber', caption: 'SerialNumber' },
                { field: 'hazardType', caption: 'HazardType' },
                { field: 'locationDescription', caption: 'LocationDescription' }
            ]
        };
    }

    public async ngOnInit(): Promise<void> {
        this._containerHelper.setContainerVisibility(false);

        // Show the spinner for the whole initial load — the permission check and user lookup run before
        // getCompliance() would otherwise turn it on, which left the page blank until the data call started.
        try {
            this.table.isLoading = true;

            this.canModify = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.Sites);
            this.table.columns = this.getColumns();

            await this.loadUsers();

            this.table.query = this.buildQuery();
            this.resultsHeaderPrefix = this.buildResultsHeaderPrefix();

            await this.getCompliance();
        } finally {
            this.table.isLoading = false;
        }
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

        await this.getCompliance();
    }

    public exportResults(): void {
        this._downloadService.showDownloadManager(this.downloadConfig, this.table.query);
    }

    public viewPrintableTable(): void {
        this._printService.open(this._printableSection.nativeElement);
    }

    public async getCompliance(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._backflowTestService.getBackflowCompliance(
                this.table.items?.pageInfo || {},
                this.table.query
            );

            const pageInfo = this.table.items?.pageInfo;
            const offset = ((pageInfo?.pageNumber || 1) - 1) * (pageInfo?.pageSize || 0);

            const rows = (this.table.items?.data ?? []) as ComplianceRow[];
            let previousSiteId: number | undefined;

            rows.forEach((row, index) => {
                row.rowNumber = offset + index + 1;
                row.canModify = this.canModify;

                if (this.groupBySite) {
                    // Site columns render once per contiguous same-site run. A site's assemblies stay
                    // adjacent because grouped mode sorts by accountNumber (buildSort), and the key is the
                    // site id so the rare "two sites share an account number" case still splits correctly.
                    row.isFirstOfGroup = index === 0 || row.site?.id !== previousSiteId;
                    previousSiteId = row.site?.id;
                } else {
                    // Flat, expiration-ordered list — every row is self-contained (matches Backflow Test Search).
                    row.isFirstOfGroup = true;
                }

                this.decorate(row);
            });
        } finally {
            this.table.isLoading = false;
        }
    }

    public async onAssignmentChange(row: BackflowCompliance, value: string): Promise<void> {
        const siteId = row.site?.id;

        if (siteId == null) {
            return;
        }

        const userId = value ? Number(value) : null;

        await this._siteService.updateBackflowAssignment(siteId, userId);

        const assignmentDate = userId ? new Date().toISOString() : undefined;

        // A site's assignment shows once per group but every row carries the site — sync them all so the
        // grid stays consistent without a reload.
        (this.table.items?.data ?? []).forEach(r => {
            if (r.site?.id === siteId && r.site) {
                r.site.backflowAccountAssignmentId = userId ?? undefined;
                r.site.backflowAccountAssignmentDate = assignmentDate;
                (r as ComplianceRow).assignedName = this.assignedUserName(r);
            }
        });

        if (this.emailWhenAssigned && userId) {
            this.sendAssignmentEmail(row, userId);
        }
    }

    private async loadUsers(): Promise<void> {
        const users = await this._userService.getAll(
            { pageSize: MAX_PAGE_SIZE },
            { sort: { contactName: 'Asc' }, filter: [] }
        );

        this.users = (users.data ?? []).filter(user => user.id != null);
        this.userOptions.push(...this.users.map(user => ({
            id: String(user.id),
            text: user.contactName ?? user.emailAddress ?? String(user.id)
        })));
    }

    private getColumns(): TableColumn<BackflowCompliance>[] {
        const columns: TableColumn<BackflowCompliance>[] = [
            this.templateColumn('', this.numberTemplate),
            this.templateColumn('Property Information', this.propertyTemplate),
            this.templateColumn('Account Number', this.accountTemplate),
            this.logColumn(),
            this.templateColumn('Assigned To', this.assignedToTemplate),
            this.templateColumn('Status', this.statusTemplate),
            this.templateColumn('Test / Exp Dates', this.datesTemplate),
            this.templateColumn('Serial #', this.serialTemplate),
            this.templateColumn('Assembly Information', this.assemblyTemplate),
            this.templateColumn('', this.viewTemplate)
        ];

        if (this.showMailing) {
            columns.splice(2, 0, this.templateColumn('Mailing Information', this.mailingTemplate));
        }

        return columns;
    }

    private templateColumn(caption: string, template: TemplateRef<CellTemplateData<BackflowCompliance>>): TableColumn<BackflowCompliance> {
        return { field: '', caption, type: ColumnType.other, queryColumnExcluded: true, cellTemplate: template, rowCssClass: 'align-top' };
    }

    // Property Log cell rendered by a dedicated app cell component (via cellComponent, not cellTemplate).
    // The cell reads canModify off the row (set in getCompliance) since it is instantiated dynamically.
    private logColumn(): TableColumn<BackflowCompliance> {
        return {
            field: '',
            caption: 'Property Log',
            type: ColumnType.other,
            queryColumnExcluded: true,
            cellComponent: PropertyLogCellComponent,
            rowCssClass: 'align-top'
        };
    }

    private buildQuery(): Query {
        const filter = [...this.panelFilters, ...this.buildExpirationFilter(), ...this.buildStreetFilter()];
        return { sort: this.buildSort(), filter };
    }

    private buildSort(): { [key: string]: 'Asc' | 'Desc' } {
        const direction: 'Asc' | 'Desc' = this.sortBy === '0' ? 'Asc' : 'Desc';

        // Group-by-site keeps a site's assemblies contiguous (they share an account number), then orders
        // within the site by expiration. Otherwise it's a flat expiration-ordered list.
        return this.groupBySite
            ? { accountNumber: 'Asc', expirationDate: direction }
            : { expirationDate: direction };
    }

    private buildResultsHeaderPrefix(): string {
        const from = this.computeFromDate();
        const to = this.computeToDate();
        const fromLabel = from ? this.formatDate(from) : 'Any date';
        return `Expired Assemblies (${fromLabel} to ${this.formatDate(to)})`;
    }

    private buildExpirationFilter(): QueryProperty[] {
        const from = this.computeFromDate();
        const to = this.computeToDate();

        const comparisons: { value: string; op: ComparisonOperator }[] = [];

        if (from) {
            comparisons.push({ value: from.toISOString(), op: 'Gte' });
        }

        comparisons.push({ value: to.toISOString(), op: 'Lte' });

        return [this.rangeFilter('expirationDate', comparisons)];
    }

    private buildStreetFilter(): QueryProperty[] {
        const text = this.propertyStreet.trim();

        if (!text) {
            return [];
        }

        const match = text.match(/^(\d+)\s+(.*)$/);

        if (!match) {
            return [this.rangeFilter('propertyStreetName', [{ value: text, op: 'Ct' }])];
        }

        const filters = [this.rangeFilter('propertyStreetNumber', [{ value: match[1], op: 'Eq' }])];
        const streetName = match[2].trim();

        if (streetName) {
            filters.push(this.rangeFilter('propertyStreetName', [{ value: streetName, op: 'Ct' }]));
        }

        return filters;
    }

    private rangeFilter(columnName: string, comparisons: { value: string; op: ComparisonOperator }[]): QueryProperty {
        return {
            columnName,
            children: comparisons.map(c => ({
                columnName,
                value: c.value,
                comparisonOperator: c.op,
                logicalOperator: 'And'
            }))
        };
    }

    private computeFromDate(): Date | null {
        const now = new Date();

        switch (this.expiredFrom) {
            case 'any':
                return null;
            case 'custom':
                return this.customFromDate ? this.startOfDay(new Date(this.customFromDate)) : null;
            case 'week': {
                const date = new Date();
                date.setDate(date.getDate() - date.getDay());
                return this.startOfDay(date);
            }
            case 'month':
                return new Date(now.getFullYear(), now.getMonth(), 1);
            case 'year':
                return new Date(now.getFullYear(), 0, 1);
            default: {
                const date = new Date();
                date.setDate(date.getDate() - Number(this.expiredFrom));
                return this.startOfDay(date);
            }
        }
    }

    private computeToDate(): Date {
        if (this.expiredTo === 'now') {
            return this.endOfDay(new Date());
        }

        if (this.expiredTo === 'custom') {
            return this.customToDate ? this.endOfDay(new Date(this.customToDate)) : this.endOfDay(new Date());
        }

        const date = new Date();
        date.setDate(date.getDate() - Number(this.expiredTo));
        return this.endOfDay(date);
    }

    private startOfDay(date: Date): Date {
        date.setHours(0, 0, 0, 0);
        return date;
    }

    private endOfDay(date: Date): Date {
        date.setHours(23, 59, 59, 999);
        return date;
    }

    private formatDate(date: Date): string {
        return `${date.getMonth() + 1}/${date.getDate()}/${date.getFullYear()}`;
    }

    private decorate(row: ComplianceRow): void {
        row.daysExpired = this.getDaysExpired(row);
        row.expiredClass = row.daysExpired != null ? this.expiredBadgeClass(row.daysExpired) : '';
        row.assignedName = this.assignedUserName(row);
    }

    private getDaysExpired(row: BackflowCompliance): number | undefined {
        if (!row.expirationDate) {
            return undefined;
        }

        const expiration = new Date(row.expirationDate).setHours(0, 0, 0, 0);
        const days = Math.floor((this.today - expiration) / DAY_MS);

        return days < 0 ? undefined : days;
    }

    private expiredBadgeClass(days: number): string {
        if (days > 60) {
            return 'bg-danger';
        }

        if (days > 30) {
            return 'bg-warning text-dark';
        }

        return 'bg-warning-subtle text-dark border';
    }

    private assignedUserName(row: BackflowCompliance): string {
        if (row.site?.backflowAccountAssignmentId == null) {
            return 'Unassigned';
        }

        const user = this.users.find(u => u.id === row.site!.backflowAccountAssignmentId);
        return user?.contactName ?? user?.emailAddress ?? '';
    }

    private sendAssignmentEmail(row: BackflowCompliance, userId: number): void {
        const user = this.users.find(u => u.id === userId);

        if (!user?.emailAddress) {
            return;
        }

        const cityStateZip = [row.propertyCity, row.propertyState?.code, row.propertyZip].filter(Boolean).join(' ');
        const link = `${window.location.origin}/sites/${row.site?.id}/edit`;

        const body = `Your Envirotrax account has been assigned to the following property record:\r\n\r\n` +
            `Account #:  ${row.accountNumber ?? ''}\r\n\r\n` +
            `${row.propertyBusinessName ?? ''}\r\n` +
            `${row.propertyStreetNumber ?? ''} ${row.propertyStreetName ?? ''}\r\n` +
            `${cityStateZip}\r\n\r\n` +
            `Open the property record:\r\n${link}`;

        const subject = encodeURIComponent(`Backflow Account Assignment - ${row.propertyBusinessName || row.accountNumber || ''}`);
        window.open(`mailto:${user.emailAddress}?subject=${subject}&body=${encodeURIComponent(body)}`);
    }
}
