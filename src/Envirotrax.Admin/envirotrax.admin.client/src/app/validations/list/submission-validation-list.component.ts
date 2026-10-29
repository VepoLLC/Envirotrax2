import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { InputOption, QueryProperty } from '@envirotrax/common-ui';
import { BackflowTestDetailsComponent } from '../../backflow/tests/details/backflow-test-details.component';
import { CsiInspectionDetailsComponent } from '../../csi/inspections/details/csi-inspection-details.component';
import { FogInspectionDetailsComponent } from '../../fog/inspections/details/fog-inspection-details.component';
import { SharedComponentsModule } from '../../shared/components/shared.components.module';
import { BackflowTest } from '../../shared/models/backflow/backflow-test';
import { CsiInspection } from '../../shared/models/csi/csi-inspection';
import { FogInspection } from '../../shared/models/fog/fog-inspection';
import { FogTripTicket } from '../../shared/models/fog/fog-trip-ticket';
import { PropertyType } from '../../shared/models/sites/site';
import {
    SubmissionValidationFlag,
    SubmissionValidationInfoLine,
    SubmissionValidationReason,
    SubmissionValidationReasonFlag,
    SubmissionValidationRecord,
    SubmissionValidationRow,
    SubmissionValidationSource,
    SubmissionValidationTab
} from '../../shared/models/validations/submission-validation';
import { BackflowTestService } from '../../shared/services/backflow/backflow-test.service';
import { CsiInspectionService } from '../../shared/services/csi/csi-inspection.service';
import { FogInspectionService } from '../../shared/services/fog/fog-inspection.service';
import { FogTripTicketService } from '../../shared/services/fog/fog-trip-ticket.service';
import { WaterSupplierService } from '../../shared/services/water-suppliers/water-supplier.service';
import { SubmissionValidationBadgeComponent } from './badge/submission-validation-badge.component';
import {
    BACKFLOW_REASONS,
    getReasonFlags,
    NEEDS_VALIDATION_FLAG,
    ON_HOLD_FLAG,
    REASON_FLAGS,
    SITE_REASONS,
    TRIP_TICKET_REASONS
} from './submission-validation-flags';
import { SubmissionValidationTabComponent } from './tab/submission-validation-tab.component';

interface ValidationTabView {
    key: SubmissionValidationTab;
    title: string;
    count: number;

    // Null when none of the tab's reasons is selected, so nothing can match.
    filter: QueryProperty[] | null;

    source: SubmissionValidationSource;
}

@Component({
    templateUrl: './submission-validation-list.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        SubmissionValidationBadgeComponent,
        SubmissionValidationTabComponent
    ]
})
export class SubmissionValidationListComponent implements OnInit {
    private static windowCount: number = 0;

    public readonly idPrefix: string;

    public readonly needsValidationFlag: SubmissionValidationFlag = NEEDS_VALIDATION_FLAG;

    public readonly onHoldFlag: SubmissionValidationFlag = ON_HOLD_FLAG;

    // The V1 window lays the reason checkboxes out in two columns.
    public readonly reasonColumns: SubmissionValidationReasonFlag[][] = [REASON_FLAGS.slice(0, 4), REASON_FLAGS.slice(4)];

    public selectedReasons: Record<SubmissionValidationReason, boolean> = {
        [SubmissionValidationReason.NewSite]: true,
        [SubmissionValidationReason.SiteInformationChanged]: true,
        [SubmissionValidationReason.CreatedFromCsi]: true,
        [SubmissionValidationReason.DeviceInformationChanged]: true,
        [SubmissionValidationReason.DeviceMoved]: true,
        [SubmissionValidationReason.HazardTypeChanged]: true,
        [SubmissionValidationReason.ReceiverInformationChanged]: true,
        [SubmissionValidationReason.UnknownSerialNumber]: true
    };

    public validationOnHold: boolean = false;

    public showResults: boolean = false;

    public activeTab: SubmissionValidationTab = SubmissionValidationTab.CsiInspections;

    public tabs: ValidationTabView[] = [];

    public waterSupplierOptions: InputOption[] = [{ id: '', text: 'Any Value' }];

