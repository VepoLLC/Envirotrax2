import { Component, DestroyRef, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { ProfessionalFogInspectionService } from '../../../../shared/services/fog/professional-fog-inspection.service';
import { FogInspection } from '../../../../shared/models/fog/fog-inspection';
import { FogInspectionResult, FogReasonForInspection, fogReasonForInspectionLabels } from '../../../../shared/models/fog/fog-inspection-enums';
import { FacilityType, facilityTypeLabels } from '../../../../shared/enums/facility-type.enum';
import { PropertyType } from '../../../../shared/enums/property-type.enum';

@Component({
    standalone: false,
    templateUrl: './fog-inspection-view.component.html'
})
export class FogInspectionViewComponent implements OnInit {
    public isLoading = true;
    public inspection?: FogInspection;

    public reasonLabel = '';
    public facilityTypeLabel = '';
    public inletGreaseLayerPercent = '';
    public inletSedimentLayerPercent = '';
    public outletGreaseLayerPercent = '';
    public outletSedimentLayerPercent = '';

    public readonly FogInspectionResult = FogInspectionResult;
    public readonly PropertyType = PropertyType;

    constructor(
        private readonly _destroyRef: DestroyRef,
        private readonly _route: ActivatedRoute,
        private readonly _inspectionService: ProfessionalFogInspectionService
    ) {}

    public ngOnInit(): void {
        this._route.paramMap
            .pipe(takeUntilDestroyed(this._destroyRef))
            .subscribe((params: ParamMap) => this.loadInspection(params.get('id')));
    }

    private async loadInspection(idParam: string | null): Promise<void> {
        if (!idParam) {
            this.isLoading = false;
            return;
        }

        try {
            this.isLoading = true;
            this.inspection = await this._inspectionService.getById(Number(idParam));
            this.setDisplayValues(this.inspection);
        } finally {
            this.isLoading = false;
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
