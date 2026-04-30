import { Component, OnInit, AfterViewInit, ViewChild, TemplateRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CsiInspectionService } from '../../../../shared/services/csi/csi-inspection.service';
import { CsiProfessionalSearchRequest } from '../../../../shared/models/csi/csi-inspection-enums';
import { ProfessionalSupplierService } from '../../../../shared/services/professionals/professional-supplier.service';
import { CsiInspection } from '../../../../shared/models/csi/csi-inspection';
import { PageInfo } from '../../../../shared/models/page-info';
import { Query, QueryProperty } from '../../../../shared/models/query';
import { InputOption } from '../../../../shared/components/input/input.component';
import { TableColumn, TableCustomAction, CellTemplateData } from '../../../../shared/components/data-components/table/table.component';
import { ColumnType } from '../../../../shared/components/data-components/sorting-filtering/query-view-model';

interface SearchForm extends CsiProfessionalSearchRequest {
    waterSupplierId?: number | null;
    accountNumber?: string | null;
    propertyType?: string | null;
    propertyBusinessName?: string | null;
    propertyStreetNumber?: string | null;
    propertyStreetName?: string | null;
    propertyNumber?: string | null;
    propertyZip?: string | null;
    mailingCompanyName?: string | null;
    mailingContactName?: string | null;
    mailingStreetNumber?: string | null;
    mailingStreetName?: string | null;
    mailingNumber?: string | null;
    mailingZip?: string | null;
}

@Component({
    standalone: false,
    templateUrl: './csi-inspection-list.component.html',
    styleUrl: './csi-inspection-list.component.scss'
})
export class CsiInspectionListComponent implements OnInit, AfterViewInit {
    @ViewChild('statusTemplate') statusTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;
    @ViewChild('propertyTemplate') propertyTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;
    @ViewChild('mailingTemplate') mailingTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;
    @ViewChild('inspectorTemplate') inspectorTemplate!: TemplateRef<CellTemplateData<CsiInspection>>;

    public showResults = false;
    public isLoading = false;
    public resultCount = 0;

    public pageInfo: PageInfo = {};
    public inspections: CsiInspection[] = [];
    public query: Query = {};
    public columns: TableColumn<CsiInspection>[] = [];
    public customActions: TableCustomAction<CsiInspection>[] = [
        {
            text: 'View',
            iconClass: 'fa-solid fa-magnifying-glass',
            action: (inspection: CsiInspection) => this.viewInspection(inspection)
        }
    ];

    public searchRequest: SearchForm = {
        waterSupplierId: null,
        latestOnly: true,
        passFail: '',
        dateType: ''
    };

    public scopeValue: string = '';
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
        { id: '1', text: 'Passed' },
        { id: '2', text: 'Failed' }
    ];

    public dateTypeOptions: InputOption[] = [
        { id: '', text: 'Any date range' },
        { id: '1', text: 'Inspection Date' },
        { id: '2', text: 'Submission Date' }
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
    ) {}

    public async ngOnInit(): Promise<void> {
        await this.loadWaterSupplierScopeOptions();
    }

    public ngAfterViewInit(): void {
        this.columns = this.buildColumns();
    }

    public async search(): Promise<void> {
        this.searchAttempted = true;
        this.pageInfo = {};
        this.query = this.buildSearchQuery();
        await this.loadInspections();
        if (this.inspections.length > 0) {
            this.showResults = true;
        }
    }

    public async loadInspections(): Promise<void> {
        try {
            this.isLoading = true;

            const request: CsiProfessionalSearchRequest = {
                latestOnly: this.searchRequest.latestOnly,
                passFail: this.searchRequest.passFail,
                dateType: this.searchRequest.dateType,
                fromDate: this.searchRequest.fromDate,
                toDate: this.searchRequest.toDate
            };
            const result = await this._inspectionService.getProfessionalInspections(this.pageInfo, this.query, request);
            this.inspections = result.data ?? [];
            this.pageInfo = result.pageInfo ?? {};
            this.resultCount = this.pageInfo.totalItems ?? this.inspections.length;
        } finally {
            this.isLoading = false;
        }
    }

    public onLatestOnlyChange(value: string): void {
        this.searchRequest.latestOnly = value === 'true';
    }

    public onScopeChange(value: string): void {
        this.scopeValue = value;
        this.searchRequest.waterSupplierId = value ? Number(value) : null;
    }

    public searchAgain(): void {
        this.showResults = false;
        this.inspections = [];
        this.resultCount = 0;
        this.searchAttempted = false;
    }

    public viewInspection(inspection: CsiInspection): void {
        const url = this._router.serializeUrl(
            this._router.createUrlTree([inspection.id], { relativeTo: this._activatedRoute })
        );
        window.open(url, '_blank');
    }

    public isPassed(inspection: CsiInspection): boolean {
        return !!(inspection.compliance1 && inspection.compliance2 && inspection.compliance3 &&
                  inspection.compliance4 && inspection.compliance5 && inspection.compliance6);
    }

    public isPaid(inspection: CsiInspection): boolean {
        return !!inspection.transactionId;
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

    private buildSearchQuery(): Query {
        const filter: QueryProperty[] = [];
        const f = this.searchRequest;

        this.addFilter(filter, 'waterSupplierId', f.waterSupplierId, 'Eq');
        this.addFilter(filter, 'site.accountNumber', f.accountNumber, 'Eq');
        this.addFilter(filter, 'propertyType', f.propertyType, 'Eq');
        this.addFilter(filter, 'propertyBusinessName', f.propertyBusinessName, 'Ct');
        this.addFilter(filter, 'propertyStreetNumber', f.propertyStreetNumber, 'Ct');
        this.addFilter(filter, 'propertyStreetName', f.propertyStreetName,'Ct');
        this.addFilter(filter, 'propertyNumber', f.propertyNumber, 'Eq');
        this.addFilter(filter, 'propertyZip', f.propertyZip, 'Eq');
        this.addFilter(filter, 'mailingCompanyName', f.mailingCompanyName,'Ct');
        this.addFilter(filter, 'mailingContactName', f.mailingContactName, 'Ct');
        this.addFilter(filter, 'mailingStreetNumber', f.mailingStreetNumber, 'Ct');
        this.addFilter(filter, 'mailingStreetName', f.mailingStreetName, 'Ct');
        this.addFilter(filter, 'mailingNumber', f.mailingNumber, 'Eq');
        this.addFilter(filter, 'mailingZip', f.mailingZip, 'Eq');

        return { filter };
    }

    private addFilter(
        filter: QueryProperty[],
        columnName: string,
        value: string | number | null | undefined,
        comparisonOperator: 'Eq' | 'Ct'
    ): void {
        if (value === null || value === undefined || value === '')
        {
            return;
        }

        filter.push({
            columnName,
            value: String(value),
            comparisonOperator,
            logicalOperator: 'And'
        });
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
