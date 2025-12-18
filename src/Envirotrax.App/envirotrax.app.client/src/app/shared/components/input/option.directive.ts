import { Directive, ElementRef, Input, OnChanges, OnInit, SimpleChanges } from "@angular/core";

@Directive({
    selector: 'option',
    standalone: false,
})
export class OptionDirective implements OnInit, OnChanges {
    private _parentSelectElement: HTMLSelectElement | null = null;

    @Input()
    public value: any;

    constructor(private readonly _element: ElementRef<HTMLElement>) {

    }

    private setSelected(): void {
        if (this._parentSelectElement) {
            let parentValue = this._parentSelectElement.getAttribute('value');

            if (parentValue == this.value) {
                this._element.nativeElement.setAttribute('selected', 'selected');
            } else {
                this._element.nativeElement.removeAttribute('selected');
            }
        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if (changes['value']) {
            this.setSelected();
        }
    }

    public ngOnInit(): void {
        this._parentSelectElement = this._element.nativeElement.closest('select');
        this.setSelected();
    }
}