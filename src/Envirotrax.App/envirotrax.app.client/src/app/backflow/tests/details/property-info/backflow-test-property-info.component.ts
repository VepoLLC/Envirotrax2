import { Component, EventEmitter, Input, Output } from "@angular/core";
import { Router } from "@angular/router";
import { NgForm } from "@angular/forms";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";
import { State } from "../../../../shared/models/lookup/state";
import { InputOption } from "@envirotrax/common-ui";

@Component({
    selector: 'vp-backflow-test-property-info',
    standalone: false,
    templateUrl: './backflow-test-property-info.component.html'
})
export class BackflowTestPropertyInfoComponent {
    @Input() public test!: BackflowTest;
    @Input() public canModify: boolean = false;
    @Input() public saving: boolean = false;
    @Input() public states: InputOption<State>[] = [];

    @Output() public save = new EventEmitter<NgForm>();

    public propertyTypeOptions: InputOption[] = [
        { id: 0, text: 'Residential' },
        { id: 1, text: 'Commercial' }
    ];

    constructor(private readonly _router: Router) { }

    public propertyStateChanged(stateId: number): void {
        if (this.test == null) {
            return;
        }
        this.test.propertyState = stateId ? { id: stateId } : null;
    }

    public viewPropertyRecord(): void {
        if (this.test?.site?.id == null) {
            return;
        }
        this._router.navigate(['/sites', this.test.site.id, 'edit']);
    }
}
