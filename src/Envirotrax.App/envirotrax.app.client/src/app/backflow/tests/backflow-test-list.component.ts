import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { BackflowTestService } from '../../shared/services/backflow/backflow-test.service';
import { QueryProperty } from '../../shared/models/query';
import { TableViewModel } from '../../shared/models/table-view-model';
import { BackflowTest } from '../../shared/models/backflow/backflow-test';
import { TableColumn } from '../../shared/components/data-components/table/table.component';
import { ColumnType } from '../../shared/components/data-components/sorting-filtering/query-view-model';
import { InputOption } from '../../shared/components/input/input.component';
import { BackflowTestResult, BackflowReasonForTest } from '../../shared/models/backflow/backflow-test-enums';

@Component({
    selector: 'app-backflow-test-list',
    standalone: false,
    templateUrl: './backflow-test-list.component.html'
})
export class BackflowTestListComponent implements OnInit {
    public showResults: boolean = false;

    public table: TableViewModel<BackflowTest> = {
        columns: this.getColumns(),
        query: {
            sort: {},
            filter: []
        },
        freeTextSearch: {
            searchQuery: [
                { field: 'accountNumber', operator: 'Ct' },
                { field: 'serialNumber', operator: 'Ct' },
                { field: 'bpatLicenseNumber', operator: 'Ct' }
            ]
        }
    };

    public testHistoryOptions: InputOption[] = [
        { id: "", text: "All Tests" },
        { id: "true", text: "Latest Test Only" }
    ];

    public testResultOptions: InputOption[] = [
        { id: "", text: "All Test Results" },
        { id: BackflowTestResult.Pass.toString(), text: "Pass" },
        { id: BackflowTestResult.Fail.toString(), text: "Fail" },
        { id: BackflowTestResult.PassAfterRepairs.toString(), text: "Pass After Repairs" }
    ];

    public serviceStatusOptions: InputOption[] = [
        { id: "", text: "All Status Types" },
        { id: "false", text: "Active Only" },
        { id: "true", text: "Out of Service Only" }
    ];

    public paymentStatusOptions: InputOption[] = [
        { id: "", text: "Any Status" },
        { id: "true", text: "Paid" },
        { id: "false", text: "Unpaid" }
    ];

    public approvalStatusOptions: InputOption[] = [
        { id: "", text: "Any Status" },
        { id: "false", text: "Approved" },
        { id: "true", text: "Disapproved" }
    ];

    public rejectedStatusOptions: InputOption[] = [
        { id: "", text: "Any Status" },
        { id: "false", text: "Not Rejected" },
        { id: "true", text: "Rejected" }
    ];

    public reasonForTestOptions: InputOption[] = [
        { id: "", text: "All Values" },
        { id: BackflowReasonForTest.AnnualTest.toString(), text: "Annual Test" },
        { id: BackflowReasonForTest.NewInstallation.toString(), text: "New Installation" },
        { id: BackflowReasonForTest.Relocation.toString(), text: "Relocation" },
        { id: BackflowReasonForTest.Replacement.toString(), text: "Replacement" },
        { id: BackflowReasonForTest.Repair.toString(), text: "Repair" },
        { id: BackflowReasonForTest.AnnualTestAfterRepairs.toString(), text: "Annual Test After Repairs" }
    ];

    public yesNoOptions: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "true", text: "Yes" },
        { id: "false", text: "No" }
    ];

    public propertyTypeOptions: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "0", text: "Residential" },
        { id: "1", text: "Commercial" }
    ];

    constructor(
        private readonly _backflowTestService: BackflowTestService
    ) {}

    public async ngOnInit(): Promise<void> {}

    private getColumns(): TableColumn<BackflowTest>[] {
        return [
            {
                field: 'accountNumber',
                caption: 'Account Number',
                type: ColumnType.text
            },
            {
                field: 'serialNumber',
                caption: 'Serial Number',
                type: ColumnType.text
            },
            {
                field: 'propertyBusinessName',
                caption: 'Business Name',
                type: ColumnType.text
            },
            {
                field: 'propertyStreetNumber',
                caption: 'Street Number',
                type: ColumnType.text
            },
            {
                field: 'propertyStreetName',
                caption: 'Street Name',
                type: ColumnType.text
            },
            {
                field: 'propertyCity',
                caption: 'City',
                type: ColumnType.text
            },
            {
                field: 'testDate',
                caption: 'Test Date',
                type: ColumnType.date
            },
            {
                field: 'testResult',
                caption: 'Test Result',
                type: ColumnType.text
            },
            {
                field: 'bpatCompanyName',
                caption: 'BPAT Company',
                type: ColumnType.text
            },
            {
                field: 'expirationDate',
                caption: 'Expiration Date',
                type: ColumnType.date
            }
        ];
    }

    public async getTests(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._backflowTestService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (searchForm.valid) {
            await this.getTests();
            this.showResults = true;
        }
    }
}
