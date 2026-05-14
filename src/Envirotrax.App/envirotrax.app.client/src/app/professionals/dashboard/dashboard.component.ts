import { Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../shared/services/auth/auth.service';
import { CsiInspectionService } from '../../shared/services/csi/csi-inspection.service';
import { CsiInspection } from '../../shared/models/csi/csi-inspection';
import { FeatureType } from '../../shared/models/feature-tyype';
import { TableColumn, CellTemplateData } from '../../shared/components/data-components/table/table.component';
import { ColumnType } from '../../shared/components/data-components/sorting-filtering/query-view-model';
import { TableViewModel } from '../../shared/models/table-view-model';

@Component({
    standalone: false,
    templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
    @ViewChild('statusTemplate', { static: true })
    public statusTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    @ViewChild('propertyTemplate', { static: true })
    public propertyTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    @ViewChild('mailingTemplate', { static: true })
    public mailingTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    public hasCsi = false;
    public hasBackflow = false;
    public isLoading = true;

    public recentInspections: TableViewModel<CsiInspection> = {
        query: {},
        columns: []
    };

    constructor(
        private readonly _authService: AuthService,
        private readonly _inspectionService: CsiInspectionService,
        private readonly _router: Router
    ) {}

    public async ngOnInit(): Promise<void> {
        try {
            [this.hasCsi, this.hasBackflow] = await Promise.all([
                this._authService.hasAnyFeatures(FeatureType.CsiInspection),
                this._authService.hasAnyFeatures(FeatureType.BackflowTesting)
            ]);

            this.recentInspections.columns = this.buildColumns();

            if (this.hasCsi) {
                await this.loadRecentInspections();
            }
        } finally {
            this.isLoading = false;
        }
    }

    private async loadRecentInspections(): Promise<void> {
        try {
            this.recentInspections.isLoading = true;
            this.recentInspections.items = await this._inspectionService.getProfessionalInspections(
                { pageSize: 5 },
                this.recentInspections.query,
                true
            );
        } finally {
            this.recentInspections.isLoading = false;
        }
    }

    public viewInspection(inspection: CsiInspection): void {
        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/professionals/csi/inspections', inspection.id])
        );
        window.open(url, '_blank');
    }

    private buildColumns(): TableColumn<CsiInspection>[] {
        return [
            {
                field: '',
                caption: 'Status',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.statusTemplate
            },
            {
                field: 'inspectionDate',
                caption: 'Inspection Date',
                type: ColumnType.date
            },
            {
                field: 'site.accountNumber',
                caption: 'Account Number',
                type: ColumnType.text
            },
            {
                field: '',
                caption: 'Property Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.propertyTemplate
            },
            {
                field: '',
                caption: 'Mailing / Contact Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.mailingTemplate
            }
        ];
    }
}
