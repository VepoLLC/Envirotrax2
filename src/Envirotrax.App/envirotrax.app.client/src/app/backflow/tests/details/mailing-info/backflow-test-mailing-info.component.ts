import { Component, EventEmitter, Input, Output } from "@angular/core";
import { NgForm } from "@angular/forms";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";
import { State } from "../../../../shared/models/lookup/state";
import { InputOption } from "../../../../shared/components/input/input.component";

@Component({
    selector: 'vp-backflow-test-mailing-info',
    standalone: false,
    templateUrl: './backflow-test-mailing-info.component.html'
})
export class BackflowTestMailingInfoComponent {
    @Input() public test!: BackflowTest;
    @Input() public canModify: boolean = false;
    @Input() public saving: boolean = false;
    @Input() public states: InputOption<State>[] = [];

    @Output() public save = new EventEmitter<NgForm>();

    public mailingStateChanged(stateId: number): void {
        if (this.test == null) {
            return;
        }
        this.test.mailingState = stateId ? { id: stateId } : null;
    }

    public copyFromPropertyAddress(): void {
        if (this.test == null) {
            return;
        }

        this.test.mailingStreetNumber = this.test.propertyStreetNumber;
        this.test.mailingStreetName = this.test.propertyStreetName;
        this.test.mailingNumber = this.test.propertyNumber;
        this.test.mailingCity = this.test.propertyCity;
        this.test.mailingState = this.test.propertyState ? { ...this.test.propertyState } : null;
        this.test.mailingZip = this.test.propertyZip;
    }
}
