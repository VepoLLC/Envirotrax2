import { Component, ElementRef, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { NgForm } from "@angular/forms";
import { CellTemplateData, ColumnType, InputOption, TableColumn } from "@envirotrax/common-ui";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { Site } from "../../../shared/models/sites/site";
import { ComparisonOperator, Query, QueryProperty } from "../../../shared/models/query";
import { WaterSupplierUser } from "../../../shared/models/users/water-supplier-user";
import { SiteService } from "../../../shared/services/sites/site.service";
import { UserService } from "../../../shared/services/water-suppliers/user.service";
import { AuthService } from "../../../shared/services/auth/auth.service";
import { DownloadService } from "../../../shared/services/download.service";
import { PrintableTableService } from "../../../shared/services/printable-table.service";
import { DownloadConfig } from "../../../shared/models/download-config";
import { MAX_PAGE_SIZE } from "../../../shared/models/page-info";
import { PermissionAction, PermissionType } from "../../../shared/models/permission-type";
import { FacilityType } from "../../../shared/enums/facility-type.enum";
import { AppContainerHelperService } from "../../../shared/services/helpers/app-contaner-helper.service";
import { PropertyLogCellComponent } from "../../../shared/components/data-components/table-cells/property-log-cell.component";

const DAY_MS = 86400000;

interface OverdueBucket {
    amount: number;
    unit: 'month' | 'year';
    mode: 'greaterThan' | 'lessThan';
}

const OVERDUE_BUCKETS: { [index: number]: OverdueBucket } = {
    1: { amount: 1, unit: 'month', mode: 'greaterThan' },
    2: { amount: 2, unit: 'month', mode: 'greaterThan' },
    3: { amount: 3, unit: 'month', mode: 'greaterThan' },
    4: { amount: 6, unit: 'month', mode: 'greaterThan' },
    5: { amount: 9, unit: 'month', mode: 'greaterThan' },
    6: { amount: 1, unit: 'year', mode: 'greaterThan' },
    7: { amount: 1, unit: 'month', mode: 'lessThan' },
    8: { amount: 2, unit: 'month', mode: 'lessThan' },
    9: { amount: 3, unit: 'month', mode: 'lessThan' },
    10: { amount: 6, unit: 'month', mode: 'lessThan' },
    11: { amount: 9, unit: 'month', mode: 'lessThan' },
    12: { amount: 1, unit: 'year', mode: 'lessThan' }
};

type SiteRow = Site & {
    dueDate?: Date;
    daysOverdue?: number;
    overdueClass?: string;
    assignedName?: string;
    rowNumber?: number;
    canModify?: boolean;   // surfaced to the shared property-log cell
};

@Component({
    standalone: false,
    templateUrl: './fog-trip-ticket-compliance-management.component.html'
})
export class FogTripTicketComplianceManagementComponent implements OnInit {
    @ViewChild('numberTemplate', { static: true })
    public numberTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('propertyTemplate', { static: true })
    public propertyTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('mailingTemplate', { static: true })
    public mailingTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('assignedToTemplate', { static: true })
    public assignedToTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('lastTripTicketDateTemplate', { static: true })
    public lastTripTicketDateTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('intervalTemplate', { static: true })
    public intervalTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('dueDateTemplate', { static: true })
    public dueDateTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('daysOverdueTemplate', { static: true })
    public daysOverdueTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('viewSiteTemplate', { static: true })
    public viewSiteTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('printableSection')
    private _printableSection!: ElementRef;

    public canModify: boolean = false;
    public daysOverdueLabel: string = 'All overdue';

    public daysOverdue: string = '0';
    public sortBy: string = '0';
    public propertyStreet: string = '';
    public showMailing: boolean = false;
    public emailWhenAssigned: boolean = false;

    public users: WaterSupplierUser[] = [];
    public userOptions: InputOption[] = [{ id: '', text: 'Any account' }];

    public daysOverdueOptions: InputOption[] = [
        { id: '0', text: 'All overdue' },
        { id: '1', text: 'Greater than 1 month' },
        { id: '2', text: 'Greater than 2 months' },
        { id: '3', text: 'Greater than 3 months' },
        { id: '4', text: 'Greater than 6 months' },
        { id: '5', text: 'Greater than 9 months' },
        { id: '6', text: 'Greater than 1 year' },
        { id: '7', text: 'Less than 1 month' },
        { id: '8', text: 'Less than 2 months' },
        { id: '9', text: 'Less than 3 months' },
        { id: '10', text: 'Less than 6 months' },
        { id: '11', text: 'Less than 9 months' },
        { id: '12', text: 'Less than 1 year' }
    ];

    public sortOptions: InputOption[] = [
        { id: '0', text: 'Most to least overdue' },
        { id: '1', text: 'Least to most overdue' }
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

    public table: TableViewModel<Site> = {
        columns: [],
        query: { sort: { dueDate: 'Asc' }, filter: [] }
    };

    public downloadConfig: DownloadConfig;

    private panelFilters: QueryProperty[] = [];
    private readonly today: number = new Date().setHours(0, 0, 0, 0);

    constructor(
        private readonly _siteService: SiteService,
        private readonly _userService: UserService,
        private readonly _authService: AuthService,
        private readonly _downloadService: DownloadService,
        private readonly _printService: PrintableTableService,
        private readonly _containerHelper: AppContainerHelperService
    ) {
        this.downloadConfig = {
            fileName: 'FOG Trip Ticket Compliance',
            endpoint: this._siteService.getFogTripTicketComplianceEndpoint(),
            suppoertedFormats: ['CSV', 'Excel'],
            columns: [
                { field: 'accountNumber', caption: 'AccountNumber' },
                { field: 'businessName', caption: 'PropertyBusinessName' },
                { field: 'streetNumber', caption: 'PropertyStreetNumber' },
                { field: 'streetName', caption: 'PropertyStreetName' },
                { field: 'propertyNumber', caption: 'PropertyNumber' },
                { field: 'city', caption: 'PropertyCity' },
                { field: 'state.code', caption: 'PropertyState' },
                { field: 'zipCode', caption: 'PropertyZIP' },
                { field: 'mailingCompanyName', caption: 'MailingCompanyName' },
                { field: 'mailingContactName', caption: 'MailingContactName' },
                { field: 'mailingStreetNumber', caption: 'MailingStreetNumber' },
                { field: 'mailingStreetName', caption: 'MailingStreetName' },
                { field: 'mailingCity', caption: 'MailingCity' },
                { field: 'mailingState.code', caption: 'MailingState' },
                { field: 'mailingZipCode', caption: 'MailingZIP' },
                { field: 'lastTripTicketDate', caption: 'LastTripTicketDate' },
                { field: 'tripTicketInterval', caption: 'TripTicketInterval' }
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
        this.daysOverdueLabel = this.buildDaysOverdueLabel();

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
            this.table.items = await this._siteService.getFogTripTicketCompliance(
                this.table.items?.pageInfo || {},
                this.table.query
            );

            const pageInfo = this.table.items?.pageInfo;
            const offset = ((pageInfo?.pageNumber || 1) - 1) * (pageInfo?.pageSize || 0);

            (this.table.items?.data ?? []).forEach((site, index) => {
                (site as SiteRow).rowNumber = offset + index + 1;
                this.decorate(site);
            });
        } finally {
            this.table.isLoading = false;
        }
    }

    public async onAssignmentChange(site: Site, value: string): Promise<void> {
        const userId = value ? Number(value) : null;

        await this._siteService.updateFogAssignment(site.id!, userId);
        site.fogAccountAssignmentId = userId ?? undefined;
        site.fogAccountAssignmentDate = userId ? new Date().toISOString() : undefined;
        this.decorate(site);

        if (this.emailWhenAssigned && userId) {
            this.sendAssignmentEmail(site, userId);
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

    private getColumns(): TableColumn<Site>[] {
        const columns: TableColumn<Site>[] = [
            { field: '', caption: '', type: ColumnType.other, queryColumnExcluded: true, cellTemplate: this.numberTemplate, rowCssClass: 'align-top' },
            this.templateColumn('Property Information', this.propertyTemplate),
            { field: 'accountNumber', caption: 'Account Number', type: ColumnType.text, rowCssClass: 'align-top' },
            this.logColumn(),
            this.templateColumn('Assigned To', this.assignedToTemplate),
            this.templateColumn('Last Trip Ticket Date', this.lastTripTicketDateTemplate),
            this.templateColumn('Interval', this.intervalTemplate),
            this.templateColumn('Due Date', this.dueDateTemplate),
            this.templateColumn('Days Overdue', this.daysOverdueTemplate),
            this.templateColumn('', this.viewSiteTemplate)
        ];

        if (this.showMailing) {
            columns.splice(2, 0, this.templateColumn('Mailing Information', this.mailingTemplate));
        }

        return columns;
    }

    private templateColumn(caption: string, template: TemplateRef<CellTemplateData<Site>>): TableColumn<Site> {
        return { field: '', caption, type: ColumnType.other, queryColumnExcluded: true, cellTemplate: template, rowCssClass: 'align-top' };
    }

    // Property Log rendered by the shared app cell component (via cellComponent, not cellTemplate). The cell
    // reads canModify + the site's logs off the row (decorated in getCompliance / decorate).
    private logColumn(): TableColumn<Site> {
        return { field: '', caption: 'Property Log', type: ColumnType.other, queryColumnExcluded: true, cellComponent: PropertyLogCellComponent, rowCssClass: 'align-top' };
    }

    // "dueDate" is a synthetic column name (Site has no such column — it's LastTripTicketDate +
    // TripTicketInterval days) that the backend intercepts before generic filter/sort conversion.
    private buildQuery(): Query {
        const filter = [...this.panelFilters, ...this.buildDaysOverdueFilter(), ...this.buildStreetFilter()];
        return { sort: { dueDate: this.sortBy === '0' ? 'Asc' : 'Desc' }, filter };
    }

    private buildDaysOverdueLabel(): string {
        const label = this.daysOverdueOptions.find(o => o.id === this.daysOverdue)?.text;

        if (!label) {
            return 'All overdue';
        }

        return this.daysOverdue === '0' ? label : `${label} overdue`;
    }

    private buildDaysOverdueFilter(): QueryProperty[] {
        const bucket = OVERDUE_BUCKETS[Number(this.daysOverdue)];

        if (!bucket) {
            return [];
        }

        const threshold = this.subtractFromToday(bucket.amount, bucket.unit);

        if (bucket.mode === 'greaterThan') {
            return [this.rangeFilter('dueDate', [{ value: threshold, op: 'Lte' }])];
        }

        return [this.rangeFilter('dueDate', [
            { value: threshold, op: 'Gte' },
            { value: this.toIso(new Date()), op: 'Lte' }
        ])];
    }

    private buildStreetFilter(): QueryProperty[] {
        const text = this.propertyStreet.trim();

        if (!text) {
            return [];
        }

        const match = text.match(/^(\d+)\s+(.*)$/);

        if (!match) {
            return [this.rangeFilter('streetName', [{ value: text, op: 'Ct' }])];
        }

        const filters = [this.rangeFilter('streetNumber', [{ value: match[1], op: 'Eq' }])];
        const streetName = match[2].trim();

        if (streetName) {
            filters.push(this.rangeFilter('streetName', [{ value: streetName, op: 'Ct' }]));
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

    private decorate(site: Site): void {
        const row = site as SiteRow;
        row.dueDate = this.getDueDate(site);
        row.daysOverdue = this.getDaysOverdue(row.dueDate);
        row.overdueClass = row.daysOverdue != null ? this.overdueBadgeClass(row.daysOverdue) : '';
        row.assignedName = this.assignedUserName(site);
        row.canModify = this.canModify;
    }

    private getDueDate(site: Site): Date | undefined {
        if (!site.lastTripTicketDate || !site.tripTicketInterval) {
            return undefined;
        }

        const dueDate = new Date(site.lastTripTicketDate);
        dueDate.setDate(dueDate.getDate() + site.tripTicketInterval);

        return dueDate;
    }

    private getDaysOverdue(dueDate: Date | undefined): number | undefined {
        if (!dueDate) {
            return undefined;
        }

        const due = new Date(dueDate).setHours(0, 0, 0, 0);
        const days = Math.floor((this.today - due) / DAY_MS);

        return days < 0 ? undefined : days;
    }

    private overdueBadgeClass(days: number): string {
        if (days > 90) {
            return 'bg-danger';
        }

        if (days >= 30) {
            return 'bg-warning text-dark';
        }

        if (days > 0) {
            return 'bg-warning-subtle text-dark border';
        }

        return 'bg-secondary';
    }

    private assignedUserName(site: Site): string {
        if (site.fogAccountAssignmentId == null) {
            return 'Unassigned';
        }

        const user = this.users.find(u => u.id === site.fogAccountAssignmentId);
        return user?.contactName ?? user?.emailAddress ?? '';
    }

    private sendAssignmentEmail(site: Site, userId: number): void {
        const user = this.users.find(u => u.id === userId);

        if (!user?.emailAddress) {
            return;
        }

        const cityStateZip = [site.city, site.state?.code, site.zipCode].filter(Boolean).join(' ');
        const link = `${window.location.origin}/sites/${site.id}/edit`;

        const body = `Your Envirotrax account has been assigned to the following property record:\r\n\r\n` +
            `Account #:  ${site.accountNumber ?? ''}\r\n\r\n` +
            `${site.businessName ?? ''}\r\n` +
            `${site.streetNumber ?? ''} ${site.streetName ?? ''}\r\n` +
            `${cityStateZip}\r\n\r\n` +
            `Open the property record:\r\n${link}`;

        const subject = encodeURIComponent(`FOG Trip Ticket Account Assignment - ${site.businessName || site.accountNumber || ''}`);
        window.open(`mailto:${user.emailAddress}?subject=${subject}&body=${encodeURIComponent(body)}`);
    }

    private subtractFromToday(amount: number, unit: 'month' | 'year'): string {
        const date = new Date();

        if (unit === 'month') {
            date.setMonth(date.getMonth() - amount);
        } else {
            date.setFullYear(date.getFullYear() - amount);
        }

        return this.toIso(date);
    }

    private toIso(date: Date): string {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }
}
