import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { SiteService } from "../../../shared/services/sites/site.service";
import { ProfessionalSupplierService } from "../../../shared/services/professionals/professional-supplier.service";
import { TableColumn } from "../../../shared/components/data-components/table/table.component";
import { ColumnType } from "../../../shared/components/data-components/sorting-filtering/query-view-model";
import { Site } from "../../../shared/models/sites/site";
import { QueryProperty } from "../../../shared/models/query";
import { NgForm } from "@angular/forms";

@Component({
    standalone: false,
    templateUrl: './csi-submission-property-search.component.html'
})
export class CsiSubmissionPropertySearchComponent implements OnInit {

    public showResults: boolean = false;
    public hasRegistrations: boolean = true;
    public isSubAccount: boolean = false;
    public accountNumber: string = '';
    public streetNumber: string = '';

    public errorMessage: string = '';

    public table: TableViewModel<Site> = {
        columns: this.getColumns(),
        query: {
            sort: {},
            filter: []
        }
    };

    constructor(
        private readonly _siteService: SiteService,
        private readonly _proSupplierService: ProfessionalSupplierService,
        private readonly _router: Router,
        private readonly _activatedRoute: ActivatedRoute
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.checkRegistrations();
    }

    private async checkRegistrations(): Promise<void> {
        try {
            // TODO: replace with real service call once getMyRegistrations is implemented
            // const registrations = await this._proSupplierService.getMyRegistrations();
            const registrations = [{ id: 1 }]; // temp mock - assume registered

            if (!registrations || registrations.length === 0) {
                this.hasRegistrations = false;
            }
        } catch {
            this.hasRegistrations = false;
        }
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }
        if(!searchForm.value.streetNumber) {
            this.errorMessage = 'The Property Street Number and Property Street Name fields are required.';
        }
        else{
            this.accountNumber = searchForm.value.accountNumber?.trim();
            this.streetNumber = searchForm.value.streetNumber?.trim();
        }
           
        await this.getSites();
        
        this.showResults = true;
    }

    public async getSites(): Promise<void> {
        try {
            this.table.isLoading = true;

            const filter: QueryProperty[] = [];

            if (this.accountNumber) {
                filter.push({
                    columnName: 'accountNumber',
                    comparisonOperator: 'Eq',
                    value: this.accountNumber
                });
            } else if (this.streetNumber) {
                filter.push({
                    columnName: 'streetNumber',
                    comparisonOperator: 'StW',
                    value: this.streetNumber
                });
            }

            this.table.query.filter = filter;

            this.table.items = await this._siteService.getAllForProfessional(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            //this.table.items =  { data: [], pageInfo: {} };
            this.table.isLoading = false;
        }
    }

    public viewSite(site: Site): void {
        this._router.navigate([site.id], { relativeTo: this._activatedRoute });
    }

    public submitUnlisted(): void {
        this._router.navigate([0], { relativeTo: this._activatedRoute });
    }

    private getColumns(): TableColumn<Site>[] {
        return [
            {
                field: 'accountNumber',
                caption: 'Account #',
                type: ColumnType.text
            },
            {
                field: 'propertyType',
                caption: 'Property Type',
                type: ColumnType.text
            },
            {
                field: 'propertyInformation',
                caption: 'Property Information',
                type: ColumnType.text
            },
            {
                field: 'contactInformation',
                caption: 'Contact Information',
                type: ColumnType.text
            },
            {
                field: 'action',
                caption: '',
                type: ColumnType.other,
            }
        ];
    }
}