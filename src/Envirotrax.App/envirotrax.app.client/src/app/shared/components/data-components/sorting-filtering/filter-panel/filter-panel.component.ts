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

    public onFilterChanged(): void {
        const filterFields: FilterFieldVm[] = this.fields!.map(f => ({
            fieldName: f.fieldName,
            label: f.label,
            type: f.type,
            value: f.value
        }));

        const queryProperties = this._queryHelper.convertFilterPanelFields(filterFields);

        this.filterChange.emit(queryProperties);
    }
}