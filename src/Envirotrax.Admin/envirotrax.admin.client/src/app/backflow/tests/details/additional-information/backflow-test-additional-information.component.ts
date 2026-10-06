import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, Optional, SkipSelf } from '@angular/core';
import { ControlContainer, FormsModule, NgForm } from '@angular/forms';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { BackflowTestDetails } from '../../../../shared/models/backflow/backflow-test';

@Component({
    selector: 'vp-backflow-test-additional-information',
    templateUrl: './backflow-test-additional-information.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule],
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class BackflowTestAdditionalInformationComponent implements OnInit {
    @Input() public test: BackflowTestDetails = {};
    @Input() public form?: NgForm;
    @Input() public idPrefix: string = '';

    public isVisible: boolean = false;

    public ngOnInit(): void {
        this.isVisible = this.test.showRainSensor === true
            || this.test.showOSSF === true
            || this.test.showPermitNumber === true;
    }
}
