import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { BackflowTestDetails } from '../../../../shared/models/backflow/backflow-test';

@Component({
    selector: 'vp-backflow-test-bpat',
    templateUrl: './backflow-test-bpat.component.html',
    imports: [CommonModule, SharedComponentsModule]
})
export class BackflowTestBpatComponent implements OnInit {
    @Input() public test: BackflowTestDetails = {};

    public header: string = 'BPAT';
    public cityStateZip: string = '';

    public ngOnInit(): void {
        this.header = `BPAT - ${this.test.bpatCompanyName ?? ''} - ${this.test.bpatContactName ?? ''}`;

        let result = this.test.bpatCity ?? '';

        if (this.test.bpatState?.code) {
            result = result ? `${result}, ${this.test.bpatState.code}` : this.test.bpatState.code;
        }

        if (this.test.bpatZip) {
            result = result ? `${result}  ${this.test.bpatZip}` : this.test.bpatZip;
        }

        this.cityStateZip = result;
    }
}
