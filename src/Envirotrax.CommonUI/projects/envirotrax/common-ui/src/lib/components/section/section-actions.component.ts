import { Component } from "@angular/core";

// Projects its content into a <vp-section> header's action area (e.g. buttons).
@Component({
    selector: 'vp-section-actions',
    standalone: false,
    template: '<ng-content></ng-content>'
})
export class SectionActionsComponent {
}
