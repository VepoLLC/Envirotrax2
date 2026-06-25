import { Component, Input, OnChanges } from '@angular/core';
import { FogInspection } from '../../../models/fog/fog-inspection';
import { FogInspectionResult, FogReasonForInspection, fogReasonForInspectionLabels } from '../../../models/fog/fog-inspection-enums';
import { FacilityType, facilityTypeLabels } from '../../../enums/facility-type.enum';
import { PropertyType } from '../../../enums/property-type.enum';

@Component({
    selector: 'app-fog-inspection-details',
    standalone: false,
    templateUrl: './fog-inspection-details.component.html'
})
export class FogInspectionDetailsComponent implements OnChanges {
    @Input() public inspection?: FogInspection;
    @Input() public isLoading = false;

    public reasonLabel = '';
    public facilityTypeLabel = '';
    public inletGreaseLayerPercent = '';
    public inletSedimentLayerPercent = '';
    public outletGreaseLayerPercent = '';
    public outletSedimentLayerPercent = '';

    public readonly FogInspectionResult = FogInspectionResult;
    public readonly PropertyType = PropertyType;

    public ngOnChanges(): void {
        if (this.inspection) {
            this.setDisplayValues(this.inspection);
        }
    }

    private setDisplayValues(inspection: FogInspection): void {
        this.reasonLabel = this.getReasonLabel(inspection.reasonForInspection);
        this.facilityTypeLabel = this.getFacilityTypeLabel(inspection.facilityType);
        this.inletGreaseLayerPercent = this.getPercent(inspection.inletChamberGreaseBlanket, inspection.inletChamberWettingHeight);
        this.inletSedimentLayerPercent = this.getPercent(inspection.inletChamberSediments, inspection.inletChamberWettingHeight);
        this.outletGreaseLayerPercent = this.getPercent(inspection.outletChamberGreaseBlanket, inspection.outletChamberWettingHeight);
        this.outletSedimentLayerPercent = this.getPercent(inspection.outletChamberSediments, inspection.outletChamberWettingHeight);
    }

    private getReasonLabel(reason?: number): string {
        if (reason == null) {
            return '';
        }
        return fogReasonForInspectionLabels[reason as FogReasonForInspection] ?? '';
    }

    private getFacilityTypeLabel(facilityType?: number): string {
        if (facilityType == null) {
            return '';
        }
        return facilityTypeLabels[facilityType as FacilityType] ?? '';
    }

    private getPercent(numerator?: string, denominator?: string): string {
        const n = parseFloat(numerator ?? '');
        const d = parseFloat(denominator ?? '');
        if (!isFinite(n) || !isFinite(d) || d === 0) {
            return '';
        }
        return Math.round((n / d) * 100) + '%';
    }
}
