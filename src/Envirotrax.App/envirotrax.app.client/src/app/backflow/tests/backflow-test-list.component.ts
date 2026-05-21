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
import { FacilityType } from '../../shared/enums/facility-type.enum';

@Component({
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
        { id: BackflowReasonForTest.ExistingInstallation.toString(), text: "Existing Installation" },
        { id: BackflowReasonForTest.Replacement.toString(), text: "Replacement" },
        { id: BackflowReasonForTest.Repair.toString(), text: "Repair" },
        { id: BackflowReasonForTest.AnnualTestAfterRepairs.toString(), text: "Annual Test After Repairs" }
    ];

    public yesNoOptions: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "true", text: "Yes" },
        { id: "false", text: "No" }
    ];

    public gaugeOptions: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "false", text: "Potable" },
        { id: "true", text: "Non-Potable" }
    ];

    public propertyTypeOptions: InputOption[] = [
        { id: "", text: "Any Value" },
        { id: "0", text: "Residential" },
        { id: "1", text: "Commercial" }
    ];

    public hazardTypeOptions: InputOption[] = [
        { id: "", text: "All Hazard Types" },
        { id: "Agricultural/Feed Lot", text: "Agricultural/Feed Lot" },
        { id: "Domestic/Premises Isolation", text: "Domestic/Premises Isolation" },
        { id: "Fire System", text: "Fire System" },
        { id: "Gas Station/Car Wash", text: "Gas Station/Car Wash" },
        { id: "Irrigation - Non Chemical", text: "Irrigation - Non Chemical" },
        { id: "Irrigation - Chemical Feed", text: "Irrigation - Chemical Feed" },
        { id: "Laundry/Cleaners", text: "Laundry/Cleaners" },
        { id: "Medical/Dental/Laboratory/Mortuary", text: "Medical/Dental/Laboratory/Mortuary" },
        { id: "Nails/Salon/Grooming", text: "Nails/Salon/Grooming" },
        { id: "Pool/Recreation/Athletics", text: "Pool/Recreation/Athletics" },
        { id: "Restaurant/Vending/Grocery", text: "Restaurant/Vending/Grocery" },
        { id: "Fire Hydrant/Temporary Construction", text: "Fire Hydrant/Temporary Construction" },
        { id: "Fountains/Garden Ponds/Water Features", text: "Fountains/Garden Ponds/Water Features" },
        { id: "Water Softener", text: "Water Softener" },
        { id: "Other", text: "Other" }
    ];

    public facilityTypeOptions: InputOption[] = [
        { id: FacilityType.Other.toString(), text: "Other" },
        { id: FacilityType.Restaurant.toString(), text: "Restaurant" },
        { id: FacilityType.FastFoodEstablishment.toString(), text: "Fast food establishment" },
        { id: FacilityType.HotelMotel.toString(), text: "Hotel/motel" },
        { id: FacilityType.CarWash.toString(), text: "Car wash" },
        { id: FacilityType.SchoolUniversity.toString(), text: "School/university" },
        { id: FacilityType.GroceryStore.toString(), text: "Grocery store" },
        { id: FacilityType.ConvenienceStore.toString(), text: "Convenience store" },
        { id: FacilityType.AssistedLivingFacility.toString(), text: "Assisted living facility" },
        { id: FacilityType.MedicalFacility.toString(), text: "Medical facility" },
        { id: FacilityType.Industrial.toString(), text: "Industrial" },
        { id: FacilityType.CityOwnedFacility.toString(), text: "City-owned facility" }
    ];

    public deviceTypeOptions: InputOption[] = [
        { id: "", text: "All Device Types" },
        { id: "DC", text: "DC - Double Check Valve" },
        { id: "DCD", text: "DCD - Double Check Detector" },
        { id: "DCD2", text: "DCD2 - Double Check Detector Type II" },
        { id: "RP", text: "RP - Reduced Pressure Principle" },
        { id: "RPPD", text: "RPPD - Reduced Pressure Principle Detector" },
        { id: "RPPD2", text: "RPPD2 - Reduced Pressure Principle Detector Type II" },
        { id: "PVB", text: "PVB - Pressure Vacuum Breaker" },
        { id: "SVB", text: "SVB - Spill-Resistant Pressure Vacuum Breaker" },
        { id: "AG", text: "AG - Air Gap" }
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
