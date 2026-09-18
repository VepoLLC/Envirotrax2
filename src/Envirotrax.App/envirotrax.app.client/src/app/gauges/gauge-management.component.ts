import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { WaterSupplierGauge } from '../shared/models/backflow/water-supplier-gauge';
import { WaterSupplierGaugeService } from '../shared/services/backflow/water-supplier-gauge.service';
import { TableViewModel } from '../shared/models/table-view-model';
import { AuthService } from '../shared/services/auth/auth.service';
import { FeatureType } from '../shared/models/feature-type';
import { PermissionAction, PermissionType } from '../shared/models/permission-type';
import { CellTemplateData, ColumnType, TableColumn } from '@envirotrax/common-ui';

@Component({
    templateUrl: './gauge-management.component.html',
    standalone: false
})
export class GaugeManagementComponent implements OnInit {
    @ViewChild('calibrationDateCell', { static: true })
    private calibrationDateCell!: TemplateRef<CellTemplateData<WaterSupplierGauge>>;

    @ViewChild('actionsCell', { static: true })
    private actionsCell!: TemplateRef<CellTemplateData<WaterSupplierGauge>>;

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

        this.table.columns = this.getColumns();
        await this.loadGauges();
    }

    private getColumns(): TableColumn<WaterSupplierGauge>[] {
        return [
            { field: 'companyName', caption: 'Company name', type: ColumnType.text },
            { field: 'companyEmail', caption: 'Company email', type: ColumnType.text },
            { field: 'manufacturer', caption: 'Manufacturer', type: ColumnType.text },
            { field: 'model', caption: 'Model', type: ColumnType.text },
            { field: 'serialNumber', caption: 'Serial #', type: ColumnType.text },
            {
                field: 'lastCalibrationDate',
                caption: 'Calibration date',
                type: ColumnType.date,
                cellTemplate: this.calibrationDateCell
            },
            { field: 'id', caption: '', type: ColumnType.text, cellTemplate: this.actionsCell }
        ];
    }

    public async loadGauges(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._gaugeService.getGauges(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public manage(gauge: WaterSupplierGauge): void {
        if (gauge.professionalId == null) {
            return;
        }

        this._router.navigate(['/backflow/testers/details', String(gauge.professionalId)]);
    }
}
