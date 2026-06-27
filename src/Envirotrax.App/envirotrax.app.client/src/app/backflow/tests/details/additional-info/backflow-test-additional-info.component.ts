import { Component, EventEmitter, Input, Output } from "@angular/core";
import { NgForm } from "@angular/forms";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";

@Component({
    selector: 'vp-backflow-test-additional-info',
    standalone: false,
    templateUrl: './backflow-test-additional-info.component.html',
    styleUrls: ['./backflow-test-additional-info.component.scss']
})
export class BackflowTestAdditionalInfoComponent {
    @Input() public test!: BackflowTest;
    @Input() public canModify: boolean = false;
    @Input() public saving: boolean = false;
    @Input() public showRainSensor: boolean = false;
    @Input() public showOSSF: boolean = false;
    @Input() public showPermitNumber: boolean = false;

    @Output() public save = new EventEmitter<NgForm>();
}
