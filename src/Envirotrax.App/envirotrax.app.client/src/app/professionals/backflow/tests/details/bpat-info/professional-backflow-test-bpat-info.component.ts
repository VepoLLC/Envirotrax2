import { Component, Input } from "@angular/core";
import { BackflowTest } from "../../../../../shared/models/backflow/backflow-test";

@Component({
    selector: 'vp-professional-backflow-test-bpat-info',
    standalone: false,
    templateUrl: './professional-backflow-test-bpat-info.component.html'
})
export class ProfessionalBackflowTestBpatInfoComponent {
    @Input() public test!: BackflowTest;
}
