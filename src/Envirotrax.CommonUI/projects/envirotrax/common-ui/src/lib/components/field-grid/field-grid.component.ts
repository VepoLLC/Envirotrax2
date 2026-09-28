import { Component } from "@angular/core";

// Wrap horizontal-labelStyle vp-input/vp-filter-panel-field/app-gis-area-lookup/vp-field-grid-row fields
// (only those) in <vp-field-grid> so their label column auto-sizes to the widest label in the group,
// instead of a fixed Bootstrap column.
@Component({
    selector: 'vp-field-grid',
    standalone: false,
    template: `<ng-content></ng-content>`,
    host: {
        // d-xl-grid overrides d-flex at the xl breakpoint (Bootstrap generates its responsive
        // display utilities after the base ones, so the later rule wins the cascade tie).
        // row-gap-xl-1/column-gap-xl-3 likewise override the base gap-1 once this becomes a grid,
        // since this is the actual grid that ends up placing every field's label/control (they're
        // pass-through display:contents down to here) so the label-control gap must live here, not
        // on .vp-input-row.
        class: 'd-flex flex-column gap-1 d-xl-grid align-items-xl-start row-gap-xl-1 column-gap-xl-3'
    },
    styleUrl: './field-grid.component.css'
})
export class FieldGridComponent {
}
