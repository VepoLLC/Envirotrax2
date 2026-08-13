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
import { DownloadColumn, DownloadConfig } from "../../../shared/models/download-config";
import { MAX_PAGE_SIZE } from "../../../shared/models/page-info";
import { PermissionAction, PermissionType } from "../../../shared/models/permission-type";
import { FacilityType } from "../../../shared/enums/facility-type.enum";
import { PropertyType } from "../../../shared/enums/property-type.enum";
import { AppContainerHelperService } from "../../../shared/services/helpers/app-contaner-helper.service";
import { PropertyLogCellComponent } from "../../../shared/components/data-components/table-cells/property-log-cell.component";

const DAY_MS = 86400000;

interface OverdueBucket {
    amount: number;
    unit: 'month' | 'year';
    mode: 'greaterThan' | 'lessThan';
}

// Indexed by the Days Overdue select value. Index 0 ("All overdue") has no bucket because it applies no
// date predicate at all — matching V1's Case 0. The server gate (NeedsFogInspection && !OutOfArea) has no
// date clause either, so that option also returns sites not yet due; the per-row Days Overdue badge is what
// distinguishes the actually-overdue ones (it renders empty when daysOverdue is undefined).
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

// A grid row: a site with an overdue FOG inspection, decorated with presentation-only fields.
type SiteRow = Site & {
    rowNumber?: number;
    daysOverdue?: number;
    overdueClass?: string;
    assignedName?: string;
    canModify?: boolean;   // surfaced to the shared property-log cell
};

@Component({
    standalone: false,
    templateUrl: './fog-inspection-compliance-management.component.html'
})
export class FogInspectionComplianceManagementComponent implements OnInit {
    // Kept as a template (rather than a plain field binding) so the column's `field` stays empty: vp-table's
    // sortColumn() emits a query change — resetting to page 1 and refetching — for ANY column with a truthy
    // field, even one flagged queryColumnExcluded. It only interpolates the pre-computed rowNumber, so it
    // does no per-row work on change detection.
    @ViewChild('numberTemplate', { static: true })
    public numberTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('propertyTemplate', { static: true })
    public propertyTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('mailingTemplate', { static: true })
    public mailingTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('assignedToTemplate', { static: true })
    public assignedToTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('daysOverdueTemplate', { static: true })
    public daysOverdueTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('viewSiteTemplate', { static: true })
    public viewSiteTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('printableSection')
    private _printableSection!: ElementRef;

    public readonly PropertyType = PropertyType;

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
        query: { sort: { fogInspectionExpirationDate: 'Asc' }, filter: [] }
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
            fileName: 'FOG Inspection Compliance',
            endpoint: this._siteService.getFogInspectionComplianceEndpoint(),
            suppoertedFormats: ['CSV', 'Excel'],
            columns: this.buildDownloadColumns()
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
        this.downloadConfig.columns = this.buildDownloadColumns();
        this._downloadService.showDownloadManager(this.downloadConfig, this.table.query);
    }

    // V1 gates the mailing block of the export on the same "Show mailing information" checkbox that controls
    // the grid column, so the export is rebuilt at click time rather than fixed in the constructor.
    // PropertyLog / AccountAssignment / DaysOverdue are in V1's export but omitted here (as on the sibling
    // pages): the first two would emit a raw log collection and a numeric user id, and DaysOverdue is
    // computed client-side, so none of them resolve server-side.
    private buildDownloadColumns(): DownloadColumn[] {
        const columns: DownloadColumn[] = [
            { field: 'accountNumber', caption: 'AccountNumber' },
            { field: 'propertyType', caption: 'PropertyType' },
            { field: 'businessName', caption: 'PropertyBusinessName' },
            { field: 'streetNumber', caption: 'PropertyStreetNumber' },
            { field: 'streetName', caption: 'PropertyStreetName' },
            { field: 'propertyNumber', caption: 'PropertyNumber' },
            { field: 'city', caption: 'PropertyCity' },
            { field: 'state.code', caption: 'PropertyState' },
            { field: 'zipCode', caption: 'PropertyZIP' }
        ];

        if (this.showMailing) {
            columns.push(
                { field: 'mailingCompanyName', caption: 'MailingCompanyName' },
                { field: 'mailingContactName', caption: 'MailingContactName' },
                { field: 'mailingStreetNumber', caption: 'MailingStreetNumber' },
                { field: 'mailingStreetName', caption: 'MailingStreetName' },
                { field: 'mailingNumber', caption: 'MailingNumber' },
                { field: 'mailingCity', caption: 'MailingCity' },
                { field: 'mailingState.code', caption: 'MailingState' },
                { field: 'mailingZipCode', caption: 'MailingZIP' },
                { field: 'mailingPhoneNumber', caption: 'MailingPhoneNumber' },
                { field: 'mailingEmailAddress', caption: 'MailingEmailAddress' }
            );
        }

        columns.push({ field: 'fogInspectionExpirationDate', caption: 'InspectionDate' });

        return columns;
    }

    public viewPrintableTable(): void {
        this._printService.open(this._printableSection.nativeElement);
    }

    public async getCompliance(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._siteService.getFogInspectionCompliance(
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
        // The inspection date binds straight to the field — vp-table's ColumnType.date renders it exactly as
        // the sibling pages' `| date` template did, and keeps the header sortable on a real DTO column.
        const columns: TableColumn<Site>[] = [
            this.templateColumn('', this.numberTemplate),
            this.templateColumn('Property Information', this.propertyTemplate),
            { field: 'accountNumber', caption: 'Account Number', type: ColumnType.text, rowCssClass: 'align-top' },
            this.logColumn(),
            this.templateColumn('Assigned To', this.assignedToTemplate),
            { field: 'fogInspectionExpirationDate', caption: 'Inspection Date', type: ColumnType.date, rowCssClass: 'align-top' },
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

    private buildQuery(): Query {
        const filter = [...this.panelFilters, ...this.buildDaysOverdueFilter(), ...this.buildStreetFilter()];
        return { sort: { fogInspectionExpirationDate: this.sortBy === '0' ? 'Asc' : 'Desc' }, filter };
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
            return [this.rangeFilter('fogInspectionExpirationDate', [{ value: threshold, op: 'Lte' }])];
        }

        return [this.rangeFilter('fogInspectionExpirationDate', [
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
        row.daysOverdue = this.getDaysOverdue(site);
        row.overdueClass = row.daysOverdue != null ? this.overdueBadgeClass(row.daysOverdue) : '';
        row.assignedName = this.assignedUserName(site);
        row.canModify = this.canModify;
    }

    // Deliberate deviation from the "compute date-based status on the backend" convention: both sibling
    // compliance pages (CSI, Backflow) derive their overdue count and badge class client-side in decorate(),
    // and three near-identical grids disagreeing would be worse than one shared deviation. Moving this to a
    // DTO enum + int is a cross-page change to make on all three at once (and would also make DaysOverdue
    // exportable — see buildDownloadColumns). Computed once per load, never from the template.
    private getDaysOverdue(site: Site): number | undefined {
        if (!site.fogInspectionExpirationDate) {
            return undefined;
        }

        const expiration = new Date(site.fogInspectionExpirationDate).setHours(0, 0, 0, 0);
        const days = Math.floor((this.today - expiration) / DAY_MS);

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

        const subject = encodeURIComponent(`FOG Account Assignment - ${site.businessName || site.accountNumber || ''}`);
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
