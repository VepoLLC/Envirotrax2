import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProfessionalFogInspectionService } from '../../../../shared/services/fog/professional-fog-inspection.service';
import { FogInspection } from '../../../../shared/models/fog/fog-inspection';
import { FogInspectionResult, FogReasonForInspection, fogReasonForInspectionLabels } from '../../../../shared/models/fog/fog-inspection-enums';
import { FacilityType, facilityTypeLabels } from '../../../../shared/enums/facility-type.enum';
import { PropertyType } from '../../../../shared/enums/property-type.enum';

@Component({
    standalone: false,
    templateUrl: './fog-inspection-view.component.html',
    styleUrl: './fog-inspection-view.component.scss'
})
export class FogInspectionViewComponent implements OnInit {
    public isLoading = true;
    public inspection?: FogInspection;

    public readonly FogInspectionResult = FogInspectionResult;
    public readonly PropertyType = PropertyType;

    constructor(
        private readonly _route: ActivatedRoute,
        private readonly _inspectionService: ProfessionalFogInspectionService
    ) {}

    public async ngOnInit(): Promise<void> {
        const idParam = this._route.snapshot.paramMap.get('id');

        if (!idParam) {
            this.isLoading = false;
            return;
        }

        try {
            this.isLoading = true;
            this.inspection = await this._inspectionService.getById(Number(idParam));
        } finally {
            this.isLoading = false;
        }
    }

    public getReasonLabel(reason?: number): string {
        if (reason == null) {
            return '';
        }
        return fogReasonForInspectionLabels[reason as FogReasonForInspection] ?? '';
    }

    public getFacilityTypeLabel(facilityType?: number): string {
        if (facilityType == null) {
            return '';
        }
        return facilityTypeLabels[facilityType as FacilityType] ?? '';
    }

    public getPercent(numerator?: string, denominator?: string): string {
        const n = parseFloat(numerator ?? '');
        const d = parseFloat(denominator ?? '');
        if (!isFinite(n) || !isFinite(d) || d === 0) {
            return '';
        }
        return Math.round((n / d) * 100) + '%';
    }
}
