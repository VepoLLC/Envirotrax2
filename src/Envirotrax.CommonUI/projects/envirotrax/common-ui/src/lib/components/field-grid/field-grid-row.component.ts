import { Component } from "@angular/core";

// A label + arbitrary content row for use inside <vp-field-grid>, for cases where the control
// side isn't a single vp-input (e.g. a group of fields), so its label still aligns with the grid.
// Usage: <vp-field-grid-row><vp-field-grid-label>...</vp-field-grid-label><vp-field-grid-control>...</vp-field-grid-control></vp-field-grid-row>
@Component({
    selector: 'vp-field-grid-row',
    standalone: false,
    templateUrl: './field-grid-row.component.html',
    styleUrl: './field-grid-row.component.css'
})
export class FieldGridRowComponent {
}
