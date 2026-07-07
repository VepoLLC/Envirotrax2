import { Component, OnInit, ViewChild, TemplateRef, ElementRef } from "@angular/core";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { CsiInspection } from "../../../shared/models/csi/csi-inspection";
import { CsiInspectionService } from "../../../shared/services/csi/csi-inspection.service";
import { QueryProperty } from "../../../shared/models/query";
import { NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { DownloadConfig } from "../../../shared/models/download-config";
import { DownloadService } from "../../../shared/services/download.service";
import { CellTemplateData, ColumnType, InputOption, TableColumn } from "@envirotrax/common-ui";
import { PrintableTableService } from "../../../shared/services/printable-table.service";

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
    
    @ViewChild('printableSection')
    private _printableSection!: ElementRef;

    public showResults: boolean = false;

    public table: TableViewModel<CsiInspection> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'inspectorLicenseNumber', operator: 'Ct' },
                { field: 'inspectorCompanyName', operator: 'Ct', multiWordSearch: true },
                { field: 'inspectorContactName', operator: 'Ct', multiWordSearch: true }
            ]
        }
    };

    public downloadConfig: DownloadConfig;

    public passFailOptions: InputOption[] = [
        { id: '', text: 'Any result' },
        { id: 'true', text: 'Pass' },
        { id: 'false', text: 'Fail' }
    ];

    public yesNoOptions: InputOption[] = [
        { id: '', text: 'Any result' },
        { id: 'true', text: 'Yes' },
        { id: 'false', text: 'No' }
    ];

    public approvalStatusOptions: InputOption[] = [
        { id: '', text: 'Any Status' },
        { id: 'false', text: 'Approved' },
        { id: 'true', text: 'Disapproved' }
    ];

    public propertyTypes: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: '0', text: 'Residential' },
        { id: '1', text: 'Commercial' }
    ];

    constructor(
        private readonly _csiInspectionService: CsiInspectionService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _downloadService: DownloadService,
        private readonly _printService: PrintableTableService
    ) {
        this.downloadConfig = {
            fileName: 'CSI Inspections',
            endpoint: this._csiInspectionService.getAllEndpoint(),
            pdfEndpoint: this._csiInspectionService.getAllPdfEndpoint(),
            suppoertedFormats: ['CSV', 'Excel', "PDF"],
            columns: [
                { field: 'inspectionDate', caption: 'InspectionDate' },
                { field: 'inspectionResult', caption: 'PassedInspection' },
                { field: 'site.accountNumber', caption: 'AccountNumber' },
                { field: 'propertyBusinessName', caption: 'PropertyBusinessName' },
                { field: 'propertyStreetNumber', caption: 'PropertyStreetNumber' },
                { field: 'propertyStreetName', caption: 'PropertyStreetName' },
                { field: 'propertyNumber', caption: 'PropertyNumber' },
                { field: 'propertyCity', caption: 'PropertyCity' },
                { field: 'propertyState.code', caption: 'PropertyState' },
                { field: 'propertyZip', caption: 'PropertyZIP' },
                { field: 'mailingCompanyName', caption: 'MailingCompanyName' },
                { field: 'mailingContactName', caption: 'MailingContactName' },
                { field: 'mailingStreetNumber', caption: 'MailingStreetNumber' },
                { field: 'mailingStreetName', caption: 'MailingStreetName' },
                { field: 'mailingNumber', caption: 'MailingNumber' },
                { field: 'mailingCity', caption: 'MailingCity' },
                { field: 'mailingState.code', caption: 'MailingState' },
                { field: 'mailingZip', caption: 'MailingZIP' },
                { field: 'mailingPhoneNumber', caption: 'MailingPhoneNumber' },
                { field: 'mailingEmailAddress', caption: 'MailingEmailAddress' },
                { field: 'inspectorCompanyName', caption: 'InspectorCompanyName' },
                { field: 'inspectorContactName', caption: 'InspectorContactName' },
                { field: 'inspectorAddress', caption: 'InspectorAddress' },
                { field: 'inspectorCity', caption: 'InspectorCity' },
                { field: 'inspectorState', caption: 'InspectorState' },
                { field: 'inspectorZip', caption: 'InspectorZip' },
                { field: 'comments', caption: 'Remarks' }
            ]
        };
    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        const dateParam = this._activatedRoute.snapshot.queryParamMap.get('date');
        if (dateParam) {
            this.table.query.filter = [{
                columnName: 'inspectionDate',
                children: [
                    { columnName: 'inspectionDate', value: dateParam, comparisonOperator: 'Gte', logicalOperator: 'And' },
                    { columnName: 'inspectionDate', value: dateParam, comparisonOperator: 'Lte', logicalOperator: 'And' }
                ]
            }];
            await this.getInspections();
            this.showResults = (this.table.items?.pageInfo?.totalItems ?? 0) > 0;
        }
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

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            await this.getInspections();
            this.showResults = true;
        }
    }

    public async getInspections(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._csiInspectionService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public searchAgain(): void {
        this.showResults = false;
    }

    public viewDetails(inspection: CsiInspection): void {
        this._router.navigate([inspection.id, 'view'], {
            relativeTo: this._activatedRoute
        });
    }

    public showDownlaodManager(): void {
        this._downloadService.showDownloadManager(this.downloadConfig, this.table.query);
    }

    public viewPrintableTable(): void {
        this._printService.open(this._printableSection.nativeElement);
    }
}
