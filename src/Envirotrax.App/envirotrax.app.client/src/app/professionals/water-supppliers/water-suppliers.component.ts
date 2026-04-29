import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { TableViewModel } from "../../shared/models/table-view-model";
import { ProfessionalSupplierService } from "../../shared/services/professionals/professional-supplier.service";
import { CellTemplateData, TableColumn, TableCustomAction } from "../../shared/components/data-components/table/table.component";
import { CurrencyCellComponent } from "../../shared/components/data-components/table/table-cells/currency-cell.component";
import { ColumnType } from "../../shared/components/data-components/sorting-filtering/query-view-model";
import { AvailableWaterSupplier, ProfessionalWaterSupplier } from "../../shared/models/professionals/professional-water-supplier";
import { ProfesisonalService } from "../../shared/services/professionals/professional.service";
import { QueryProperty } from "../../shared/models/query";
import { State } from "../../shared/models/lookup/state";
import { LookupService } from "../../shared/services/lookup/lookup.service";
import { Professional } from "../../shared/models/professionals/professional";
import { InputOption } from "../../shared/components/input/input.component";
import { ModalHelperService } from "../../shared/services/helpers/modal-helper.service";
import { WaterSupplierRegistrationComponent, WaterSupplierRegistrationVm } from "./registration/water-supplier-registration.component";
import { ToastService } from "../../shared/services/toast.service";
import { AuthService } from "../../shared/services/auth/auth.service";
import { FeatureType } from "../../shared/models/feature-tyype";

type TabType = 'backflow' | 'csi' | 'fogInspection' | 'fogTransport';

@Component({
    standalone: false,
    templateUrl: './water-suppliers.component.html',
    styles: `
        .vp-requirement-icon {
            max-width: 25px;
        }

        .vp-filter {
            min-width: 270px;
        }

        :host ::ng-deep .vp-registration-table-column {
            min-width: 300px;
        }

        :host ::ng-deep .vp-registration-actions-column {
            min-width: 150px;
        }
    `
})
export class WaterSuppliersComponent implements OnInit {
    private readonly _stateQuuery: QueryProperty = {
        columnName: 'stateId'
    };

    private readonly _tabQuery: QueryProperty = {
        columnName: 'hasBackflowTesting',
        value: 'true'
    };

