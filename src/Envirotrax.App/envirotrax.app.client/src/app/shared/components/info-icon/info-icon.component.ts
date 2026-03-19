import { Component, ElementRef, Input, ViewChild } from "@angular/core";
import { createPopper, Instance } from "@popperjs/core";

@Component({
    selector: 'vp-info-icon',
    templateUrl: './info-icon.component.html',
    standalone: false,
    styles: `
        .vp-info-icon-trigger {
            cursor: default;
        }

        .vp-info-icon-tooltip {
            padding: 6px 10px;
            border-radius: 4px;
            font-size: 0.8rem;
            max-width: 250px;
            z-index: 9999;
            pointer-events: none;
            word-wrap: break-word;
        }

        .vp-info-icon-tooltip[data-hidden] {
            display: none;
        }
    `
})
export class InfoIconComponent {
    @Input()
    public text: string = '';

    @ViewChild('iconRef')
    public iconRef: ElementRef<HTMLElement> = null!;

    @ViewChild('tooltipRef')
    public tooltipRef: ElementRef<HTMLElement> = null!;

    private _popperInstance: Instance | null = null;

    public show(): void {
        this.tooltipRef.nativeElement.removeAttribute('data-hidden');
        this._popperInstance = createPopper(this.iconRef.nativeElement, this.tooltipRef.nativeElement, {
            strategy: 'fixed',
            placement: 'top',
            modifiers: [
                { name: 'offset', options: { offset: [0, 8] } }
            ]
        });
    }

    public hide(): void {
        this.tooltipRef.nativeElement.setAttribute('data-hidden', '');
        if (this._popperInstance) {
            this._popperInstance.destroy();
            this._popperInstance = null;
        }
    }
}