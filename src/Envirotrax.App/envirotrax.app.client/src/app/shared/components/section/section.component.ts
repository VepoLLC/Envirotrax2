import { Component, Input } from "@angular/core";


@Component({
    selector: 'vp-section',
    standalone: false,
    templateUrl: './section.component.html'
})
export class SectionComponent {
    @Input()
    public isExpanded: boolean = true;

    @Input()
    public header: string = '';
}