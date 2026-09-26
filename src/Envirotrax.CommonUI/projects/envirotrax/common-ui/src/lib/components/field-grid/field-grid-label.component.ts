import { Component } from "@angular/core";

// Marks projected content as the label side of a <vp-field-grid-row>.
@Component({
    selector: 'vp-field-grid-label',
    standalone: false,
    template: `<ng-content></ng-content>`,
    styleUrl: './field-grid-contents.css'
})
export class FieldGridLabelComponent {
}
