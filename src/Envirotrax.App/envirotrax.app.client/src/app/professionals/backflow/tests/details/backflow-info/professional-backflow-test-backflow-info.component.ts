import { Component, Input } from "@angular/core";
import { BackflowTest } from "../../../../../shared/models/backflow/backflow-test";
import { BackflowTestOptionsService } from "../../../../../shared/services/backflow/backflow-test-options.service";

@Component({
    selector: 'vp-professional-backflow-test-backflow-info',
    standalone: false,
    templateUrl: './professional-backflow-test-backflow-info.component.html'
})
export class ProfessionalBackflowTestBackflowInfoComponent {
    @Input() public test!: BackflowTest;

    constructor(private readonly _optionsService: BackflowTestOptionsService) { }

    public getDeviceTypeLabel(): string {
        if (!this.test?.deviceType) {
            return '';
        }
        return this._optionsService.deviceTypeOptions.find(o => o.id === this.test!.deviceType)?.text ?? this.test.deviceType;
    }
}
