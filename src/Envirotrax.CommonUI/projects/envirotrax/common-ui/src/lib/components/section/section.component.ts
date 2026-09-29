import { Component, Input } from "@angular/core";


@Component({
    selector: 'vp-section',
    standalone: false,
    templateUrl: './section.component.html',
    styles: `
        h2 {
            font-weight: 500
        }

        .vp-section-action-container .btn,
        .vp-section-action-container button {
            font-size: 0.875rem;
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
