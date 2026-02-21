import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { TableViewModel } from "../../shared/models/table-view-model";
import { ProfessionalSupplierService } from "../../shared/services/professionals/professional-supplier.service";
import { CellTemplateData, TableColumn, TableCustomAction } from "../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../shared/components/data-components/sorting-filtering/query-view-model";
import { AvailableWaterSupplier, ProfessionalWaterSupplier } from "../../shared/models/professionals/professional-water-supplier";
import { ProfesisonalService } from "../../shared/services/professionals/professional.service";
import { QueryProperty } from "../../shared/models/query";
import { State } from "../../shared/models/states/state";
import { LookupService } from "../../shared/services/lookup/lookup.service";
import { Professional } from "../../shared/models/professionals/professional";
import { InputOption } from "../../shared/components/input/input.component";
import { ModalHelperService } from "../../shared/services/helpers/modal-helper.service";
import { WaterSupplierRegistrationComponent, WaterSupplierRegistrationVm } from "./registration/water-supplier-registration.component";

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
    `
})
export class WaterSuppliersComponent implements OnInit {
    private readonly _stateQuuery: QueryProperty = {
        columnName: 'stateId'
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

    public readonly supplierActions: TableCustomAction<ProfessionalSupplierVm>[] = [
        {
            text: 'Register',
            iconClass: 'fa-solid fa-pen-to-square',
            action: (supplier) => this.openRegistration(supplier)
        }
    ];

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
        private readonly _modalHelper: ModalHelperService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.suppliers.query.filter?.push(this._stateQuuery);
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

            this.states = states;
            this.professional = currentProfessional;

            this.stateId = states.find(s => s.data?.code == 'TX')?.id;
            this._stateQuuery.value = this.stateId?.toString();

            const queryProperty: QueryProperty = {
                children: [],
                columnName: 'id'
            };

            if (currentProfessional.hasBackflowTesting) {
                queryProperty.children!.push({
                    columnName: 'hasBackflowTesting',
                    value: 'true',
                    logicalOperator: 'Or'
                });
            }

            if (currentProfessional.hasCsiInspection) {
                queryProperty.children!.push({
                    columnName: 'hasCsiInspection',
                    value: 'true',
                    logicalOperator: 'Or'
                });
            }

            if (currentProfessional.hasFogInspection) {
                queryProperty.children!.push({
                    columnName: 'hasFogInspection',
                    value: 'true',
                    logicalOperator: 'Or'
                });
            }

            if (currentProfessional.hasFogTransportation) {
                queryProperty.children!.push({
                    columnName: 'hasFogTransportation',
                    value: 'true',
                    logicalOperator: 'Or'
                });
            }

            this.suppliers.query.filter?.push(queryProperty);
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
        } finally {
            this.suppliers.isLoading = false;
        }

        await this.getSuppliers();
    }

    private getColumns(): TableColumn<ProfessionalSupplierVm>[] {
        return [
            {
                field: 'name',
                caption: 'Water Supplier',
                type: ColumnType.text
            },
            {
                field: 'Supported Programs',
                caption: 'Supported Programs',
                type: ColumnType.other,
                cellTemplate: this.supportedPrograms,
                queryColumnExcluded: true
            },
            {
                field: 'BPAT Requirements',
                caption: 'BPAT Requirements',
                type: ColumnType.other,
                cellTemplate: this.bpatRequirements,
                queryColumnExcluded: true
            },
            {
                field: 'CSI Requirements',
                caption: 'CSI Requirements',
                type: ColumnType.other,
                cellTemplate: this.csiRequirements,
                queryColumnExcluded: true
            },
            {
                field: 'FOG Requirements',
                caption: 'FOG Requirements',
                type: ColumnType.other,
                cellTemplate: this.fogRequirements,
                queryColumnExcluded: true
            },
            {
                field: 'Selected Programs',
                caption: 'Selected Programs',
                type: ColumnType.other,
                cellTemplate: this.selectedPrograms,
                queryColumnExcluded: true
            },
        ]
    }

    public stateChanged(): void {
        this._stateQuuery.value = this.stateId?.toString();
        this.getSuppliers();
    }
}

interface ProfessionalSupplierVm extends AvailableWaterSupplier {
    selected?: ProfessionalWaterSupplier
}
