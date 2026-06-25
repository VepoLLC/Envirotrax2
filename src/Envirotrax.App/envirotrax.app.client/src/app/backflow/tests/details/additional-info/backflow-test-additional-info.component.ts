import { Component, EventEmitter, Input, Output } from "@angular/core";
import { NgForm } from "@angular/forms";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";

@Component({
    selector: 'vp-backflow-test-additional-info',
    standalone: false,
    templateUrl: './backflow-test-additional-info.component.html'
})
export class BackflowTestAdditionalInfoComponent {
    @Input() public test!: BackflowTest;
    @Input() public canModify: boolean = false;
    @Input() public saving: boolean = false;

    @Output() public save = new EventEmitter<NgForm>();
}
