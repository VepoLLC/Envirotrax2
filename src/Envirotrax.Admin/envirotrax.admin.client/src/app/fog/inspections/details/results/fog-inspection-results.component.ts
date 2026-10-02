import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, Optional, SkipSelf } from '@angular/core';
import { ControlContainer, FormsModule, NgForm } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import {
    FogInspection,
    FogInspectionResult,
    FogReasonForInspection
} from '../../../../shared/models/fog/fog-inspection';
import { FacilityType } from '../../../../shared/models/sites/site';
import { FogInspectionOptionsService } from '../../../../shared/services/fog/fog-inspection-options.service';

export function isInvalidChamberValue(value: string | undefined): boolean {
    if (value == null || value.trim() === '') {
        return false;
    }

    const parsed = Number(value);

    return isNaN(parsed) || parsed < 0;
}

@Component({
    selector: 'vp-fog-inspection-results',
    templateUrl: './fog-inspection-results.component.html',
    styles: [`
        .vp-field-invalid {
            border: 1px solid var(--bs-danger, #dc3545);
            border-radius: 0.25rem;
        }
    `],
    imports: [CommonModule, FormsModule, SharedComponentsModule],
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class FogInspectionResultsComponent implements OnInit {
    @Input() public inspection: FogInspection = {};
    @Input() public form?: NgForm;

    /** Namespaces radio ids: the window container can hold several details windows at once. */
    @Input() public idPrefix: string = 'fog';

    public readonly FogInspectionResult = FogInspectionResult;

    public readonly reasonOptions: InputOption[];
    public readonly facilityTypeOptions: InputOption[];
    public readonly sampledFromOptions: InputOption[];

    public reasonId: string = '';
    public facilityTypeId: string = '';

    public inletGreaseLayerPercent: number = 0;
    public inletSedimentLayerPercent: number = 0;
    public outletGreaseLayerPercent: number = 0;
    public outletSedimentLayerPercent: number = 0;

    public readonly isInvalidChamberValue = isInvalidChamberValue;

    constructor(private readonly _options: FogInspectionOptionsService) {
        this.reasonOptions = this._options.reasonOptions;
        this.facilityTypeOptions = this._options.facilityTypeOptions;
        this.sampledFromOptions = this._options.sampledFromOptions;
    }

    public ngOnInit(): void {
        this.reasonId = String(this.inspection.reasonForInspection ?? FogReasonForInspection.Scheduled);
        this.facilityTypeId = String(this.inspection.facilityType ?? FacilityType.Other);

        this.recalcCapacity();
    }

    public onReasonChange(value: string): void {
        this.inspection.reasonForInspection = Number(value) as FogReasonForInspection;
    }

    public onFacilityTypeChange(value: string): void {
        this.inspection.facilityType = Number(value);
    }

    public recalcCapacity(): void {
        const inlet = this.chamberPercents(
            this.inspection.inletChamberWettingHeight,
            this.inspection.inletChamberGreaseBlanket,
            this.inspection.inletChamberSediments);
        this.inletGreaseLayerPercent = Math.round(inlet.grease);
        this.inletSedimentLayerPercent = Math.round(inlet.sediment);
        this.inspection.inletTotalCapacityPercent = Math.round(inlet.total);

        const outlet = this.chamberPercents(
            this.inspection.outletChamberWettingHeight,
            this.inspection.outletChamberGreaseBlanket,
            this.inspection.outletChamberSediments);
        this.outletGreaseLayerPercent = Math.round(outlet.grease);
        this.outletSedimentLayerPercent = Math.round(outlet.sediment);
        this.inspection.outletTotalCapacityPercent = Math.round(outlet.total);

        let total = 0;

        if (inlet.total > 0 && outlet.total > 0) {
            total = (inlet.total + outlet.total) / 2;
        } else if (inlet.total > 0) {
            total = inlet.total;
        } else if (outlet.total > 0) {
            total = outlet.total;
        }

        this.inspection.totalCapacityPercent = Math.round(total);
    }

    private chamberPercents(wetting?: string, grease?: string, sediment?: string): { grease: number; sediment: number; total: number } {
        const w = Number(wetting);
        const g = Number(grease);
        const s = Number(sediment);

        let greasePct = (g / w) * 100;
        let sedimentPct = (s / w) * 100;
        let total = greasePct + sedimentPct;

        if (isNaN(total) || !isFinite(total)) {
            greasePct = 0;
            sedimentPct = 0;
            total = 0;
        }

        return { grease: greasePct, sediment: sedimentPct, total };
    }
}
