import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { SubmissionValidationFlag } from '../../../shared/models/validations/submission-validation';

@Component({
    selector: 'vp-submission-validation-badge',
    templateUrl: './submission-validation-badge.component.html',
    imports: [
        CommonModule
    ],
    styles: [`
        .vp-validation-badge {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            flex-shrink: 0;
            width: 1.25rem;
            height: 1.25rem;
            border-radius: 0.25rem;
            color: #fff;
            font-size: 0.75rem;
            font-weight: 700;
            line-height: 1;
        }

        .vp-validation-badge-red { background-color: #e00000; }
        .vp-validation-badge-orange { background-color: #f5a000; }
        .vp-validation-badge-blue { background-color: #0d47d9; }
        .vp-validation-badge-green { background-color: #2e9e3e; }
        .vp-validation-badge-yellow { background-color: #f5c400; color: #000; }
        .vp-validation-badge-magenta { background-color: #d600d6; }
        .vp-validation-badge-outline-green { background-color: #fff; border: 2px solid #2e9e3e; color: #2e9e3e; }
    `]
})
export class SubmissionValidationBadgeComponent {
    @Input()
    public flag?: SubmissionValidationFlag;
}
