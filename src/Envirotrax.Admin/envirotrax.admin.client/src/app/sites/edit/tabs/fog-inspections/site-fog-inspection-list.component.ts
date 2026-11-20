import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { CellTemplateData, ColumnType, TableColumn, TableViewModel } from '@envirotrax/common-ui';
import { FogInspectionDetailsComponent } from '../../../../fog/inspections/details/fog-inspection-details.component';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { FogInspection, InterceptorCapacityType, interceptorCapacityTypeLabels, InterceptorType } from '../../../../shared/models/fog/fog-inspection';
import { FogPaymentStatus } from '../../../../shared/models/fog/fog-payment-status';
import { MODULE_COLORS } from '../../../../shared/models/module-colors';
import { FogInspectionService } from '../../../../shared/services/fog/fog-inspection.service';
import { WindowService } from '../../../../shared/services/window.service';

interface SiteFogInspectionRow extends FogInspection {
    interceptorTypeDescription: string;
    interceptorCapacityDescription: string;
    inspectorCityStateZip: string;
}

@Component({
    selector: 'app-site-fog-inspection-list',
    templateUrl: './site-fog-inspection-list.component.html',
    imports: [CommonModule, SharedComponentsModule],
})
export class SiteFogInspectionListComponent implements OnInit {
    @Input()
    public siteId: number = 0;

    @ViewChild('inspectionDateCell', { static: true })
    public inspectionDateCell?: TemplateRef<CellTemplateData<SiteFogInspectionRow>>;

    @ViewChild('interceptorCell', { static: true })
    public interceptorCell?: TemplateRef<CellTemplateData<SiteFogInspectionRow>>;

    @ViewChild('inspectorCell', { static: true })
    public inspectorCell?: TemplateRef<CellTemplateData<SiteFogInspectionRow>>;

    public table: TableViewModel<SiteFogInspectionRow> = {
        query: {
            sort: { inspectionDate: 'Desc' },
            filter: []
        }
    };

    constructor(
        private readonly _fogInspectionService: FogInspectionService,
        private readonly _windowService: WindowService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.load();
    }

    public async load(): Promise<void> {
        if (this.table.items) {
            this.table.items.pageInfo.pageNumber = 1;
        }

        await this.getInspections();
    }

    public async getInspections(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.query.filter = [
                { columnName: 'site.id', value: String(this.siteId), comparisonOperator: 'Eq' }
            ];

            const inspections = await this._fogInspectionService.getAll(this.table.items?.pageInfo || {}, this.table.query, FogPaymentStatus.Paid);

            const rows: SiteFogInspectionRow[] = [];

            for (const inspection of inspections.data) {
                rows.push(this.toRow(inspection));
            }

            this.table.items = {
                pageInfo: inspections.pageInfo,
                data: rows
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    public viewDetails(inspection: SiteFogInspectionRow): void {
        this._windowService.addWindow(FogInspectionDetailsComponent, {
            title: this.buildWindowTitle(inspection),
            color: MODULE_COLORS.FOG,
            model: { id: inspection.id }
        });
    }

    private buildWindowTitle(inspection: FogInspection): string {
        let address = `${inspection.propertyStreetNumber ?? ''} ${inspection.propertyStreetName ?? ''}`.trim();

        if (inspection.propertyNumber) {
            address = `${address} #${inspection.propertyNumber}`.trim();
        }

        return address ? `${inspection.id} - ${address}` : String(inspection.id);
    }

    private toRow(inspection: FogInspection): SiteFogInspectionRow {
        return {
            ...inspection,
            interceptorTypeDescription: this.buildInterceptorTypeDescription(inspection),
            interceptorCapacityDescription: this.buildCapacityDescription(inspection.interceptorCapacity, inspection.interceptorCapacityType),
            inspectorCityStateZip: `${inspection.inspectorCity ?? ''}, ${inspection.inspectorState ?? ''}  ${inspection.inspectorZip ?? ''}`
        };
    }

    private buildInterceptorTypeDescription(inspection: FogInspection): string {
        if (inspection.interceptorType === InterceptorType.Other) {
            return inspection.interceptorOtherDescription ?? '';
        }

        return inspection.interceptorType ?? '';
    }

    private buildCapacityDescription(capacity?: number, type?: InterceptorCapacityType): string {
        if (capacity == null) {
            return '';
        }

        return `${capacity} ${interceptorCapacityTypeLabels[type ?? InterceptorCapacityType.Gallons]}`;
    }

    private getColumns(): TableColumn<SiteFogInspectionRow>[] {
        return [
            { field: 'id', caption: 'ID', type: ColumnType.number },
            { field: 'inspectionDate', caption: 'Inspection Date', type: ColumnType.other, cellTemplate: this.inspectionDateCell, queryColumnExcluded: true },
            { field: 'interceptorType', caption: 'Interceptor Information', type: ColumnType.other, cellTemplate: this.interceptorCell, queryColumnExcluded: true },
            { field: 'inspectorCompanyName', caption: 'Inspector Information', type: ColumnType.other, cellTemplate: this.inspectorCell, queryColumnExcluded: true }
        ];
    }
}
