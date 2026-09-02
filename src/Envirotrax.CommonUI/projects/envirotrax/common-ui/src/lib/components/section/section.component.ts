import { Component, Input } from "@angular/core";


@Component({
    selector: 'vp-section',
    standalone: false,
    templateUrl: './section.component.html',
    styles: `
        h2 {
            font-weight: 500
        }

        :host ::ng-deep vp-section-actions button:not(.btn) {
            --bs-btn-color: var(--bs-white);
            --bs-btn-bg: transparent;
            --bs-btn-hover-bg: rgba(255, 255, 255, 0.2);
            --bs-btn-border-color: var(--bs-border-color);
            --bs-btn-border-radius: var(--bs-border-radius);
            --bs-btn-padding-x: 0.25rem;
            --bs-btn-padding-y: 0;

            display: inline-flex;
            align-items: center;
            padding: var(--bs-btn-padding-y) var(--bs-btn-padding-x);
            line-height: 1.5;
            color: var(--bs-btn-color);
            background-color: var(--bs-btn-bg);
            border: var(--bs-border-width) var(--bs-border-style) var(--bs-btn-border-color);
            border-radius: var(--bs-btn-border-radius);
        }

        :host ::ng-deep vp-section-actions button:not(.btn):hover {
            background-color: var(--bs-btn-hover-bg);
        }
    `
})
export class SectionComponent {
    @Input()
    public isExpanded: boolean = true;

    @Input()
    public header: string = '';

    @Input()
    public noPadding: boolean = false;

    @Input()
    public collapsible: boolean = true;
}
