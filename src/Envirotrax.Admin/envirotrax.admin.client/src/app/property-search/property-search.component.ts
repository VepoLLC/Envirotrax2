import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { CellTemplateData, ColumnType, MAX_PAGE_SIZE, PageInfo, QueryProperty, TableColumn, TableViewModel } from '@envirotrax/common-ui';
import { FacilityType, FogCompliancyStatus, GreaseTrapType, PropertyType, Site } from '../shared/models/sites/site';
import { SiteService } from '../shared/services/sites/site.service';
import { WaterSupplier } from '../shared/models/water-suppliers/water-supplier';
import { WaterSupplierService } from '../shared/services/water-suppliers/water-supplier.service';

interface PropertySearchCriteria {
    waterSupplierId: number | null;
    siteId: string;
    accountNumber: string;
    active: boolean | null;
    invalidMailingAddress: boolean | null;
    outOfArea: boolean | null;
    isFeeExempt: boolean | null;

    needsCsiInspection: string;
    csiRenewalFromDate: string;
    csiRenewalToDate: string;

    fogCompliancyStatus: FogCompliancyStatus | null;

    needsFogInspection: string;
    fogInspectionFromDate: string;
    fogInspectionToDate: string;

    needsFogPermit: string;
    fogPermitFromDate: string;
    fogPermitToDate: string;

    facilityType: FacilityType | null;
    greaseTrapType: GreaseTrapType | null;
    hasOnSiteSewageFacility: boolean | null;
    hasAuxWaterSupply: boolean | null;
    hasFireSystem: boolean | null;
    fireSeparateWater: boolean | null;
    hasGritTrap: boolean | null;
    hasIrrigation: boolean | null;
    irrigationSeparateWater: boolean | null;
    hasDomesticPremisesIsolation: boolean | null;

    propertyType: PropertyType | null;
    businessName: string;
    streetNumber: string;
    streetName: string;
    propertyNumber: string;

    mailingCompanyName: string;
    mailingContactName: string;
    mailingStreetNumber: string;
    mailingStreetName: string;
    mailingNumber: string;
}

@Component({
    templateUrl: './property-search.component.html',
    standalone: false,
})
export class PropertySearchComponent implements OnInit {
    @ViewChild('statusCell', { static: true })
    private statusCellTemplate!: TemplateRef<CellTemplateData<Site>>;

    @ViewChild('propertyTypeCell', { static: true })
    private propertyTypeCellTemplate!: TemplateRef<CellTemplateData<Site>>;

    public readonly propertyType = PropertyType;

    public criteria: PropertySearchCriteria = this.getEmptyCriteria();
    public waterSuppliers: WaterSupplier[] = [];
    public maxRecords: number = 500;
    public criteriaExpanded: boolean = true;
    public hasSearched: boolean = false;

    private activeFogCompliancyStatus: FogCompliancyStatus | null = null;

    public table: TableViewModel<Site> = {
        query: {
            sort: {},
            filter: []
        }
    };

    public readonly triStateOptions = [
        { label: '', value: null },
        { label: 'True', value: true },
        { label: 'False', value: false }
    ];

    public readonly needsInspectionOptions = [
        { label: '', value: '' },
        { label: 'True - Any Date', value: 'true-any' },
        { label: 'True - Date Range', value: 'true-range' },
        { label: 'False', value: 'false' }
    ];

    public readonly facilityTypeOptions = [
        { label: 'Any Value', value: null },
        { label: 'Restaurant', value: FacilityType.Restaurant },
        { label: 'Fast Food Establishment', value: FacilityType.FastFoodEstablishment },
        { label: 'Hotel/Motel', value: FacilityType.HotelMotel },
        { label: 'Car Wash', value: FacilityType.CarWash },
        { label: 'School/University', value: FacilityType.SchoolUniversity },
        { label: 'Grocery Store', value: FacilityType.GroceryStore },
        { label: 'Convenience Store', value: FacilityType.ConvenienceStore },
        { label: 'Assisted Living Facility', value: FacilityType.AssistedLivingFacility },
        { label: 'Medical Facility', value: FacilityType.MedicalFacility },
        { label: 'Industrial', value: FacilityType.Industrial },
        { label: 'City Owned Facility', value: FacilityType.CityOwnedFacility },
        { label: 'Other', value: FacilityType.Other }
    ];

