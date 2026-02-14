import { Component, OnInit } from "@angular/core";
import { WaterSupplier } from "../../shared/models/water-suppliers/water-supplier";
import { TableViewModel } from "../../shared/models/table-view-model";
import { ProfessionalSupplierService } from "../../shared/services/professionals/professional-supplier.service";
import { TableColumn } from "../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../shared/components/data-components/sorting-filtering/query-view-model";
import { AvailableWaterSupplier, ProfessionalWaterSupplier } from "../../shared/models/professionals/professional-water-supplier";
import { ProfesisonalService } from "../../shared/services/professionals/professional.service";
import { QueryProperty } from "../../shared/models/query";

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

    constructor(
        private readonly _professionalSupplierService: ProfessionalSupplierService,
        private readonly _professionalService: ProfesisonalService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.suppliers.columns = this.getColumns();

        await this.setSupplierFilters();
        await this.getSuppliers();
    }

    private async setSupplierFilters(): Promise<void> {
        try {
            this.suppliers.isLoading = true;
            const currentProfessional = await this._professionalService.getLoggedInProfessional();

            const queryProperty: QueryProperty = {
                children: []
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

            if (currentProfessional.hasWiseGuys) {
                queryProperty.children!.push({
                    columnName: 'hasWiseGuys',
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
                        supplier: supplier,
                        selected: mySuppliers.find(s => s.waterSupplier?.id == supplier.id)
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
}

interface ProfessionalSupplierVm {
    supplier: AvailableWaterSupplier;
    selected?: ProfessionalWaterSupplier
}