    public suppliers: TableViewModel<ProfessionalSupplierVm> = {
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'name', operator: 'Ct', placeholder: 'Water Supplier Name' }
            ]
        }
    };

    public states: InputOption<State>[] = [];
    public stateId?: number;
    public professional: Professional = {};

    public activeTab: TabType = 'backflow';
    public hasBackflowTesting = false;
    public hasCsiInspection = false;
    public hasFogInspection = false;
    public hasFogTransportation = false;

    @ViewChild('supportedPrograms', { static: true })
    public supportedPrograms?: TemplateRef<CellTemplateData<AvailableWaterSupplier>>;

    @ViewChild('selectedPrograms', { static: true })
    public selectedPrograms?: TemplateRef<CellTemplateData<ProfessionalSupplierVm>>;

    @ViewChild('bpatRequirements', { static: true })
    public bpatRequirements?: TemplateRef<CellTemplateData<AvailableWaterSupplier>>;

    @ViewChild('csiRequirements', { static: true })
    public csiRequirements?: TemplateRef<CellTemplateData<AvailableWaterSupplier>>;

    @ViewChild('fogRequirements', { static: true })
    public fogRequirements?: TemplateRef<CellTemplateData<AvailableWaterSupplier>>;

    constructor(
        private readonly _professionalSupplierService: ProfessionalSupplierService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _lookupService: LookupService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _authService: AuthService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        [this.hasBackflowTesting, this.hasCsiInspection, this.hasFogInspection, this.hasFogTransportation] = await Promise.all([
            this._authService.hasAnyFeatures(FeatureType.BackflowTesting),
            this._authService.hasAnyFeatures(FeatureType.CsiInspection),
            this._authService.hasAnyFeatures(FeatureType.FogInspection),
            this._authService.hasAnyFeatures(FeatureType.FogTransportation)
        ]);

        if (this.hasBackflowTesting) this.activeTab = 'backflow';
        else if (this.hasCsiInspection) this.activeTab = 'csi';
        else if (this.hasFogInspection) this.activeTab = 'fogInspection';
        else if (this.hasFogTransportation) this.activeTab = 'fogTransport';

        this._tabQuery.columnName = this._getTabColumnName();

        this.suppliers.query.filter?.push(this._stateQuuery);
        this.suppliers.query.filter?.push(this._tabQuery);
        this.suppliers.columns = this.getColumns();

        await this.setSupplierFilters();
        await this.getSuppliers();
    }

    private async setSupplierFilters(): Promise<void> {
        try {
            this.suppliers.isLoading = true;

            const [currentProfessional, states] = await Promise.all([
                this._professionalService.getLoggedInProfessional(),
                this._lookupService.getAllStatesAsOptions(true)
            ]);

            this.states = states.filter(s => s.data?.code === 'TX' || s.data?.code === 'KS' || s.data?.code === 'WA');
            this.professional = currentProfessional;

            this.stateId = states.find(s => s.data?.code == 'TX')?.id;
            this._stateQuuery.value = this.stateId?.toString();
        } finally {
            this.suppliers.isLoading = false;
        }
    }

    public async getSuppliers(): Promise<void> {
        try {
            this.suppliers.isLoading = true;

            const allSuppliers = await this._professionalSupplierService.getAllAvailableSuppliers(this.suppliers.items?.pageInfo || {}, this.suppliers.query);
            const mySuppliers = await this._professionalSupplierService.getAllMy();

            this.suppliers.items = {
                pageInfo: allSuppliers.pageInfo,
                data: allSuppliers
                    .data
                    .map(supplier => ({
                        ...supplier,
                        selected: mySuppliers.data.find(s => s.waterSupplier?.id == supplier.id)
                    }))
            };
        } finally {
            this.suppliers.isLoading = false;
        }
    }

    public setActiveTab(tab: TabType): void {
        this.activeTab = tab;
        this._tabQuery.columnName = this._getTabColumnName();
        this.suppliers.columns = this.getColumns();
        this.getSuppliers();
    }

    public openRegistration(supplier: ProfessionalSupplierVm): void {
        const model: WaterSupplierRegistrationVm = {
            availableSupplier: supplier,
            selected: supplier.selected
        };

        this._modalHelper.show<WaterSupplierRegistrationVm, ProfessionalWaterSupplier>(WaterSupplierRegistrationComponent, {
            title: `Register with ${supplier.name}`,
            model
        }).result()
            .subscribe(() => this.getSuppliers());
    }

    public deleteRegistration(supplier: ProfessionalWaterSupplier): void {
        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(() => this.processDelete(supplier));
    }

    private async processDelete(supplier: ProfessionalSupplierVm): Promise<void> {
        try {
            this.suppliers.isLoading = true;
            await this._professionalSupplierService.delete(supplier.selected!.waterSupplier!.id!);

            this._toastService.successFullyDeleted('Registration');
        } finally {
            this.suppliers.isLoading = false;
        }

        await this.getSuppliers();
    }

    private _getTabColumnName(): string {
        switch (this.activeTab) {
            case 'backflow': return 'hasBackflowTesting';
            case 'csi': return 'hasCsiInspection';
            case 'fogInspection': return 'hasFogInspection';
            case 'fogTransport': return 'hasFogTransportation';
        }
    }

    private getColumns(): TableColumn<ProfessionalSupplierVm>[] {
        const baseColumns: TableColumn<ProfessionalSupplierVm>[] = [
            {
                field: 'Actions',
                caption: 'Actions',
                type: ColumnType.other,
                cellTemplate: this.selectedPrograms,
                queryColumnExcluded: true,
                headerCssClass: 'vp-registration-actions-column text-center',
                rowCssClass: 'vp-registration-actions-column'
            },
            {
                field: 'name',
                caption: 'Water Supplier',
                type: ColumnType.text,
                headerCssClass: 'vp-registration-table-column text-center',
                rowCssClass: 'vp-registration-table-column text-center'
            },
            {
                field: 'Supported Programs',
                caption: 'Supported Programs',
                type: ColumnType.other,
                cellTemplate: this.supportedPrograms,
                queryColumnExcluded: true,
                headerCssClass: 'vp-registration-table-column text-center',
                rowCssClass: 'vp-registration-table-column text-center'
            }
        ];

        switch (this.activeTab) {
            case 'backflow':
                return [
                    ...baseColumns,
                    {
                        field: 'BPAT Requirements',
                        caption: 'BPAT Requirements',
                        type: ColumnType.other,
                        cellTemplate: this.bpatRequirements,
                        queryColumnExcluded: true,
                        headerCssClass: 'vp-registration-table-column text-center',
                        rowCssClass: 'vp-registration-table-column text-center'
                    },
                    {
                        field: 'backflowResidentialTestFee',
                        caption: 'BPAT Residential Fee',
                        type: ColumnType.number,
                        cellComponent: CurrencyCellComponent,
                        headerCssClass: 'vp-registration-table-column text-center',
                        rowCssClass: 'vp-registration-table-column text-center'
                    },
                    {
                        field: 'backflowCommercialTestFee',
                        caption: 'BPAT Commercial Fee',
                        type: ColumnType.number,
                        cellComponent: CurrencyCellComponent,
                        headerCssClass: 'vp-registration-table-column text-center',
                        rowCssClass: 'vp-registration-table-column text-center'
                    }
                ];

            case 'csi':
                return [
                    ...baseColumns,
                    {
                        field: 'CSI Requirements',
                        caption: 'CSI Requirements',
                        type: ColumnType.other,
                        cellTemplate: this.csiRequirements,
                        queryColumnExcluded: true,
                        headerCssClass: 'vp-registration-table-column text-center',
                        rowCssClass: 'vp-registration-table-column text-center'
                    },
                    {
                        field: 'csiResidentialInspectionFee',
                        caption: 'CSI Residential Fee',
                        type: ColumnType.number,
                        cellComponent: CurrencyCellComponent,
                        headerCssClass: 'vp-registration-table-column text-center',
                        rowCssClass: 'vp-registration-table-column text-center'
                    },
                    {
                        field: 'csiCommercialInspectionFee',
                        caption: 'CSI Commercial Fee',
                        type: ColumnType.number,
                        cellComponent: CurrencyCellComponent,
                        headerCssClass: 'vp-registration-table-column text-center',
                        rowCssClass: 'vp-registration-table-column text-center'
                    }
                ];

            case 'fogInspection':
                return [
                    ...baseColumns,
                    {
                        field: 'FOG Requirements',
                        caption: 'FOG Requirements',
                        type: ColumnType.other,
                        cellTemplate: this.fogRequirements,
                        queryColumnExcluded: true,
                        headerCssClass: 'vp-registration-table-column text-center',
                        rowCssClass: 'vp-registration-table-column text-center'
                    },
                    {
                        field: 'fogInspectorFee',
                        caption: 'FOG Inspection Fee',
                        type: ColumnType.number,
                        cellComponent: CurrencyCellComponent,
                        headerCssClass: 'vp-registration-table-column text-center',
                        rowCssClass: 'vp-registration-table-column text-center'
                    }
                ];

            case 'fogTransport':
                return [
                    ...baseColumns,
                    {
                        field: 'FOG Requirements',
                        caption: 'FOG Requirements',
                        type: ColumnType.other,
                        cellTemplate: this.fogRequirements,
                        queryColumnExcluded: true,
                        headerCssClass: 'vp-registration-table-column text-center',
                        rowCssClass: 'vp-registration-table-column text-center'
                    },
                    {
                        field: 'fogTransportFee',
                        caption: 'FOG Transport Fee',
                        type: ColumnType.number,
                        cellComponent: CurrencyCellComponent,
                        headerCssClass: 'vp-registration-table-column text-center',
                        rowCssClass: 'vp-registration-table-column text-center'
                    }
                ];
        }
    }

    public stateChanged(): void {
        this._stateQuuery.value = this.stateId?.toString();
        this.getSuppliers();
    }
}

interface ProfessionalSupplierVm extends AvailableWaterSupplier {
    selected?: ProfessionalWaterSupplier
}
