import { Component, Input } from "@angular/core";

@Component({
    selector: 'vp-dropdown-option',
    templateUrl: './dropdown-option.component.html',
    standalone: false,
})
export class DropdownOptionComponent {
    @Input()
    public routerLink: any;

    @Input()
    public queryParams: any;

    @Input()
    public state: any;
}
