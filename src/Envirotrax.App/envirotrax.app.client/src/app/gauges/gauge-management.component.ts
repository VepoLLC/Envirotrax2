import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { CellTemplateData, ColumnType, TableColumn } from '@envirotrax/common-ui';
import { WaterSupplierGauge } from '../shared/models/backflow/water-supplier-gauge';
import { WaterSupplierGaugeService } from '../shared/services/backflow/water-supplier-gauge.service';
import { TableViewModel } from '../shared/models/table-view-model';
import { AuthService } from '../shared/services/auth/auth.service';
import { FeatureType } from '../shared/models/feature-type';
import { PermissionAction, PermissionType } from '../shared/models/permission-type';

@Component({
    templateUrl: './gauge-management.component.html',
    standalone: false
})
export class GaugeManagementComponent implements OnInit {
    @ViewChild('gaugeCell', { static: true })
    private gaugeCell!: TemplateRef<CellTemplateData<WaterSupplierGauge>>;

    @ViewChild('typeCell', { static: true })
    private typeCell!: TemplateRef<CellTemplateData<WaterSupplierGauge>>;

    @ViewChild('actionsCell', { static: true })
    private actionsCell!: TemplateRef<CellTemplateData<WaterSupplierGauge>>;

    public canManage = false;

    public table: TableViewModel<WaterSupplierGauge> = {
        columns: [],
        query: { sort: {}, filter: [] },
        freeTextSearch: {
            searchQuery: [
                { field: 'serialNumber' }
            ]
        }
    };

    constructor(
        private readonly _gaugeService: WaterSupplierGaugeService,
        private readonly _authService: AuthService,
        private readonly _router: Router
    ) { }

    public async ngOnInit(): Promise<void> {
        const hasGaugeAccess = await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalLicenses)
            || await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.Licenses);

        if (!hasGaugeAccess) {
            await this._router.navigate(['auth', 'unauthorized']);
            return;
        }

        // Manage opens the BPAT details page, which is guarded separately.
        this.canManage = await this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowTesters);

        this.table.columns = this.getColumns();

        await this.loadGauges();
    }

    private getColumns(): TableColumn<WaterSupplierGauge>[] {
        const cols: TableColumn<WaterSupplierGauge>[] = [
            { field: 'submittedOn', caption: 'Submitted On', type: ColumnType.date },
            { field: 'userEmail', caption: 'User ID', type: ColumnType.text },
            { field: 'companyName', caption: 'Company Name', type: ColumnType.text },
            { field: 'manufacturer', caption: 'Gauge', type: ColumnType.text, cellTemplate: this.gaugeCell },
            { field: 'isPortable', caption: 'Type', type: ColumnType.text, cellTemplate: this.typeCell },
            { field: 'serialNumber', caption: 'Serial Number', type: ColumnType.text }
        ];

        if (this.canManage) {
            cols.push({ field: 'id', caption: '', type: ColumnType.text, cellTemplate: this.actionsCell, queryColumnExcluded: true });
        }

        return cols;
    }

    public async loadGauges(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._gaugeService.getUnverifiedGauges(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public async manage(gauge: WaterSupplierGauge): Promise<void> {
        await this._router.navigate(['/backflow/testers/details', gauge.professionalId]);
    }
}