    public readonly propertyTypeOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: String(PropertyType.Residential), text: 'Residential' },
        { id: String(PropertyType.Commercial), text: 'Commercial' }
    ];

    private panelFilter: QueryProperty[] = [];

    constructor(
        private readonly _csiInspectionService: CsiInspectionService,
        private readonly _backflowTestService: BackflowTestService,
        private readonly _fogInspectionService: FogInspectionService,
        private readonly _fogTripTicketService: FogTripTicketService,
        private readonly _waterSupplierService: WaterSupplierService
    ) {
        SubmissionValidationListComponent.windowCount = SubmissionValidationListComponent.windowCount + 1;

        this.idPrefix = `submission-validation-${SubmissionValidationListComponent.windowCount}`;
    }

    public async ngOnInit(): Promise<void> {
        this.tabs = this.createTabs();

        await this.loadWaterSuppliers();
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.panelFilter = queryProperties;
    }

    public search(searchForm: NgForm): void {
        if (!searchForm.valid) {
            return;
        }

        this.refresh();

        this.showResults = true;
    }

    // Each tab reloads when it gets a new filter, so a search or a refresh is one request per record type.
    public refresh(): void {
        for (const tab of this.tabs) {
            tab.filter = this.buildTabFilter(tab);

            if (tab.filter == null) {
                tab.count = 0;
            }
        }
    }

    private buildTabFilter(tab: ValidationTabView): QueryProperty[] | null {
        const reasons = tab.source.reasons.filter(reason => this.selectedReasons[reason]);

        if (reasons.length === 0) {
            return null;
        }

        // V1: "Needs Validation" is always on, and "Validation On Hold" switches between the records on hold and
        // the ones still waiting, like the Replaced Assemblies window.
        const filter: QueryProperty[] = [
            ...this.panelFilter,
            { columnName: 'needsValidation', value: 'true', comparisonOperator: 'Eq' },
            { columnName: 'validationOnHold', value: String(this.validationOnHold), comparisonOperator: 'Eq' }
        ];

        filter.push({
            columnName: reasons[0],
            children: reasons.map(reason => ({
                columnName: reason,
                value: 'true',
                comparisonOperator: 'Eq',
                logicalOperator: 'Or'
            }))
        });

        return filter;
    }

    private createTabs(): ValidationTabView[] {
        return [
            {
                key: SubmissionValidationTab.CsiInspections,
                title: 'CSI Inspections',
                count: 0,
                filter: null,
                source: {
                    infoCaption: 'Inspection Information',
                    reasons: SITE_REASONS,
                    detailsComponent: CsiInspectionDetailsComponent,
                    actions: this._csiInspectionService,
                    load: async (pageInfo, query) => {
                        const inspections = await this._csiInspectionService.getAll(pageInfo, query);

                        return { ...inspections, data: inspections.data.map(inspection => this.toCsiRow(inspection)) };
                    }
                }
            },
            {
                key: SubmissionValidationTab.BackflowTests,
                title: 'Backflow Tests',
                count: 0,
                filter: null,
                source: {
                    infoCaption: 'Test Information',
                    reasons: BACKFLOW_REASONS,
                    detailsComponent: BackflowTestDetailsComponent,
                    actions: this._backflowTestService,
                    load: async (pageInfo, query) => {
                        const tests = await this._backflowTestService.getAll(pageInfo, query);

                        return { ...tests, data: tests.data.map(test => this.toBackflowRow(test)) };
                    }
                }
            },
            {
                key: SubmissionValidationTab.FogInspections,
                title: 'FOG Inspections',
                count: 0,
                filter: null,
                source: {
                    infoCaption: 'Inspection Information',
                    reasons: SITE_REASONS,
                    detailsComponent: FogInspectionDetailsComponent,
                    actions: this._fogInspectionService,
                    load: async (pageInfo, query) => {
                        const inspections = await this._fogInspectionService.getAll(pageInfo, query);

                        return { ...inspections, data: inspections.data.map(inspection => this.toFogInspectionRow(inspection)) };
                    }
                }
            },
            {
                // There is no Admin trip ticket details window yet, so these rows have no View action.
                key: SubmissionValidationTab.FogTripTickets,
                title: 'FOG Trip Tickets',
                count: 0,
                filter: null,
                source: {
                    infoCaption: 'Ticket Information',
                    reasons: TRIP_TICKET_REASONS,
                    actions: this._fogTripTicketService,
                    load: async (pageInfo, query) => {
                        const tickets = await this._fogTripTicketService.getAll(pageInfo, query, {});

                        return { ...tickets, data: tickets.data.map(ticket => this.toTripTicketRow(ticket)) };
                    }
                }
            }
        ];
    }

    private toCsiRow(inspection: CsiInspection): SubmissionValidationRow {
        return this.buildRow(inspection, SITE_REASONS, [
            { label: 'Inspected', date: inspection.inspectionDate },
            { label: 'Submitted', date: inspection.createdTime },
            { label: 'Inspector', text: inspection.inspectorCompanyName },
            { label: 'Contact', text: inspection.inspectorContactName }
        ]);
    }

    private toBackflowRow(test: BackflowTest): SubmissionValidationRow {
        const assembly = [test.manufacturer, test.model, test.size, test.deviceType]
            .filter(part => part)
            .join(' ');

        return this.buildRow(test, BACKFLOW_REASONS, [
            { label: 'Tested', date: test.testDate },
            { label: 'Submitted', date: test.createdTime },
            { label: 'Assembly', text: assembly },
            { label: 'Serial #', text: test.serialNumber },
            { label: 'BPAT', text: test.bpatCompanyName }
        ]);
    }

    private toFogInspectionRow(inspection: FogInspection): SubmissionValidationRow {
        return this.buildRow(inspection, SITE_REASONS, [
            { label: 'Inspected', date: inspection.inspectionDate },
            { label: 'Submitted', date: inspection.createdTime },
            { label: 'Inspector', text: inspection.inspectorCompanyName },
            { label: 'Contact', text: inspection.inspectorContactName }
        ]);
    }

    private toTripTicketRow(ticket: FogTripTicket): SubmissionValidationRow {
        return this.buildRow(ticket, TRIP_TICKET_REASONS, [
            { label: 'Waste Removed', date: ticket.interceptorWasteRemovedDate },
            { label: 'Submitted', date: ticket.createdTime },
            { label: 'Transporter', text: ticket.transporterCompanyName },
            { label: 'Receiver', text: ticket.receiverCompanyName }
        ]);
    }

    private buildRow(
        record: SubmissionValidationRecord,
        reasons: SubmissionValidationReason[],
        infoLines: SubmissionValidationInfoLine[]
    ): SubmissionValidationRow {
        return {
            id: record.id,
            waterSupplier: record.waterSupplier,
            propertyType: record.propertyType,
            propertyBusinessName: record.propertyBusinessName,
            propertyAddress: this.buildPropertyAddress(record),
            propertyCityStateZip: this.buildCityStateZip(record),
            validationOnHold: record.validationOnHold ?? false,
            statusFlag: record.validationOnHold ? ON_HOLD_FLAG : NEEDS_VALIDATION_FLAG,
            reasonFlags: getReasonFlags(reasons).filter(flag => record[flag.reason]),
            infoLines: infoLines.filter(line => line.text || line.date)
        };
    }

    private buildPropertyAddress(record: SubmissionValidationRecord): string {
        const address = `${record.propertyStreetNumber ?? ''} ${record.propertyStreetName ?? ''}`.trim();

        return record.propertyNumber ? `${address} #${record.propertyNumber}`.trim() : address;
    }

    private buildCityStateZip(record: SubmissionValidationRecord): string {
        const city = record.propertyCity ? `${record.propertyCity},` : '';

        return `${city} ${record.propertyState?.code ?? ''} ${record.propertyZip ?? ''}`.trim();
    }

    private async loadWaterSuppliers(): Promise<void> {
        const options = await this._waterSupplierService.getAllAsOptions();

        this.waterSupplierOptions = [{ id: '', text: 'Any Value' }, ...options];
    }
}
