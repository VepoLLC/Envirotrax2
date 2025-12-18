import { DatePipe } from "@angular/common";
import { AfterViewInit, Component, ElementRef, forwardRef, Input, OnInit, ViewChild } from "@angular/core";
import { ControlValueAccessor, NgForm, NG_VALUE_ACCESSOR } from "@angular/forms";
import flatpickr from "flatpickr";
import { Instance } from "flatpickr/dist/types/instance";

@Component({
    selector: 'vp-input',
    templateUrl: './input.component.html',
    standalone: false,
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => InputComponent),
            multi: true
        },
        DatePipe
    ],
    styles: `
        .form-floating > textarea.form-control {
            height: auto;
        }
    `
})
export class InputComponent implements ControlValueAccessor, OnInit, AfterViewInit {
    private _onChanged: (value: any) => void = null!;
    private _onTouched: (event: FocusEvent) => void = null!;

    private _flatpickerInstance?: Instance

    private static _counter: number = 0;

    @Input()
    public id: string = null!;

    @Input()
    public name: string = null!;

    @Input()
    public type: 'text' | 'number' | 'date' | 'datetime' | 'textarea' | 'select' | 'email' = 'text';

    @Input()
    public required: boolean = false;

    @Input()
    public readonly: boolean = false;

    @Input()
    public disabled: boolean = false;

    @Input()
    public placeholder: string = null!;

    @Input()
    public maxLength: number = null!;

    @Input()
    public rows: number = 5;

    @Input()
    public min?: number | Date;

    @Input()
    public max?: number | Date;

    @Input()
    public label: string = null!;

    @Input()
    public form: NgForm = null!;

    public value: any;

    @ViewChild('flatpickr')
    public flatpickr?: ElementRef<HTMLElement>;

    constructor(private readonly _datePipe: DatePipe) {

    }

    public ngAfterViewInit(): void {
        if (this.flatpickr?.nativeElement) {
            this._flatpickerInstance = flatpickr(this.flatpickr.nativeElement, {
                enableTime: true,
                onChange: (selectedDates: any[]) => {
                    const dateTime = selectedDates.find(_ => true);

                    this.value = this._datePipe.transform(dateTime, 'yyyy-MM-ddTHH:mm');
                    this.onChanged();
                }
            });

            if (this.value) {
                this._flatpickerInstance.setDate(this.value, false);
            }
        }
    }

    public writeValue(obj: any): void {
        this.value = obj;

        if (this._flatpickerInstance) {
            this._flatpickerInstance.setDate(this.value, false);
        }
    }

    public registerOnChange(fn: any): void {
        this._onChanged = fn;
    }

    public registerOnTouched(fn: any): void {
        this._onTouched = fn;
    }

    public setDisabledState?(isDisabled: boolean): void {
        this.disabled = isDisabled;
    }

    public ngOnInit(): void {
        InputComponent._counter = InputComponent._counter + 1;

        if (!this.id) {
            this.id = `input-${InputComponent._counter}`;
        }

        if (!this.name) {
            this.name = `input${InputComponent._counter}`;
        }
    }

    public onChanged(): void {
        this._onChanged(this.value);
    }

    public onTouched(event: FocusEvent): void {
        this._onTouched(event);
    }

    public onInput(e: Event) {
        const v = (e.target as HTMLInputElement).value;
        this.value = v;
        this._onChanged(v);
    }
}