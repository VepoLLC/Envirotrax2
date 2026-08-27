import { Component, EventEmitter, Input, Output } from "@angular/core";
import { NgForm } from "@angular/forms";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";
import { BackflowTestOptionsService } from "../../../../shared/services/backflow/backflow-test-options.service";
import { InputOption } from "@envirotrax/common-ui";

@Component({
    selector: 'vp-backflow-test-backflow-info',
    standalone: false,
    templateUrl: './backflow-test-backflow-info.component.html'
})
export class BackflowTestBackflowInfoComponent {
    @Input() public test!: BackflowTest;
    @Input() public canModify: boolean = false;
    @Input() public saving: boolean = false;

    @Output() public save = new EventEmitter<NgForm>();

    public hazardTypeOptions: InputOption[] = [];

    constructor(private readonly _optionsService: BackflowTestOptionsService) {
        this.hazardTypeOptions = [{ id: '', text: '' }, ...this._optionsService.hazardTypeOptions];
    }

    public getDeviceTypeLabel(): string {
        if (!this.test?.deviceType) {
            return '';
        }
        return this._optionsService.deviceTypeOptions.find(o => o.id === this.test!.deviceType)?.text ?? this.test.deviceType;
    }
}
