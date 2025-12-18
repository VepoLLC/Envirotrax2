import { Component, Input } from "@angular/core";
import { NgForm } from "@angular/forms";

@Component({
    selector: 'vp-validation-field',
    templateUrl: './validation-field.component.html',
    standalone: false,
})
export class ValidationFieldComponent {
    @Input()
    public form: NgForm = null!;

    @Input()
    public fieldName: string = null!;

    @Input()
    public fieldLabel: string = null!;
}
