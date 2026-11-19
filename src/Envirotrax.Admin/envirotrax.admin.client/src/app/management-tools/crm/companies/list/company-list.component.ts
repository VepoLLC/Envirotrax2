import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import {
    CellTemplateData,
    ColumnType,
    DateRange,
    FilterPanelComponent,
    InputOption,
    ModalHelperService,
    QueryProperty,
    TableColumn,
    TableViewModel,
    ToastService
} from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import {
    Company,
    CompanyProgramBadge,
    CompanyRow,
    ContactType,
    contactTypeIconClasses,
    contactTypeLabels,
    contactTypeOptions,
    NextContactDateStatus,
    nextContactDateStatusClasses
} from '../../../../shared/models/crm/company';
import { State } from '../../../../shared/models/lookup/state';
import { MODULE_COLORS } from '../../../../shared/models/module-colors';
import { CompanyService } from '../../../../shared/services/crm/company.service';
import { LookupService } from '../../../../shared/services/lookup/lookup.service';
import { WindowService } from '../../../../shared/services/window.service';
import { CompanyDetailsComponent, CompanyWindowModel } from '../details/company-details.component';

// V1 Vepo Manager's Search Contacts tab opens on the next contact dates from 180 days ago to 30 days ahead.
const DEFAULT_NEXT_CONTACT_DAYS_BACK = 180;
const DEFAULT_NEXT_CONTACT_DAYS_AHEAD = 30;

/**
 * V1 Vepo Manager, Contact Management → Search Contacts. V1 calls them contacts, but they are the companies Vepo
 * works with.
 */
@Component({
    templateUrl: './company-list.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
    providers: [DatePipe]
})
export class CompanyListComponent implements OnInit {
    @ViewChild('contactTypeCell', { static: true })
    public contactTypeCell?: TemplateRef<CellTemplateData<CompanyRow>>;

    @ViewChild('programCell', { static: true })
    public programCell?: TemplateRef<CellTemplateData<CompanyRow>>;

    @ViewChild('cityStateCell', { static: true })
    public cityStateCell?: TemplateRef<CellTemplateData<CompanyRow>>;

    @ViewChild('nextContactDateCell', { static: true })
    public nextContactDateCell?: TemplateRef<CellTemplateData<CompanyRow>>;

    @ViewChild(FilterPanelComponent, { static: true })
    private _filterPanel!: FilterPanelComponent;

    public showResults: boolean = false;

    public table: TableViewModel<CompanyRow> = {
        query: {
            sort: { nextContactDate: 'Asc' },
            filter: []
        }
    };

    public readonly contactTypeOptions: InputOption[] = [{ id: '', text: 'Any Type' }, ...contactTypeOptions];

