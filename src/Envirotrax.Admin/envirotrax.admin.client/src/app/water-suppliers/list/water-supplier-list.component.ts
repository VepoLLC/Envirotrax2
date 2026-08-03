import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ColumnType, InputOption, QueryProperty, TableColumn, TableViewModel } from '@envirotrax/common-ui';
import { WaterSupplier } from '../../shared/models/water-suppliers/water-supplier';
import { WaterSupplierService } from '../../shared/services/water-suppliers/water-supplier.service';
import { LookupService } from '../../shared/services/lookup/lookup.service';
import { WindowService } from '../../shared/services/window.service';
import { WaterSupplierDetailsComponent } from '../detail/water-supplier-details.component';

@Component({
    templateUrl: './water-supplier-list.component.html',
    standalone: false,
})
export class WaterSupplierListComponent implements OnInit {
    public showResults: boolean = false;

    public isInitializing: boolean = true;

    public table: TableViewModel<WaterSupplier> = {
        query: {
            sort: {},
            filter: []
        }
    };

    public stateOptions: InputOption[] = [{ id: '', text: 'Any Value' }];

    constructor(
        private readonly _waterSupplierService: WaterSupplierService,
        private readonly _lookupService: LookupService,
        private readonly _windowService: WindowService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        try {
            await this.loadStates();
        } finally {
            this.isInitializing = false;
        }
    }

    private async loadStates(): Promise<void> {
        const states = await this._lookupService.getAllStates();

        const options: InputOption[] = states.map(state => ({ id: String(state.id), text: state.name ?? '' }));

        this.stateOptions = [{ id: '', text: 'Any Value' }, ...options];
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }

        await this.getSuppliers();

        this.showResults = true;
    }

    public viewDetails(supplier: WaterSupplier): void {
        this._windowService.addWindow(WaterSupplierDetailsComponent, {
            title: supplier.name ?? 'Water Supplier',
            model: { id: supplier.id }
        });
    }

    public async getSuppliers(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._waterSupplierService.getAll(this.table.items?.pageInfo || {}, this.table.query);
        } finally {
            this.table.isLoading = false;
        }
    }

    private getColumns(): TableColumn<WaterSupplier>[] {
        return [
            { field: 'name', caption: 'Name', type: ColumnType.text },
            { field: 'contactName', caption: 'Contact Name', type: ColumnType.text },
            { field: 'pwsId', caption: 'PWS ID', type: ColumnType.text },
            { field: 'city', caption: 'City', type: ColumnType.text },
            { field: 'state.code', caption: 'ST', type: ColumnType.text },
            { field: 'parent.name', caption: 'Parent', type: ColumnType.text }
        ];
    }
}
