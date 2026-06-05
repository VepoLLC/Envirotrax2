import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../shared/services/auth/auth.service';
import { CsiInspectionService } from '../../../shared/services/csi/csi-inspection.service';
import { CsiInspection } from '../../../shared/models/csi/csi-inspection';
import { FeatureType } from '../../../shared/models/feature-type';
import { ROLE_DEFINITIONS } from '../../../shared/models/role-definitions';
import { Query, QueryProperty } from '../../../shared/models/query';
import { TableViewModel } from '../../../shared/models/table-view-model';
import { CellTemplateData, TableColumn } from '../../../shared/components/data-components/table/table.component';
import { ColumnType } from '../../../shared/components/data-components/sorting-filtering/query-view-model';
import { CsiInspectionReason, csiInspectionReasonLabels } from '../../../shared/enums/csi-inspection-reason.enum';

@Component({
    selector: 'vp-site-csi-inspections',
    standalone: false,
    templateUrl: './site-csi-inspections.component.html'
})
export class SiteCsiInspectionsComponent implements OnInit {
    @Input() public siteId!: number;

    @ViewChild('statusTemplate', { static: true })
    private statusTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    @ViewChild('inspectorTemplate', { static: true })
    private inspectorTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    @ViewChild('reasonTemplate', { static: true })
    private reasonTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    public isVisible = false;

    public getReasonLabel(reason?: number | null): string {
        if (reason == null) return '';
        return csiInspectionReasonLabels[reason as CsiInspectionReason] ?? '';
    }

    public table: TableViewModel<CsiInspection> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    constructor(
        private readonly _authService: AuthService,
        private readonly _inspectionService: CsiInspectionService,
        private readonly _router: Router
    ) {}

    public async ngOnInit(): Promise<void> {
        const hasFeature = await this._authService.hasAnyFeatures(FeatureType.CsiInspection);
        const hasRole = await this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.CSI_INSPECTOR);
        this.isVisible = hasFeature && hasRole;

        if (this.isVisible) {
            this.table.columns = this.buildColumns();
            await this.loadInspections();
        }
    }

    public async loadInspections(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.query = this.buildQuery();
            this.table.items = await this._inspectionService.getProfessionalInspections(
                this.table.items?.pageInfo || {},
                this.table.query,
                false
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public viewInspection(inspection: CsiInspection): void {
        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/professionals/csi/inspections', inspection.id])
        );
        window.open(url, '_blank');
    }

    public submitNewInspection(): void {
        this._router.navigate(['/professionals/csi/inspections/create', this.siteId]);
    }

    private buildQuery(): Query {
        const filter: QueryProperty[] = [
            { columnName: 'site.id', comparisonOperator: 'Eq', value: String(this.siteId) }
        ];
        return { filter, sort: this.table.query.sort };
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
                field: '',
                caption: 'Reason for Inspection',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.reasonTemplate
            },
            {
                field: '',
                caption: 'Inspector Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.inspectorTemplate
            }
        ];
    }
}
