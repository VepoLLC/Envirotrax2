import { Component, Input } from "@angular/core";
import { BackflowTest } from "../../../../../shared/models/backflow/backflow-test";

@Component({
    selector: 'vp-professional-backflow-test-mailing-info',
    standalone: false,
    templateUrl: './professional-backflow-test-mailing-info.component.html'
})
export class ProfessionalBackflowTestMailingInfoComponent {
    @Input() public test!: BackflowTest;
}
