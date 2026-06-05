import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { CsiInspection } from '../../../shared/models/csi/csi-inspection';
import { CsiInspectionService } from '../../../shared/services/csi/csi-inspection.service';
import { AuthService } from '../../../shared/services/auth/auth.service';
import { PermissionAction, PermissionType } from '../../../shared/models/permission-type';
import { TableViewModel } from '../../../shared/models/table-view-model';
import { CellTemplateData, TableColumn } from '../../../shared/components/data-components/table/table.component';
import { ColumnType } from '../../../shared/components/data-components/sorting-filtering/query-view-model';
import { ComparisonOperator, QueryProperty } from '../../../shared/models/query';
import { CsiInspectionReason, csiInspectionReasonLabels } from '../../../shared/enums/csi-inspection-reason.enum';

@Component({
    selector: 'app-site-csi-inspections',
    standalone: false,
    templateUrl: './site-csi-inspections.component.html'
})
export class SiteCsiInspectionsComponent implements OnInit {
    @Input()
    public siteId?: number;

    public canViewInspectors: boolean = false;

    @ViewChild('statusTemplate', { static: true })
    public statusTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    @ViewChild('reasonTemplate', { static: true })
    public reasonTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    @ViewChild('inspectorTemplate', { static: true })
    public inspectorTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    public reasonLabel(reason?: number | null): string {
        if (reason === undefined || reason === null) {
            return '';
        }
        return csiInspectionReasonLabels[reason as CsiInspectionReason] ?? '';
    }

    public table: TableViewModel<CsiInspection> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        }
    };

    constructor(
        private readonly _csiInspectionService: CsiInspectionService,
        private readonly _router: Router,
        private readonly _authService: AuthService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.canViewInspectors = await this._authService.hasAnyPermisison(
            PermissionAction.CanView, PermissionType.CsiInspectors);
        this.table.columns = this.getColumns();
        await this.getInspections();
    }

    public viewInspection(inspection: CsiInspection): void {
        if (!inspection.id) {
            return;
        }
        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/csi', inspection.id, 'view'])
        );
        window.open(url, '_blank');
    }

    public async getInspections(): Promise<void> {
        if (!this.siteId) {
            return;
        }
        try {
            this.table.isLoading = true;
            this.table.query.filter = [this.siteFilter()];
            this.table.items = await this._csiInspectionService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    private siteFilter(): QueryProperty {
        return {
            columnName: 'site.id',
            value: this.siteId!.toString(),
            comparisonOperator: 'Eq' as ComparisonOperator
        };
    }

    private getColumns(): TableColumn<CsiInspection>[] {
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
                field: 'reasonForInspection',
                caption: 'Reason for Inspection',
                type: ColumnType.other,
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
