import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, Optional, SkipSelf } from '@angular/core';
import { ControlContainer, FormsModule, NgForm } from '@angular/forms';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { BackflowTestDetails } from '../../../../shared/models/backflow/backflow-test';

@Component({
    selector: 'vp-backflow-test-validation-notes',
    templateUrl: './backflow-test-validation-notes.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule],
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class BackflowTestValidationNotesComponent implements OnInit {
    @Input() public test: BackflowTestDetails = {};
    @Input() public form?: NgForm;

    public isExpanded: boolean = false;

    public ngOnInit(): void {
        this.isExpanded = !!this.test.validationNotes;
    }
}
