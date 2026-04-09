import { Component, Input } from "@angular/core";

@Component({
    selector: 'vp-status-icon',
    templateUrl: './status-icon.component.html',
    standalone: false
})
export class StatusIconComponent {
    @Input()
    public imageSrc: string = null!;

    @Input()
    public iconClass: string = null!;

    @Input()
    public text: string = null!;
}
