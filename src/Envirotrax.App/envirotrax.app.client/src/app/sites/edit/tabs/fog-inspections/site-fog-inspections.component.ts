import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { FogInspection } from '../../../../shared/models/fog/fog-inspection';
import { FogInspectionService } from '../../../../shared/services/fog/fog-inspection.service';
import { TableViewModel } from '../../../../shared/models/table-view-model';
import { ComparisonOperator, QueryProperty } from '../../../../shared/models/query';
import { CellTemplateData, ColumnType, TableColumn } from '@envirotrax/common-ui';

@Component({
    selector: 'app-site-fog-inspections',
    standalone: false,
    templateUrl: './site-fog-inspections.component.html'
})
export class SiteFogInspectionsComponent implements OnInit {
    @Input()
    public siteId?: number;

    @ViewChild('statusTemplate', { static: true })
    public statusTemplate!: TemplateRef<CellTemplateData<FogInspection>>;

    @ViewChild('inspectionDateTemplate', { static: true })
    public inspectionDateTemplate!: TemplateRef<CellTemplateData<FogInspection>>;

    @ViewChild('generatorTemplate', { static: true })
    public generatorTemplate!: TemplateRef<CellTemplateData<FogInspection>>;

    @ViewChild('interceptorTemplate', { static: true })
    public interceptorTemplate!: TemplateRef<CellTemplateData<FogInspection>>;

    @ViewChild('inspectorTemplate', { static: true })
    public inspectorTemplate!: TemplateRef<CellTemplateData<FogInspection>>;

    public table: TableViewModel<FogInspection> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        }
    };

    constructor(
        private readonly _fogInspectionService: FogInspectionService,
        private readonly _router: Router
    ) { }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();
        await this.getInspections();
    }

    public viewInspection(inspection: FogInspection): void {
        if (!inspection.id) {
            return;
        }
        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/fog', 'inspections', inspection.id])
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
            this.table.items = await this._fogInspectionService.getAll(
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

    private getColumns(): TableColumn<FogInspection>[] {
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
                type: ColumnType.other,
                cellTemplate: this.inspectionDateTemplate
            },
            {
                field: '',
                caption: 'Generator Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.generatorTemplate
            },
            {
                field: '',
                caption: 'Interceptor Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.interceptorTemplate
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
