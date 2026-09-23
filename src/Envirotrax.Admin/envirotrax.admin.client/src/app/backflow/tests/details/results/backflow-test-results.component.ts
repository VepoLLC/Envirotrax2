import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import {
    BackflowReasonForTest,
    BackflowTestDetails,
    BackflowTestResult
} from '../../../../shared/models/backflow/backflow-test';
import { BackflowTestOptionsService } from '../../../../shared/services/backflow/backflow-test-options.service';
import { BackflowAssemblyVisibility, buildAssemblyVisibility } from '../backflow-test-visibility';

@Component({
    selector: 'vp-backflow-test-results',
    templateUrl: './backflow-test-results.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule]
})
export class BackflowTestResultsComponent implements OnInit {
    @Input() public test: BackflowTestDetails = {};
    @Input() public form?: NgForm;
    @Input() public idPrefix: string = '';
    @Input() public visibility: BackflowAssemblyVisibility = buildAssemblyVisibility(undefined);

    public testResultId: string = '';
    public reasonForTestId: string = '';
    public gaugeDescription: string = '';

    public readonly testResultOptions: InputOption[];
    public readonly reasonForTestOptions: InputOption[];

    constructor(private readonly _options: BackflowTestOptionsService) {
        this.testResultOptions = this._options.testResultFormOptions;
        this.reasonForTestOptions = this._options.reasonForTestFormOptions;
    }

    public ngOnInit(): void {
        this.testResultId = String(this.test.testResult ?? BackflowTestResult.Pass);
        this.reasonForTestId = String(this.test.reasonForTest ?? BackflowReasonForTest.AnnualTest);
        this.gaugeDescription = this.buildGaugeDescription();
    }

    public onTestResultChange(value: string): void {
        this.test.testResult = Number(value) as BackflowTestResult;
    }

    public onReasonForTestChange(value: string): void {
        this.test.reasonForTest = Number(value) as BackflowReasonForTest;
    }

    private buildGaugeDescription(): string {
        const parts: string[] = [];

        if (this.test.gaugeManufacturer) {
            parts.push(this.test.gaugeManufacturer);
        }

        if (this.test.gaugeModel) {
            parts.push(this.test.gaugeModel);
        }

        let result = parts.join(' ');

        if (result) {
            result = this.test.gaugeNonPotable ? `${result} (non-potable)` : `${result} (potable)`;
        }

        return result;
    }
}
