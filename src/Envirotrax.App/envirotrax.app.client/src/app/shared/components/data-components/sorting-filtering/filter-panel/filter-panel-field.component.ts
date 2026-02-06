import { Component, Input, Optional, Output } from "@angular/core";
import { NgForm } from "@angular/forms";
import { FilterPanelComponent } from "./filter-panel.component";

@Component({
    selector: 'vp-filter-panel-field',
    templateUrl: './filter-panel-field.component.html',
    standalone: false
})
export class FilterPanelFieldComponent {
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

    constructor(@Optional() private readonly _parent: FilterPanelComponent) {

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