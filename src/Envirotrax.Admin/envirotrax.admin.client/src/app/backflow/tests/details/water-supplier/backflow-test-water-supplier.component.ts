import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { BackflowTestDetails } from '../../../../shared/models/backflow/backflow-test';

@Component({
    selector: 'vp-backflow-test-water-supplier',
    templateUrl: './backflow-test-water-supplier.component.html',
    imports: [CommonModule, SharedComponentsModule]
})
export class BackflowTestWaterSupplierComponent implements OnInit {
    @Input() public test: BackflowTestDetails = {};

    public header: string = 'Water Supplier';
    public cityStateZip: string = '';

    public ngOnInit(): void {
        const supplier = this.test.waterSupplier;

        this.header = supplier?.name ? `Water Supplier - ${supplier.name}` : 'Water Supplier';

        let result = supplier?.city ?? '';

        if (supplier?.state?.code) {
            result = result ? `${result}, ${supplier.state.code}` : supplier.state.code;
        }

        if (supplier?.zipCode) {
            result = result ? `${result}  ${supplier.zipCode}` : supplier.zipCode;
        }

        this.cityStateZip = result;
    }
}
