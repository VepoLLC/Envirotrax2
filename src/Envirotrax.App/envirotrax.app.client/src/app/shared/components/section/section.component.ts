import { Component, Input } from "@angular/core";


@Component({
    selector: 'vp-section',
    standalone: false,
    templateUrl: './section.component.html',
    styles: `
        h2 {
            font-weight: 500
        }
    `
})
export class SectionComponent {
    @Input()
    public isExpanded: boolean = true;

    @Input()
    public header: string = '';
}