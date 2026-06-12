import { Component, Input } from "@angular/core";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";

@Component({
    selector: 'vp-backflow-test-bpat-info',
    standalone: false,
    templateUrl: './backflow-test-bpat-info.component.html'
})
export class BackflowTestBpatInfoComponent {
    @Input() public test!: BackflowTest;
}
