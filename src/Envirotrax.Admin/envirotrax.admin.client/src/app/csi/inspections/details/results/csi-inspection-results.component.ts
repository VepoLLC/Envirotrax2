import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Optional, Output, SkipSelf } from '@angular/core';
import { ControlContainer, FormsModule, NgForm } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import {
    CsiInspectionDetails,
    CsiInspectionReason,
    csiInspectionReasonLabels
} from '../../../../shared/models/csi/csi-inspection';

export interface ComplianceItem {
    number: number;
    text: string;
    isCompliant: boolean;
}

@Component({
    selector: 'vp-csi-inspection-results',
    templateUrl: './csi-inspection-results.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule],
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class CsiInspectionResultsComponent {
    @Input() public inspection: CsiInspectionDetails = {};
    @Input() public form?: NgForm;
    @Input() public idPrefix: string = '';
    @Input() public complianceItems: ComplianceItem[] = [];

    @Input() public reasonForInspectionId: string = '';
    @Output() public reasonForInspectionIdChange: EventEmitter<string> = new EventEmitter<string>();

    @Input() public inspectionDate: string = '';
    @Output() public inspectionDateChange: EventEmitter<string> = new EventEmitter<string>();

    public readonly reasonOptions: InputOption[] = [
        {
            id: String(CsiInspectionReason.NewConstruction),
            text: csiInspectionReasonLabels[CsiInspectionReason.NewConstruction]
        },
        {
            id: String(CsiInspectionReason.ExistingServiceContaminantHazardsSuspected),
            text: csiInspectionReasonLabels[CsiInspectionReason.ExistingServiceContaminantHazardsSuspected]
        },
        {
            id: String(CsiInspectionReason.MajorRenovationOrExpansion),
            text: csiInspectionReasonLabels[CsiInspectionReason.MajorRenovationOrExpansion]
        }
    ];
}
