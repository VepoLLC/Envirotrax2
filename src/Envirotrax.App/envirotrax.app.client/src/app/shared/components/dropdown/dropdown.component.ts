import { ChangeDetectorRef, Component, ElementRef, Input, ViewChild } from "@angular/core";
import { createPopper, flip, Placement } from "@popperjs/core";

@Component({
    selector: 'vp-dropdown',
    templateUrl: './dropdown.component.html',
    standalone: false,
    styles: `
        .dp-backdrop {
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
        }
    `
})
export class DropdownComponent {
    @Input()
    public isExpanded: boolean = false;

    @Input()
    public label: string = null!;

    @Input()
    public title: string = null!;

    @Input()
    public iconCss: string = null!;

    @Input()
    public showCaret: boolean = false;

    @Input()
    public placement: Placement = "auto";

    @Input()
    public buttonClass?: string;

    @ViewChild('dropdownButton')
    public dropdownButton: ElementRef<HTMLElement> = null!;

    @ViewChild('dropdownContent')
    public dropdownContent: ElementRef<HTMLElement> = null!;

    constructor(private readonly _changeDetector: ChangeDetectorRef) {

    }

    public toggleExpanded(e: Event) {
        if (this.isExpanded) {
            this.isExpanded = false;
        } else {
            this.isExpanded = true;

            createPopper(this.dropdownButton.nativeElement, this.dropdownContent.nativeElement, {
                strategy: "fixed",
                placement: this.placement,
                modifiers: [
                    flip
                ],
            });

            e.stopPropagation();
            this._changeDetector.detectChanges();
        }
    }
}
