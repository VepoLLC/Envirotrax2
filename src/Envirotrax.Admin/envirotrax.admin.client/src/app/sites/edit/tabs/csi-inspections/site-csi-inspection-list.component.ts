import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { CellTemplateData, ColumnType, TableColumn, TableViewModel } from '@envirotrax/common-ui';
import { CsiInspectionDetailsComponent } from '../../../../csi/inspections/details/csi-inspection-details.component';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { CsiInspection, CsiInspectionReason, CsiPaymentStatus } from '../../../../shared/models/csi/csi-inspection';
import { MODULE_COLORS } from '../../../../shared/models/module-colors';
import { CsiInspectionService } from '../../../../shared/services/csi/csi-inspection.service';
import { WindowService } from '../../../../shared/services/window.service';

interface SiteCsiInspectionRow extends CsiInspection {
    reasonDescription: string;
}

@Component({
    selector: 'app-site-csi-inspection-list',
    templateUrl: './site-csi-inspection-list.component.html',
    imports: [CommonModule, SharedComponentsModule],
})
export class SiteCsiInspectionListComponent implements OnInit {
    @Input()
    public siteId: number = 0;

    @ViewChild('resultCell', { static: true })
    public resultCell?: TemplateRef<CellTemplateData<SiteCsiInspectionRow>>;

    @ViewChild('inspectionDateCell', { static: true })
    public inspectionDateCell?: TemplateRef<CellTemplateData<SiteCsiInspectionRow>>;

    @ViewChild('inspectorCell', { static: true })
    public inspectorCell?: TemplateRef<CellTemplateData<SiteCsiInspectionRow>>;

    public table: TableViewModel<SiteCsiInspectionRow> = {
        query: {
            sort: { inspectionDate: 'Desc' },
            filter: []
        }
    };

    constructor(
        private readonly _csiInspectionService: CsiInspectionService,
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

            const inspections = await this._csiInspectionService.getAll(this.table.items?.pageInfo || {}, this.table.query, CsiPaymentStatus.Paid);

            const rows: SiteCsiInspectionRow[] = [];

            for (const inspection of inspections.data) {
                rows.push({
                    ...inspection,
                    reasonDescription: this.buildReasonDescription(inspection.reasonForInspection)
                });
            }

            this.table.items = {
                pageInfo: inspections.pageInfo,
                data: rows
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    public viewDetails(inspection: SiteCsiInspectionRow): void {
        this._windowService.addWindow(CsiInspectionDetailsComponent, {
            title: this.buildWindowTitle(inspection),
            color: MODULE_COLORS.CSI,
            model: { id: inspection.id }
        });
    }

    private buildWindowTitle(inspection: CsiInspection): string {
        let address = `${inspection.propertyStreetNumber ?? ''} ${inspection.propertyStreetName ?? ''}`.trim();

        if (inspection.propertyNumber) {
            address = `${address} #${inspection.propertyNumber}`;
        }

        return `${inspection.id} - ${address}`;
    }

    private buildReasonDescription(reason?: CsiInspectionReason): string {
        switch (reason) {
            case CsiInspectionReason.NewConstruction:
                return 'New construction';
            case CsiInspectionReason.ExistingServiceContaminantHazardsSuspected:
                return 'Existing service';
            case CsiInspectionReason.MajorRenovationOrExpansion:
                return 'Major renovation';
            default:
                return '';
        }
    }

    private getColumns(): TableColumn<SiteCsiInspectionRow>[] {
        return [
            { field: 'id', caption: 'ID', type: ColumnType.number },
            { field: 'inspectionResult', caption: '', type: ColumnType.other, cellTemplate: this.resultCell, queryColumnExcluded: true },
            { field: 'inspectionDate', caption: 'Inspection Date', type: ColumnType.other, cellTemplate: this.inspectionDateCell, queryColumnExcluded: true },
            { field: 'reasonDescription', caption: 'Reason for Inspection', type: ColumnType.other, queryColumnExcluded: true },
            { field: 'inspectorCompanyName', caption: 'Inspector Information', type: ColumnType.other, cellTemplate: this.inspectorCell, queryColumnExcluded: true }
        ];
    }
}
