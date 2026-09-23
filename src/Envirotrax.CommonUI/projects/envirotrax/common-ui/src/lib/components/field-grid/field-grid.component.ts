import { Component } from "@angular/core";

// Wrap horizontal-labelStyle vp-input/vp-filter-panel-field/app-gis-area-lookup fields (only those) in
// <vp-field-grid> so their label column auto-sizes to the widest label instead of a fixed Bootstrap column.
@Component({
    selector: 'vp-field-grid',
    standalone: false,
    template: `<ng-content></ng-content>`,
    host: {
        class: 'd-flex flex-column gap-2'
    },
    styles: `
        @media (min-width: 1200px) {
            :host {
                display: grid !important;
                grid-template-columns: max-content 1fr;
                align-items: start;
            }
        }
    `
})
export class FieldGridComponent {
}
