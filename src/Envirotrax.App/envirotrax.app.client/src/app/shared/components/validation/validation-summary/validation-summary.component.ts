import { Component, ElementRef, Input, OnChanges, ViewChild } from "@angular/core";
import { NgForm } from "@angular/forms";

@Component({
    selector: 'vp-validation-summary',
    templateUrl: './validation-summary.component.html',
    standalone: false,
})
export class ValidationSummaryComponent implements OnChanges {
    private _errorCount: number = 0;

    @Input()
    public form: NgForm = null!;

    @Input()
    public validationErrors: string[] = [];

    @ViewChild('container')
    public element?: ElementRef<HTMLElement>;

    public ngOnChanges(): void {
        if (this._errorCount !== this.validationErrors.length) {
            if (this.element?.nativeElement) {
                this.element.nativeElement.scrollIntoView({
                    block: 'center',
                    inline: 'start',
                    behavior: 'smooth'
                });
            }

            this._errorCount = this.validationErrors.length;
        }
    }
}
