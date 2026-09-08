import { Component } from "@angular/core";

// Projects its content into a <vp-section> header's action area (e.g. buttons).
@Component({
    selector: 'vp-section-actions',
    standalone: false,
    template: '<ng-content></ng-content>',
    styles: `
        :host {
            display: flex;
            align-items: center;
            height: 100%;
        }

        :host ::ng-deep button:not(.btn) {
            --bs-btn-color: var(--bs-white);
            --bs-btn-bg: transparent;
            --bs-btn-hover-bg: rgba(255, 255, 255, 0.2);
            --bs-btn-border-color: var(--bs-border-color);
            --bs-btn-border-radius: var(--bs-border-radius);
            --bs-btn-padding-x: 0.25rem;
            --bs-btn-padding-y: 0.25rem;

            display: inline-flex;
            align-items: center;
            padding: var(--bs-btn-padding-y) var(--bs-btn-padding-x);
            line-height: 1.5;
            color: var(--bs-btn-color);
            background-color: var(--bs-btn-bg);
            border: var(--bs-border-width) var(--bs-border-style) var(--bs-btn-border-color);
            border-radius: var(--bs-btn-border-radius);
        }

        :host ::ng-deep button:not(.btn):hover {
            background-color: var(--bs-btn-hover-bg);
        }
    `,
})
export class SectionActionsComponent {
}
