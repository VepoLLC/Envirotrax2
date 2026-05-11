import { Component, OnInit, AfterViewInit, ViewChild, TemplateRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CsiInspectionService } from '../../../../shared/services/csi/csi-inspection.service';
import { ProfessionalSupplierService } from '../../../../shared/services/professionals/professional-supplier.service';
import { CsiInspection } from '../../../../shared/models/csi/csi-inspection';
import { QueryProperty } from '../../../../shared/models/query';
import { InputOption } from '../../../../shared/components/input/input.component';
import { TableColumn, CellTemplateData } from '../../../../shared/components/data-components/table/table.component';
import { ColumnType } from '../../../../shared/components/data-components/sorting-filtering/query-view-model';
import { TableViewModel } from '../../../../shared/models/table-view-model';


@Component({
    standalone: false,
    templateUrl: './csi-inspection-list.component.html'
})
export class CsiInspectionListComponent implements OnInit {
    @ViewChild('statusTemplate', { static: true })
    public statusTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    @ViewChild('propertyTemplate', { static: true })
    public propertyTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    @ViewChild('mailingTemplate', { static: true })
    public mailingTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    @ViewChild('inspectorTemplate', { static: true })
    public inspectorTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    public inspections: TableViewModel<CsiInspection> = {
        query: {},
        columns: []
    };

    public showResults = false;
    public latestOnly = true;
    public searchAttempted = false;

    public waterSupplierScopeOptions: InputOption[] = [
        { id: '', text: 'My inspection history only' }
    ];

    public inspectionHistoryOptions: InputOption[] = [
        { id: 'true', text: 'Latest inspection only' },
        { id: 'false', text: 'Complete inspection history' }
    ];

    public passFailOptions: InputOption[] = [
        { id: '', text: 'Any result' },
        { id: 'true', text: 'Passed' },
        { id: 'false', text: 'Failed' }
    ];

    public propertyTypeOptions: InputOption[] = [
        { id: '', text: '' },
        { id: '0', text: 'Residential' },
        { id: '1', text: 'Commercial' }
    ];

    constructor(
        private readonly _inspectionService: CsiInspectionService,
        private readonly _supplierService: ProfessionalSupplierService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute
    ) { }

    public async ngOnInit(): Promise<void> {
        this.inspections.columns = this.buildColumns();
        await this.loadWaterSupplierScopeOptions();
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.inspections.query.filter = queryProperties;
    }

    public async search(): Promise<void> {
        this.searchAttempted = true;
        await this.loadInspections();

        if (this.inspections.items?.data.length! > 0) {
            this.showResults = true;
        }
    }

    public async loadInspections(): Promise<void> {
        try {
            this.inspections.isLoading = true;

            this.inspections.items = await this._inspectionService.getProfessionalInspections(this.inspections?.items?.pageInfo || {}, this.inspections.query, this.latestOnly);
        } finally {
            this.inspections.isLoading = false;
        }
    }

    public onLatestOnlyChange(value: string): void {
        this.latestOnly = value === 'true';
    }

    public searchAgain(): void {
        this.showResults = false;
        this.searchAttempted = false;
    }

    public viewInspection(inspection: CsiInspection): void {
        const url = this._router.serializeUrl(
            this._router.createUrlTree([inspection.id], { relativeTo: this._activatedRoute })
        );
        window.open(url, '_blank');
    }

    private async loadWaterSupplierScopeOptions(): Promise<void> {
        const suppliers = await this._supplierService.getAllMy(true);
        const supplierOptions: InputOption[] = suppliers.data
            .filter(s => s.waterSupplier?.id)
            .map(s => ({ id: String(s.waterSupplier!.id!), text: s.waterSupplier!.name ?? '' }));
        this.waterSupplierScopeOptions = [
            { id: '', text: 'My inspection history only' },
            ...supplierOptions
        ];
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
            },
            {
                field: '',
                caption: 'Inspector',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.inspectorTemplate
            }
        ];
    }
}
