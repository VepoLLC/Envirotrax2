import { Component, Input } from "@angular/core";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";

@Component({
    selector: 'vp-backflow-test-remarks',
    standalone: false,
    templateUrl: './backflow-test-remarks.component.html'
})
export class BackflowTestRemarksComponent {
    @Input() public test!: BackflowTest;
}
