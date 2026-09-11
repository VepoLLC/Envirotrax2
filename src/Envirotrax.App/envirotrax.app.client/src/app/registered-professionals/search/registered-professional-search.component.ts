import { Component, OnDestroy, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { NgForm } from "@angular/forms";
import { combineLatest, Subscription } from "rxjs";
import { CellTemplateData, ColumnType, InputOption, TableColumn } from "@envirotrax/common-ui";
import { TableViewModel } from "../../shared/models/table-view-model";
import { QueryProperty } from "../../shared/models/query";
import { AppContainerHelperService } from "../../shared/services/helpers/app-contaner-helper.service";
import { TitleHelperService } from "../../shared/services/helpers/title/title-helper.service";
import { RegisteredProfessionalService } from "../../shared/services/professionals/registered-professional.service";
import {
    DEFAULT_REGISTERED_PROFESSIONAL_PAGE_TITLE,
    REGISTERED_PROFESSIONAL_ACCOUNT_TYPES,
    RegisteredProfessional,
    RegisteredProfessionalAccountType
} from "../../shared/models/professionals/registered-professional";

/**
 * Row of the results grid. Every displayed value is pre-computed here so the cell templates bind
 * plain fields instead of calling component methods once per row per change-detection cycle.
 */
interface RegisteredProfessionalVm {
    hasFireLicense: boolean;
    companyName: string;
    contactName: string;
    registeredDate?: string;
    address: string;
    cityStateZip: string;
    workNumber: string;
    cellNumber: string;
    faxNumber: string;
    emailAddress: string;
    websiteUrl: string;
    websiteHref: string;
}

/**
 * Public directory of professionals registered with a water supplier — the V2 home of V1's
 * registrations.aspx. The marketing site links straight here, so the page must work signed out.
 */
@Component({
    standalone: false,
    templateUrl: './registered-professional-search.component.html'
})
export class RegisteredProfessionalSearchComponent implements OnInit, OnDestroy {
    public accountType: RegisteredProfessionalAccountType | null = null;
    public isAccountTypeFixed: boolean = false;
    public accountTypeSlug: string = '';
    public pageTitle: string = DEFAULT_REGISTERED_PROFESSIONAL_PAGE_TITLE;

    public waterSupplierOptions: InputOption[] = [];
    public selectedWaterSupplierId: string = '';
    public selectedWaterSupplierName: string = '';

    public showResults: boolean = false;
    public searchAttempted: boolean = false;
    public isSuppliersLoading: boolean = false;

    public readonly accountTypeOptions: InputOption[] = [
        { id: '', text: 'Select an account type' },
        ...REGISTERED_PROFESSIONAL_ACCOUNT_TYPES.map(type => ({ id: type.slug, text: type.name }))
    ];

    public table: TableViewModel<RegisteredProfessionalVm> = {
        query: {
            sort: {},
            filter: []
        }
    };

    @ViewChild('fireLicenseCell', { static: true })
    public fireLicenseCell?: TemplateRef<CellTemplateData<RegisteredProfessionalVm>>;

    @ViewChild('companyCell', { static: true })
    public companyCell?: TemplateRef<CellTemplateData<RegisteredProfessionalVm>>;

    @ViewChild('addressCell', { static: true })
    public addressCell?: TemplateRef<CellTemplateData<RegisteredProfessionalVm>>;

    @ViewChild('contactCell', { static: true })
    public contactCell?: TemplateRef<CellTemplateData<RegisteredProfessionalVm>>;

    private _routeSubscription?: Subscription;

    constructor(
        private readonly _registeredProfessionalService: RegisteredProfessionalService,
        private readonly _containerHelper: AppContainerHelperService,
        private readonly _titleHelper: TitleHelperService,
        private readonly _activatedRoute: ActivatedRoute
    ) {

    }

    public ngOnInit(): void {
        this._routeSubscription = combineLatest([
            this._activatedRoute.paramMap,
            this._activatedRoute.queryParamMap
        ]).subscribe(async ([params, queryParams]) => {
            const slug = params.get('accountType') ?? '';
            const requestedSupplierId = queryParams.get('waterSupplierId') ?? '';

            // The picker is hidden only when the URL names a directory we publish. An unrecognised
            // segment falls back to the generic page instead of a form the visitor cannot complete.
            this.isAccountTypeFixed = RegisteredProfessionalSearchComponent.findAccountType(slug) !== null;

            await this.setAccountType(slug);

            // V1 accepted registrations.aspx?at=1&wid=42 so a water supplier could deep link its own
            // directory. The picker stays visible here so the visitor can still change water systems.
            if (this.accountType && requestedSupplierId && this.hasWaterSupplier(requestedSupplierId)) {
                this.selectedWaterSupplierId = requestedSupplierId;
                await this.search();
            }
        });
    }

    public ngOnDestroy(): void {
        this._routeSubscription?.unsubscribe();
    }

    public async onAccountTypeChange(slug: string): Promise<void> {
        await this.setAccountType(slug);
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        // The water supplier scopes the whole directory rather than filtering a column of it, so it
        // travels as its own query string parameter and must not reach the backend filter.
        this.selectedWaterSupplierId = queryProperties
            .find(property => property.columnName === 'waterSupplierId')?.value ?? '';

        this.table.query.filter = queryProperties.filter(property => property.columnName !== 'waterSupplierId');
    }

    public async search(searchForm?: NgForm): Promise<void> {
        if (searchForm) {
            searchForm.form.markAllAsTouched();

            if (!searchForm.valid) {
                return;
            }
        }

        if (!this.accountType || !this.selectedWaterSupplierId) {
            return;
        }

        this.selectedWaterSupplierName = this.waterSupplierOptions
            .find(option => option.id === this.selectedWaterSupplierId)?.text ?? '';

        // A new search starts at the first page even when the previous one was left on page 5.
        if (this.table.items) {
            this.table.items.pageInfo = { ...this.table.items.pageInfo, pageNumber: 1 };
        }

        await this.getProfessionals();

        this.searchAttempted = true;
        this.setShowResults(true);
    }

    public searchAgain(): void {
        this.searchAttempted = false;
        this.setShowResults(false);
    }

    public async getProfessionals(): Promise<void> {
        try {
            this.table.isLoading = true;

            const results = await this._registeredProfessionalService.search(
                Number(this.selectedWaterSupplierId),
                this.accountType!.professionalType,
                this.table.items?.pageInfo || {},
                this.table.query
            );

            this.table.items = {
                pageInfo: results.pageInfo,
                data: results.data.map(professional => this.buildViewModel(professional))
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    private async setAccountType(slug: string): Promise<void> {
        this.accountType = RegisteredProfessionalSearchComponent.findAccountType(slug);
        this.accountTypeSlug = this.accountType?.slug ?? '';
        this.pageTitle = this.accountType?.pageTitle ?? DEFAULT_REGISTERED_PROFESSIONAL_PAGE_TITLE;

        this._titleHelper.setTitle(this.pageTitle);

        this.selectedWaterSupplierId = '';
        this.selectedWaterSupplierName = '';
        this.waterSupplierOptions = [];
        this.table.items = undefined;
        this.table.columns = this.getColumns();
        this.searchAttempted = false;
        this.setShowResults(false);

        if (this.accountType) {
            await this.loadWaterSuppliers();
        }
    }

    private async loadWaterSuppliers(): Promise<void> {
        try {
            this.isSuppliersLoading = true;
            this.waterSupplierOptions = await this._registeredProfessionalService
                .getWaterSupplierOptions(this.accountType!.professionalType);
        } finally {
            this.isSuppliersLoading = false;
        }
    }

    private hasWaterSupplier(waterSupplierId: string): boolean {
        return this.waterSupplierOptions.some(option => option.id === waterSupplierId);
    }

    private setShowResults(visible: boolean): void {
        this.showResults = visible;
        this._containerHelper.setContainerVisibility(!visible);
    }

    private getColumns(): TableColumn<RegisteredProfessionalVm>[] {
        const columns: TableColumn<RegisteredProfessionalVm>[] = [];

        if (this.accountType?.showFireLicense) {
            columns.push({
                // Presentation only — an empty field keeps the header from becoming a sort trigger.
                field: '',
                caption: '',
                type: ColumnType.other,
                cellTemplate: this.fireLicenseCell,
                queryColumnExcluded: true
            });
        }

        columns.push(
            {
                field: 'companyName',
                caption: 'Company/Contact Name',
                type: ColumnType.other,
                cellTemplate: this.companyCell
            },
            {
                field: 'address',
                caption: 'Address',
                type: ColumnType.other,
                cellTemplate: this.addressCell
            },
            {
                field: '',
                caption: 'Contact Information',
                type: ColumnType.other,
                cellTemplate: this.contactCell,
                queryColumnExcluded: true
            }
        );

        return columns;
    }

    private buildViewModel(professional: RegisteredProfessional): RegisteredProfessionalVm {
        const cityStateZip = [
            [professional.city, professional.state].filter(part => !!part).join(', '),
            professional.zipCode
        ].filter(part => !!part).join(' ');

        return {
            hasFireLicense: !!professional.hasFireLicense,
            companyName: professional.companyName ?? '',
            contactName: professional.contactName ?? '',
            registeredDate: professional.registeredDate,
            address: professional.address ?? '',
            cityStateZip: cityStateZip,
            workNumber: professional.workNumber ?? '',
            cellNumber: professional.cellNumber ?? '',
            faxNumber: professional.faxNumber ?? '',
            emailAddress: professional.emailAddress ?? '',
            websiteUrl: professional.websiteUrl ?? '',
            websiteHref: this.buildWebsiteHref(professional.websiteUrl)
        };
    }

    private static findAccountType(slug: string): RegisteredProfessionalAccountType | null {
        return REGISTERED_PROFESSIONAL_ACCOUNT_TYPES.find(type => type.slug === slug) ?? null;
    }

    private buildWebsiteHref(websiteUrl?: string): string {
        const url = websiteUrl?.trim() ?? '';

        if (!url) {
            return '';
        }

        return /^https?:\/\//i.test(url)
            ? url
            : `https://${url}`;
    }
}
