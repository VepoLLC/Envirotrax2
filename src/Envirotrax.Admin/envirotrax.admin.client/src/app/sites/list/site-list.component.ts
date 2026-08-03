import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { CellTemplateData, ColumnType, InputOption, MAX_PAGE_SIZE, QueryProperty, TableColumn, TableViewModel } from '@envirotrax/common-ui';
import { FogCompliancyStatus, PropertyType, Site } from '../../shared/models/sites/site';
import { SiteEditWindowModel } from '../../shared/models/sites/site-detail';
import { SiteService } from '../../shared/services/sites/site.service';
import { WaterSupplierService } from '../../shared/services/water-suppliers/water-supplier.service';
import { WindowService } from '../../shared/services/window.service';
import { SiteEditComponent } from '../edit/site-edit.component';

@Component({
    templateUrl: './site-list.component.html',
    standalone: false,
})
export class SiteListComponent implements OnInit {
    @ViewChild('statusCell', { static: true })
    public statusCell?: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('propertyTypeCell', { static: true })
    public propertyTypeCell?: TemplateRef<CellTemplateData<Site>>;

    public readonly propertyType = PropertyType;

    public showResults: boolean = false;

    private fogCompliancyStatus: FogCompliancyStatus | null = null;

    public table: TableViewModel<Site> = {
        query: {
            sort: {},
            filter: []
        }
    };

    public waterSupplierOptions: InputOption[] = [{ id: '', text: 'Any Value' }];

    public readonly yesNoOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: 'true', text: 'Yes' },
        { id: 'false', text: 'No' }
    ];

    public readonly facilityTypes: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: '0', text: 'Other' },
        { id: '1', text: 'Restaurant' },
        { id: '2', text: 'Fast Food Establishment' },
        { id: '3', text: 'Hotel/Motel' },
        { id: '4', text: 'Car Wash' },
        { id: '5', text: 'School/University' },
        { id: '6', text: 'Grocery Store' },
        { id: '7', text: 'Convenience Store' },
        { id: '8', text: 'Assisted Living Facility' },
        { id: '9', text: 'Medical Facility' },
        { id: '10', text: 'Industrial' },
        { id: '11', text: 'City Owned Facility' }
    ];

    public readonly greaseTrapOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: '0', text: 'Trap Not Required' },
        { id: '1', text: 'Has Grease Trap' },
        { id: '2', text: 'Should Have Grease Trap' },
        { id: '3', text: 'Might Have Grease Trap' }
    ];

    public readonly propertyTypes: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: '0', text: 'Residential' },
        { id: '1', text: 'Commercial' }
    ];

    public readonly fogCompliancyStatusOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: String(FogCompliancyStatus.Compliant), text: 'Compliant' },
        { id: String(FogCompliancyStatus.OutOfCompliance), text: 'Out of Compliance' }
    ];

    constructor(
        private readonly _siteService: SiteService,
        private readonly _waterSupplierService: WaterSupplierService,
        private readonly _windowService: WindowService
    ) {

    }

    public openSite(site: Site): void {
        if (site.id == null) {
            return;
        }

        const model: SiteEditWindowModel = {
            siteId: site.id,
            waterSupplierId: site.waterSupplier?.id
        };

        this._windowService.addWindow(SiteEditComponent, {
            title: this.buildSiteWindowTitle(site),
            model
        });
    }

    // Window title matches the legacy Vepo Manager Edit Site window: "{Site ID} - {Street #} {Street Name}"
    // (e.g. "464886 - 229 County Rd 2871"). Falls back to just the id when the property street is blank.
    private buildSiteWindowTitle(site: Site): string {
        const address = [site.streetNumber, site.streetName]
            .filter(part => !!part && part.trim().length > 0)
            .join(' ');

        return address ? `${site.id} - ${address}` : `${site.id}`;
    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.loadWaterSuppliers();
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        const fogStatus = queryProperties.find(p => p.columnName === 'fogCompliancyStatus');

        this.fogCompliancyStatus = fogStatus?.value ? Number(fogStatus.value) as FogCompliancyStatus : null;

        this.table.query.filter = queryProperties.filter(p => p.columnName !== 'fogCompliancyStatus');
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }

        await this.getSites();

        this.showResults = true;
    }

    public async getSites(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._siteService.getAll(this.table.items?.pageInfo || {}, this.table.query, this.fogCompliancyStatus);
        } finally {
            this.table.isLoading = false;
        }
    }

    private async loadWaterSuppliers(): Promise<void> {
        const result = await this._waterSupplierService.getAll(
            { pageSize: MAX_PAGE_SIZE },
            { sort: { name: 'Asc' }, filter: [] }
        );

        const options: InputOption[] = (result.data ?? []).map(ws => ({ id: String(ws.id), text: ws.name }));

        this.waterSupplierOptions = [{ id: '', text: 'Any Value' }, ...options];
    }

    private getColumns(): TableColumn<Site>[] {
        return [
            { field: 'id', caption: 'ID', type: ColumnType.number },
            { field: 'status', caption: '', type: ColumnType.other, cellTemplate: this.statusCell, queryColumnExcluded: true },
            { field: 'waterSupplier.name', caption: 'Water Supplier', type: ColumnType.text },
            { field: 'propertyType', caption: 'Type', type: ColumnType.other, cellTemplate: this.propertyTypeCell, queryColumnExcluded: true },
            { field: 'businessName', caption: 'Business Name', type: ColumnType.text },
            { field: 'streetNumber', caption: 'P St #', type: ColumnType.text },
            { field: 'streetName', caption: 'P Street Name', type: ColumnType.text },
            { field: 'propertyNumber', caption: 'P #', type: ColumnType.text },
            { field: 'city', caption: 'City', type: ColumnType.text },
            { field: 'state.code', caption: 'ST', type: ColumnType.text }
        ];
    }
}
