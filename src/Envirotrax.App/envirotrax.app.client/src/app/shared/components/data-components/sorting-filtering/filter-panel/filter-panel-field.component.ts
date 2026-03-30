import { Component, Input, OnInit, Optional, SkipSelf } from "@angular/core";
import { ControlContainer, NgForm } from "@angular/forms";
import { FilterPanelComponent } from "./filter-panel.component";
import { InputOption } from "../../../input/input.component";

@Component({
    selector: 'vp-filter-panel-field',
    templateUrl: './filter-panel-field.component.html',
    standalone: false,
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class FilterPanelFieldComponent implements OnInit {
    @Input()
    public fieldName: string = null!;

    @Input()
    public label: string = null!;

    @Input()
    public type: 'text' | 'number' | 'date' | 'daterange' | 'select' = 'text';

    @Input()
    public value?: string | DateRange;

    @Input()
    public form?: NgForm;

    @Input()
    public options: InputOption[] = [];

    @Input()
    public required: boolean = false;

    @Input()
    public borderColor?: string;

    constructor(@Optional() private readonly _parent: FilterPanelComponent) {

    }

    public ngOnInit(): void {
        if (this.type === 'select' && this.value === undefined) {
            this.value = '';
        }
    }

    public onChange() {
        if (this._parent) {
            this._parent.onFilterChanged();
        }
    }
}

export interface DateRange {
    startDate?: string;
    endDate?: string;
}