import { Component, OnInit, Query } from "@angular/core";
import { WaterSupplier } from "../../shared/models/water-suppliers/water-supplier";
import { TableViewModel } from "../../shared/models/table-view-model";
import { ProfessionalSupplierService } from "../../shared/services/professionals/professional-supplier.service";
import { TableColumn } from "../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../shared/components/data-components/sorting-filtering/query-view-model";
import { AvailableWaterSupplier, ProfessionalWaterSupplier } from "../../shared/models/professionals/professional-water-supplier";
import { ProfesisonalService } from "../../shared/services/professionals/professional.service";
import { QueryProperty } from "../../shared/models/query";
import { State } from "../../shared/models/states/state";
import { LookupService } from "../../shared/services/lookup/lookup.service";
import { Professional } from "../../shared/models/professionals/professional";

@Component({
    standalone: false,
    templateUrl: './water-suppliers.component.html',
    styles: `
        .vp-requirement-icon {
            max-width: 25px;
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

    public states: State[] = [];
    public stateId?: number;
    public professional: Professional = {};

    constructor(
        private readonly _professionalSupplierService: ProfessionalSupplierService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _lookupService: LookupService
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
                this._lookupService.getAllStates()
            ]);

            this.states = states;
            this.professional = currentProfessional;

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

    private getColumns(): TableColumn<ProfessionalSupplierVm>[] {
        return [
            {
                field: 'name',
                caption: 'Water Supplier',
                type: ColumnType.text
            },
            {
                field: 'requirements',
                caption: 'Requirements',
                type: ColumnType.other
            }
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