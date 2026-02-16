import { Component, Input } from "@angular/core";

@Component({
    standalone: false,
    selector: 'vp-input-option',
    template: `
        <ng-option [value]="value">
            <ng-content></ng-content>
        </ng-option>
    `
})
export class InputOptionComponent {
    @Input()
    public value: any;
}