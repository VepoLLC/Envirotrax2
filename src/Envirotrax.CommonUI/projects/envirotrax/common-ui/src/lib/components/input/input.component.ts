import { DatePipe } from "@angular/common";
import { AfterViewInit, Component, ElementRef, forwardRef, input, Input, NgZone, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { AbstractControl, ControlValueAccessor, NgForm, NG_VALIDATORS, NG_VALUE_ACCESSOR, ValidationErrors, Validator } from "@angular/forms";
import { NgSelectComponent } from "@ng-select/ng-select";
import flatpickr from "flatpickr";
import { Instance } from "flatpickr/dist/types/instance";
import confirmDatePlugin from "flatpickr/dist/plugins/confirmDate/confirmDate";

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
        {
            provide: NG_VALIDATORS,
            useExisting: forwardRef(() => InputComponent),
            multi: true
        },
        DatePipe
    ],
    styleUrl: './input.component.css'
})
export class InputComponent implements ControlValueAccessor, Validator, OnInit, AfterViewInit, OnDestroy {
    private _onChanged: (value: any) => void = null!;
    private _onTouched: (event: FocusEvent) => void = null!;

    private _flatpickerInstance?: Instance

    private readonly _onScroll = () => this.select?.dropdownPanel()?.adjustPosition();

    private static _counter: number = 0;

    private static readonly EmailRegex = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/;

    private static readonly EmailSeparator = '; ';

    private static readonly MultiEmailPlaceholder = 'Type an email address and press Enter';

    @Input()
    public id: string = null!;

    @Input()
    public name: string = null!;

    @Input()
    public type: 'text' | 'number' | 'date' | 'datetime' | 'daterange' | 'textarea' | 'select' | 'email' | 'multi-select' | 'multi-email' = 'text';

    @Input()
    public required: boolean = false;

    @Input()
    public readonly: boolean = false;

    @Input()
    public disabled: boolean = false;

    @Input()
    public placeholder: string = '';

    @Input()
    public maxLength: number = null!;

    @Input()
    public rows: number = 5;

    @Input()
    public min?: number | Date;

    @Input()
    public max?: number | Date;

    @Input()
    public step?: number | string;

    @Input()
    public decimals?: number;

    @Input()
    public numeric: boolean = false;

    @Input()
    public label: string = null!;

    @Input()
    public validationLabel?: string;

    @Input()
    public form?: NgForm;

    @Input()
    public options: InputOption[] = [];

    @Input()
    public borderColor?: string;

    @Input()
    public flexFill: boolean = false;

    @Input()
    public labelStyle: 'horizontal' | 'vertical' = 'vertical';

    @Input()
    public isInputGroup: boolean = false;

    public value: any | DateRange;

    public emailAddresses: string[] = [];

    public readonly addEmailAddressTag = (term: string): string => term;

    @ViewChild('flatpickr')
    public flatpickr?: ElementRef<HTMLElement>;

    @ViewChild(NgSelectComponent)
    public select?: NgSelectComponent;

    constructor(
        private readonly _datePipe: DatePipe,
        private readonly _zone: NgZone
    ) {

    }

    public ngOnDestroy(): void {
        this.onSelectClosed();
    }

    public onSelectOpened(): void {
        this._zone.runOutsideAngular(() => {
            window.addEventListener('scroll', this._onScroll, true);
        });
    }

    public onSelectClosed(): void {
        window.removeEventListener('scroll', this._onScroll, true);
    }

