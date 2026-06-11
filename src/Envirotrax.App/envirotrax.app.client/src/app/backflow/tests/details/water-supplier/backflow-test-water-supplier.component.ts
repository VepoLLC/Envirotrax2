import { Component, Input } from "@angular/core";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";

@Component({
    selector: 'vp-backflow-test-water-supplier',
    standalone: false,
    templateUrl: './backflow-test-water-supplier.component.html'
})
export class BackflowTestWaterSupplierComponent {
    @Input() public test!: BackflowTest;
}
