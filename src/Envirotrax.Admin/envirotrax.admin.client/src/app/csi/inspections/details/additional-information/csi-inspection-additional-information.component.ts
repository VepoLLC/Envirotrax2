import { Component, Input, Optional, SkipSelf } from '@angular/core';
import { ControlContainer, FormsModule } from '@angular/forms';
import { CsiInspectionDetails } from '../../../../shared/models/csi/csi-inspection';

@Component({
    selector: 'vp-csi-inspection-additional-information',
    templateUrl: './csi-inspection-additional-information.component.html',
    imports: [FormsModule],
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class CsiInspectionAdditionalInformationComponent {
    @Input() public inspection: CsiInspectionDetails = {};
    @Input() public idPrefix: string = '';
}