    public ngAfterViewInit(): void {
        if (this.flatpickr?.nativeElement) {
            this._flatpickerInstance = flatpickr(this.flatpickr.nativeElement, {
                enableTime: this.type == 'datetime',
                mode: this.type == 'daterange'
                    ? 'range'
                    : 'single',
                plugins: this.type == 'datetime'
                    ? [confirmDatePlugin({ confirmText: 'Select', confirmIcon: '' })]
                    : [],

                onChange: (selectedDates: any[]) => {
                    if (this.type == 'datetime') {
                        const dateTime = selectedDates.find(_ => true);
                        this.value = this._datePipe.transform(dateTime, 'yyyy-MM-ddTHH:mm');
                    } else if (this.type == 'daterange') {
                        const start = selectedDates[0];
                        const end = selectedDates[selectedDates.length - 1];

                        this.value = {
                            startDate: this._datePipe.transform(start, 'yyyy-MM-dd'),
                            endDate: this._datePipe.transform(end, 'yyyy-MM-dd')
                        };
                    }

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

        if (this.type === 'multi-email') {
            this.emailAddresses = this.splitEmailAddresses([obj ?? '']);
        }

        if (this._flatpickerInstance) {
            this._flatpickerInstance.setDate(this.value, false);
        }

        // Format decimals on initial load
        if (this.type === 'number' && this.decimals != null && this.value != null && this.value !== '') {
            const num = parseFloat(this.value);
            if (!isNaN(num)) {
                this.value = num.toFixed(this.decimals);
            }
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

    public validate(control: AbstractControl): ValidationErrors | null {
        if (this.type === 'email' && control.value) {
            if (!InputComponent.EmailRegex.test(control.value)) {
                return { email: true };
            }
        }

        if (this.type === 'multi-email' && control.value) {
            if (this.emailAddresses.some(email => !InputComponent.EmailRegex.test(email))) {
                return { email: true };
            }

            if (this.maxLength && control.value.length > this.maxLength) {
                return { maxlength: { requiredLength: this.maxLength, actualLength: control.value.length } };
            }
        }

        const isNumericField = this.type === 'number' || this.numeric;

        if (isNumericField && !this.isBlank(control.value)) {
            const parsed = Number(control.value);

            if (isNaN(parsed)) {
                return { number: true };
            }

            if (typeof this.min === 'number' && parsed < this.min) {
                return { min: { min: this.min, actual: parsed } };
            }

            if (typeof this.max === 'number' && parsed > this.max) {
                return { max: { max: this.max, actual: parsed } };
            }
        }

        return null;
    }

    private isBlank(value: any): boolean {
        return value == null || (typeof value === 'string' && value.trim() === '');
    }

    public ngOnInit(): void {
        InputComponent._counter = InputComponent._counter + 1;

        if (!this.id) {
            this.id = `input-${InputComponent._counter}`;
        }

        if (!this.name) {
            this.name = `input${InputComponent._counter}`;
        }

        if (this.type === 'multi-email' && !this.placeholder) {
            this.placeholder = InputComponent.MultiEmailPlaceholder;
        }
    }

    public onChanged(): void {
        this._onChanged(this.value);
    }

    public onTouched(event: FocusEvent): void {
        this._onTouched(event);
        this.formatDecimals();
    }

    private formatDecimals(): void {
        if (this.type === 'number' && this.decimals != null && this.value != null && this.value !== '') {
            const num = parseFloat(this.value);
            if (!isNaN(num)) {
                this.value = num.toFixed(this.decimals);
                this._onChanged(this.value);
            }
        }
    }

    public onInput(e: Event) {
        const v = (e.target as HTMLInputElement).value;
        this.value = v;
        this._onChanged(v);
    }

    public onEmailAddressesChanged(): void {
        this.emailAddresses = this.splitEmailAddresses(this.emailAddresses);
        this.value = this.emailAddresses.join(InputComponent.EmailSeparator);

        this._onChanged(this.value);
    }

    public onEmailAddressesBlur(event: FocusEvent): void {
        const pendingAddress = this.select?.searchTerm?.trim();

        if (pendingAddress) {
            this.select!.searchTerm = '';
            this.emailAddresses = [...this.emailAddresses, pendingAddress];
            this.onEmailAddressesChanged();
        }

        this.onTouched(event);
    }

    private splitEmailAddresses(values: string[]): string[] {
        const addresses = values
            .flatMap(value => value.split(/[;,\s]+/))
            .map(address => address.trim())
            .filter(address => address);

        return addresses.filter((address, index) =>
            addresses.findIndex(other => other.toLowerCase() === address.toLowerCase()) === index);
    }
}

export interface DateRange {
    startDate?: string;
    endDate?: string;
}

export interface InputOption<T = any> {
    id?: any;
    text?: string;
    data?: T;
}