    public readonly yesNoOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: 'true', text: 'Yes' },
        { id: 'false', text: 'No' }
    ];

    public readonly moduleColors = MODULE_COLORS;

    public defaultNextContactDateRange: DateRange = {};

    private stateCodes = new Map<number, string>();

    constructor(
        private readonly _companyService: CompanyService,
        private readonly _lookupService: LookupService,
        private readonly _windowService: WindowService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _datePipe: DatePipe
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();
        this.defaultNextContactDateRange = this.buildDefaultNextContactDateRange();

        await this.loadStates();
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }

        // The default date range is set on the field, which emits nothing until it is changed, so read the whole form.
        this.table.query.filter = this._filterPanel.getFilter();

        // A new search starts again from the first page.
        this.table.items = undefined;

        await this.getCompanies();

        this.showResults = true;
    }

    public async getCompanies(): Promise<void> {
        try {
            this.table.isLoading = true;

            const companies = await this._companyService.getAll(this.table.items?.pageInfo || {}, this.table.query);

            this.table.items = {
                pageInfo: companies.pageInfo,
                data: companies.data.map(company => this.toRow(company))
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    public addCompany(): void {
        this.openDetails({});
    }

    public viewCompany(row: CompanyRow): void {
        this.openDetails(row);
    }

    public deleteCompany(row: CompanyRow): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;

                await this._companyService.delete(row.id!);

                this._toastService.successFullyDeleted('Company');
            } finally {
                this.table.isLoading = false;
            }

            await this.getCompanies();
        });
    }

    private openDetails(company: Company): void {
        this._windowService.addWindow<CompanyDetailsComponent, CompanyWindowModel>(CompanyDetailsComponent, {
            title: company.id ? `${company.id} - ${company.name}` : 'New Company',
            model: {
                id: company.id,
                onChanged: () => this.getCompanies()
            }
        });
    }

    private buildDefaultNextContactDateRange(): DateRange {
        const today = new Date();

        const startDate = new Date(today);
        startDate.setDate(today.getDate() - DEFAULT_NEXT_CONTACT_DAYS_BACK);

        const endDate = new Date(today);
        endDate.setDate(today.getDate() + DEFAULT_NEXT_CONTACT_DAYS_AHEAD);

        return {
            startDate: this._datePipe.transform(startDate, 'yyyy-MM-dd') ?? undefined,
            endDate: this._datePipe.transform(endDate, 'yyyy-MM-dd') ?? undefined
        };
    }

    // The list carries only the state id, so the codes come from the cached states lookup.
    private async loadStates(): Promise<void> {
        const stateOptions: InputOption<State>[] = await this._lookupService.getStatesAsOptions();

        this.stateCodes = new Map(stateOptions.map(option => [option.data!.id!, option.data!.code ?? '']));
    }

    private toRow(company: Company): CompanyRow {
        const contactType = company.contactType ?? ContactType.ExistingClient;
        const stateCode = company.state?.id != null ? this.stateCodes.get(company.state.id) : undefined;

        return {
            ...company,
            contactTypeLabel: contactTypeLabels[contactType],
            contactTypeIconClass: contactTypeIconClasses[contactType],
            programs: this.buildPrograms(company),
            cityState: [company.city, stateCode].filter(part => part).join(', '),
            nextContactDateClass: nextContactDateStatusClasses[company.nextContactDateStatus ?? NextContactDateStatus.None]
        };
    }

    // V1 shows a colored square per program, filled when the company has it.
    private buildPrograms(company: Company): CompanyProgramBadge[] {
        return [
            this.buildProgram('Backflow Testing', MODULE_COLORS.BACKFLOW, company.hasBackflowTesting),
            this.buildProgram('CSI Inspection', MODULE_COLORS.CSI, company.hasCsiInspection),
            this.buildProgram('FOG Inspection', MODULE_COLORS.FOG, company.hasFogInspection),
            this.buildProgram('FOG Transportation', MODULE_COLORS.FOG, company.hasFogTransportation)
        ];
    }

    private buildProgram(title: string, color: string, isActive?: boolean): CompanyProgramBadge {
        return {
            title: `${title}: ${isActive ? 'Yes' : 'No'}`,
            color,
            fillColor: isActive ? color : 'transparent'
        };
    }

    private getColumns(): TableColumn<CompanyRow>[] {
        return [
            // An enum, so it stays out of the table's text filter (the search form has a Contact Type filter) but can still be sorted.
            { field: 'contactType', caption: 'Type', type: ColumnType.other, cellTemplate: this.contactTypeCell, queryColumnExcluded: true, sortColumnExcluded: false },
            { field: 'programs', caption: 'Program', type: ColumnType.other, cellTemplate: this.programCell, sortColumnExcluded: true, queryColumnExcluded: true },
            { field: 'name', caption: 'Name', type: ColumnType.text },
            { field: 'contactName', caption: 'Contact Name', type: ColumnType.text },
            { field: 'city', caption: 'City and State', type: ColumnType.text, cellTemplate: this.cityStateCell },
            { field: 'lastContactDate', caption: 'Last Contact Date', type: ColumnType.date },
            { field: 'nextContactDate', caption: 'Next Contact Date', type: ColumnType.date, cellTemplate: this.nextContactDateCell }
        ];
    }
}
