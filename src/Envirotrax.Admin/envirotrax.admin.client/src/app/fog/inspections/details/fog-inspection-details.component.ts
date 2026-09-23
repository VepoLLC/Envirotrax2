import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RecordLog } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import {
    FogInspection,
    FogInspectionResult,
    FogReasonForInspection,
    fogReasonForInspectionLabels,
    InterceptorCapacityType,
    interceptorCapacityTypeLabels
} from '../../../shared/models/fog/fog-inspection';
import { PropertyType } from '../../../shared/models/sites/site';
import { FogInspectionOptionsService } from '../../../shared/services/fog/fog-inspection-options.service';
import { FogInspectionService } from '../../../shared/services/fog/fog-inspection.service';
import { WindowReference } from '../../../window/window-config';

type FogInspectionTab = 'results' | 'images' | 'logs';

/**
 * Read-only, matching V1's FOG inspection page and the water-supplier FOG view the App client already ships.
 * The ticket shows no Save, so there is deliberately no update path here - an editable window would need a
 * new admin update request, an UpdateForAdminAsync, a PUT shell and record-log writes, none of which exist.
 */
@Component({
    templateUrl: './fog-inspection-details.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
})
export class FogInspectionDetailsComponent implements OnInit {
    public id: number = 0;

    /** Namespaces DOM ids: the window container can hold several details windows at once. */
    public idPrefix: string = 'fog';

    public isLoading: boolean = false;
    public isLoadingRecordLogs: boolean = false;

    public inspection: FogInspection = {};
    public recordLogs: RecordLog[] = [];

    public selectedTab: FogInspectionTab = 'results';

    public readonly FogInspectionResult = FogInspectionResult;
    public readonly PropertyType = PropertyType;

    // Display values, computed once after load - never called from the template.
    public waterSupplierHeader: string = 'Water Supplier';
    public inspectorHeader: string = 'CSI Inspector';
    public reasonLabel: string = '';
    public facilityTypeLabel: string = '';
    public trapType: string = '';
    public trapCapacity: string = '';
    public propertyCityStateZip: string = '';
    public mailingCityStateZip: string = '';
    public inspectorCityStateZip: string = '';
    public inletGreaseLayerPercent: string = '';
    public inletSedimentLayerPercent: string = '';
    public outletGreaseLayerPercent: string = '';
    public outletSedimentLayerPercent: string = '';
    public recordLogTabTitle: string = 'Record Log';

    constructor(
        private readonly _windowReference: WindowReference<{ id?: number }>,
        private readonly _inspectionService: FogInspectionService,
        private readonly _optionsService: FogInspectionOptionsService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.id = this._windowReference.config.model?.id ?? 0;
        this.idPrefix = `fog-${this.id}`;

        await Promise.all([
            this.loadInspection(),
            this.loadRecordLogs()
        ]);
    }

    private async loadInspection(): Promise<void> {
        if (!this.id) {
            return;
        }

        try {
            this.isLoading = true;
            this.inspection = await this._inspectionService.get(this.id);
            this.setDisplayValues(this.inspection);
        } finally {
            this.isLoading = false;
        }
    }

    private async loadRecordLogs(): Promise<void> {
        if (!this.id) {
            return;
        }

        try {
            this.isLoadingRecordLogs = true;
            this.recordLogs = await this._inspectionService.getLogs(this.id);
            this.recordLogTabTitle = `Record Log (${this.recordLogs.length})`;
        } finally {
            this.isLoadingRecordLogs = false;
        }
    }

    private setDisplayValues(inspection: FogInspection): void {
        this.waterSupplierHeader = inspection.waterSupplier?.name
            ? `Water Supplier - ${inspection.waterSupplier.name}`
            : 'Water Supplier';

        this.reasonLabel = inspection.reasonForInspection == null
            ? ''
            : fogReasonForInspectionLabels[inspection.reasonForInspection as FogReasonForInspection] ?? '';

        // Reuses the label set the search panel already owns rather than adding another facility-type map.
        this.facilityTypeLabel = inspection.facilityType == null
            ? ''
            : this._optionsService.facilityTypeOptions.find(o => o.id === String(inspection.facilityType))?.text ?? '';

        this.trapType = [inspection.interceptorType, inspection.interceptorOtherDescription]
            .filter(part => part)
            .join(' ');

        // V1 resolves the unit rather than assuming gallons, which both existing V2 FOG views do.
        this.trapCapacity = inspection.interceptorCapacity == null
            ? ''
            : `${inspection.interceptorCapacity} ${interceptorCapacityTypeLabels[(inspection.interceptorCapacityType ?? InterceptorCapacityType.Gallons) as InterceptorCapacityType]}`;

        this.propertyCityStateZip = this.buildCityStateZip(inspection.propertyCity, inspection.propertyState?.code, inspection.propertyZip);
        this.mailingCityStateZip = this.buildCityStateZip(inspection.mailingCity, inspection.mailingState?.code, inspection.mailingZip);
        this.inspectorCityStateZip = this.buildCityStateZip(inspection.inspectorCity, inspection.inspectorState, inspection.inspectorZip);

        this.inletGreaseLayerPercent = this.getPercent(inspection.inletChamberGreaseBlanket, inspection.inletChamberWettingHeight);
        this.inletSedimentLayerPercent = this.getPercent(inspection.inletChamberSediments, inspection.inletChamberWettingHeight);
        this.outletGreaseLayerPercent = this.getPercent(inspection.outletChamberGreaseBlanket, inspection.outletChamberWettingHeight);
        this.outletSedimentLayerPercent = this.getPercent(inspection.outletChamberSediments, inspection.outletChamberWettingHeight);
    }

    private buildCityStateZip(city?: string, stateCode?: string, zip?: string): string {
        const cityPart = city ? `${city},` : '';

        return [cityPart, stateCode, zip].filter(part => part).join(' ').trim();
    }

    private getPercent(numerator?: string, denominator?: string): string {
        const n = parseFloat(numerator ?? '');
        const d = parseFloat(denominator ?? '');

        if (!isFinite(n) || !isFinite(d) || d === 0) {
            return '';
        }

        return `${Math.round((n / d) * 100)}%`;
    }
}
