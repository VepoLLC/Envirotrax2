import { Component, Input } from "@angular/core";
import { BackflowTest } from "../../../../../shared/models/backflow/backflow-test";

@Component({
    selector: 'vp-professional-backflow-test-property-info',
    standalone: false,
    templateUrl: './professional-backflow-test-property-info.component.html'
})
export class ProfessionalBackflowTestPropertyInfoComponent {
    @Input() public test!: BackflowTest;
}
