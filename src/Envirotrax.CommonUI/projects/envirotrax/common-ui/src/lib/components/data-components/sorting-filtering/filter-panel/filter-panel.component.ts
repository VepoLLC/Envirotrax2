import { Component, ContentChildren, EventEmitter, Output, QueryList, ViewChildren } from "@angular/core";
import { FilterPanelFieldComponent } from "./filter-panel-field.component";
import { QueryProperty } from "../../../../models/query";
import { FilterFieldVm, QueryHelperService } from "../../../../services/helpers/query-helper.service";

@Component({
    standalone: false,
    selector: 'vp-filter-panel',
    templateUrl: './filter-panel.component.html'
})
export class FilterPanelComponent {
    @ContentChildren(FilterPanelFieldComponent, { descendants: true })
    public fields?: QueryList<FilterPanelFieldComponent>;

    @Output()
    public filterChange: EventEmitter<QueryProperty[]> = new EventEmitter();

    constructor(private readonly _queryHelper: QueryHelperService) {

    }

    public getFilter(): QueryProperty[] {
        const filterFields: FilterFieldVm[] = this.fields!.map(f => ({
            fieldName: f.fieldName,
            label: f.label,
            type: f.type,
            value: f.value
        }));

        return this._queryHelper.convertFilterPanelFields(filterFields);
    }

    /**
     * Puts a filter this panel built earlier, such as one saved in a search template, back into its fields
     * and emits it. Fields the filter has no value for are cleared.
     */
    public setFilter(queryProperties: QueryProperty[]): void {
        for (const field of this.fields ?? []) {
            field.value = this._queryHelper.getFilterPanelFieldValue(field, queryProperties);
        }

        this.onFilterChanged();
    }

    public onFilterChanged(): void {
        this.filterChange.emit(this.getFilter());
    }
}