    public readonly greaseTrapOptions = [
        { label: 'Any Value', value: null },
        { label: 'Trap Not Required', value: GreaseTrapType.TrapNotRequired },
        { label: 'Has Grease Trap', value: GreaseTrapType.HasGreaseTrap },
        { label: 'Should Have Grease Trap', value: GreaseTrapType.ShouldHaveGreaseTrap },
        { label: 'Might Have Grease Trap', value: GreaseTrapType.MightHaveGreaseTrap }
    ];

    public readonly propertyTypeOptions = [
        { label: '', value: null },
        { label: 'Residential', value: PropertyType.Residential },
        { label: 'Commercial', value: PropertyType.Commercial }
    ];

    public readonly fogCompliancyStatusOptions = [
        { label: 'Any Value', value: null },
        { label: 'Compliant', value: FogCompliancyStatus.Compliant },
        { label: 'Out of Compliance', value: FogCompliancyStatus.OutOfCompliance }
    ];

    constructor(
        private readonly _siteService: SiteService,
        private readonly _waterSupplierService: WaterSupplierService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.loadWaterSuppliers();
    }

    public search(): void {
        this.table.query = {
            sort: {},
            filter: this.buildFilter()
        };

        this.activeFogCompliancyStatus = this.criteria.fogCompliancyStatus;

        this.table.items = undefined;
        this.criteriaExpanded = false;

        this.getSites({ pageSize: this.maxRecords });
    }

    public async getSites(pageInfo?: PageInfo): Promise<void> {
        const page = pageInfo ?? this.table.items?.pageInfo ?? { pageSize: this.maxRecords };

        try {
            this.table.isLoading = true;
            this.table.items = await this._siteService.getAll(page, this.table.query, this.activeFogCompliancyStatus);
            this.hasSearched = true;
        } finally {
            this.table.isLoading = false;
        }
    }

    private async loadWaterSuppliers(): Promise<void> {
        const result = await this._waterSupplierService.getAll(
            { pageSize: MAX_PAGE_SIZE },
            { sort: { name: 'Asc' }, filter: [] }
        );

        this.waterSuppliers = result.data ?? [];
    }

    private buildFilter(): QueryProperty[] {
        const filter: QueryProperty[] = [];
        const c = this.criteria;

        if (c.waterSupplierId != null) {
            filter.push({ columnName: 'waterSupplier.id', value: String(c.waterSupplierId), comparisonOperator: 'Eq' });
        }

        if (c.siteId) {
            filter.push({ columnName: 'id', value: c.siteId.trim(), comparisonOperator: 'Eq' });
        }

        if (c.accountNumber) {
            filter.push({ columnName: 'accountNumber', value: c.accountNumber.trim(), comparisonOperator: 'Eq' });
        }

        this.pushBool(filter, 'active', c.active);
        this.pushBool(filter, 'invalidMailingAddress', c.invalidMailingAddress);
        this.pushBool(filter, 'outOfArea', c.outOfArea);
        this.pushBool(filter, 'isFeeExempt', c.isFeeExempt);

        this.pushNeeds(filter, 'needsCsiInspection', 'csiRenewalDate', c.needsCsiInspection, c.csiRenewalFromDate, c.csiRenewalToDate);
        this.pushNeeds(filter, 'needsFogInspection', 'fogInspectionExpirationDate', c.needsFogInspection, c.fogInspectionFromDate, c.fogInspectionToDate);
        this.pushNeeds(filter, 'needsFogPermit', 'fogPermitExpirationDate', c.needsFogPermit, c.fogPermitFromDate, c.fogPermitToDate);

        this.pushEnum(filter, 'facilityType', c.facilityType);
        this.pushEnum(filter, 'greaseTrapType', c.greaseTrapType);

        this.pushBool(filter, 'hasOnSiteSewageFacility', c.hasOnSiteSewageFacility);
        this.pushBool(filter, 'hasAuxWaterSupply', c.hasAuxWaterSupply);
        this.pushBool(filter, 'hasFireSystem', c.hasFireSystem);
        this.pushBool(filter, 'fireSeparateWater', c.fireSeparateWater);
        this.pushBool(filter, 'hasGritTrap', c.hasGritTrap);
        this.pushBool(filter, 'hasIrrigation', c.hasIrrigation);
        this.pushBool(filter, 'irrigationSeparateWater', c.irrigationSeparateWater);
        this.pushBool(filter, 'hasDomesticPremisesIsolation', c.hasDomesticPremisesIsolation);

        this.pushEnum(filter, 'propertyType', c.propertyType);

        this.pushText(filter, 'businessName', c.businessName, 'Ct');
        this.pushText(filter, 'streetNumber', c.streetNumber, 'Ct');
        this.pushText(filter, 'streetName', c.streetName, 'Ct');
        this.pushText(filter, 'propertyNumber', c.propertyNumber, 'Eq');

        this.pushText(filter, 'mailingCompanyName', c.mailingCompanyName, 'Ct');
        this.pushText(filter, 'mailingContactName', c.mailingContactName, 'Ct');
        this.pushText(filter, 'mailingStreetNumber', c.mailingStreetNumber, 'Ct');
        this.pushText(filter, 'mailingStreetName', c.mailingStreetName, 'Ct');
        this.pushText(filter, 'mailingNumber', c.mailingNumber, 'Eq');

        return filter;
    }

    private pushBool(filter: QueryProperty[], columnName: string, value: boolean | null): void {
        if (value !== null) {
            filter.push({ columnName, value: String(value), comparisonOperator: 'Eq' });
        }
    }

    private pushEnum(filter: QueryProperty[], columnName: string, value: number | null): void {
        if (value !== null && value !== undefined) {
            filter.push({ columnName, value: String(value), comparisonOperator: 'Eq' });
        }
    }

    private pushText(filter: QueryProperty[], columnName: string, value: string, operator: 'Ct' | 'Eq'): void {
        if (value && value.trim()) {
            filter.push({ columnName, value: value.trim(), comparisonOperator: operator });
        }
    }

    private pushNeeds(filter: QueryProperty[], boolColumn: string, dateColumn: string, selection: string, fromDate: string, toDate: string): void {
        if (selection === 'true-any' || selection === 'true-range') {
            filter.push({ columnName: boolColumn, value: 'true', comparisonOperator: 'Eq' });
        } else if (selection === 'false') {
            filter.push({ columnName: boolColumn, value: 'false', comparisonOperator: 'Eq' });
        }

        if (selection === 'true-range') {
            if (fromDate) {
                filter.push({ columnName: dateColumn, value: new Date(fromDate + 'T00:00:00').toISOString(), comparisonOperator: 'Gte' });
            }

            if (toDate) {
                filter.push({ columnName: dateColumn, value: new Date(toDate + 'T23:59:59.999').toISOString(), comparisonOperator: 'Lte' });
            }
        }
    }

    private getColumns(): TableColumn<Site>[] {
        return [
            { field: 'id', caption: 'ID', type: ColumnType.number },
            { field: 'status', caption: '', type: ColumnType.other, cellTemplate: this.statusCellTemplate, queryColumnExcluded: true },
            { field: 'waterSupplier.name', caption: 'Water Supplier', type: ColumnType.text },
            { field: 'propertyType', caption: 'Type', type: ColumnType.other, cellTemplate: this.propertyTypeCellTemplate, queryColumnExcluded: true },
            { field: 'businessName', caption: 'Business Name', type: ColumnType.text },
            { field: 'streetNumber', caption: 'P St #', type: ColumnType.text },
            { field: 'streetName', caption: 'P Street Name', type: ColumnType.text },
            { field: 'propertyNumber', caption: 'P #', type: ColumnType.text },
            { field: 'city', caption: 'City', type: ColumnType.text },
            { field: 'state.code', caption: 'ST', type: ColumnType.text }
        ];
    }

    private getEmptyCriteria(): PropertySearchCriteria {
        return {
            waterSupplierId: null,
            siteId: '',
            accountNumber: '',
            active: null,
            invalidMailingAddress: null,
            outOfArea: null,
            isFeeExempt: null,

            needsCsiInspection: '',
            csiRenewalFromDate: '',
            csiRenewalToDate: '',

            fogCompliancyStatus: null,

            needsFogInspection: '',
            fogInspectionFromDate: '',
            fogInspectionToDate: '',

            needsFogPermit: '',
            fogPermitFromDate: '',
            fogPermitToDate: '',

            facilityType: null,
            greaseTrapType: null,
            hasOnSiteSewageFacility: null,
            hasAuxWaterSupply: null,
            hasFireSystem: null,
            fireSeparateWater: null,
            hasGritTrap: null,
            hasIrrigation: null,
            irrigationSeparateWater: null,
            hasDomesticPremisesIsolation: null,

            propertyType: null,
            businessName: '',
            streetNumber: '',
            streetName: '',
            propertyNumber: '',

            mailingCompanyName: '',
            mailingContactName: '',
            mailingStreetNumber: '',
            mailingStreetName: '',
            mailingNumber: ''
        };
    }
}
