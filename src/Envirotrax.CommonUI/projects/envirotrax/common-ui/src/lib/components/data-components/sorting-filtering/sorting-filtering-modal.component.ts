import { Component, TemplateRef, ViewChild } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { FilterColumnOption, SelectInputOption, SelectOption } from "./filter-input.component";
import { ColumnType, QueryColumn, QueryPropertyLocation, QueryViewModel } from "./query-view-model";
import { HelperService } from "../../../services/helpers/helper.service";
import { QueryProperty } from "../../../models/query";

@Component({
    selector: 'vp-sorting-filtering-modal',
    templateUrl: './sorting-filtering-modal.component.html',
    standalone: false,
    styleUrls: [
        './sorting-filtering-modal.component.css'
    ]
})
export class SortingFilteringModalComponent {
    public model: QueryViewModel;
    public sortColumns: SortColumn[] = [];
    public sortColumnOptions: SelectOption[] = [];
    public filterColumnOptions: FilterColumnOption[] = [];
    public sortDirectionOptions: SelectOption[] = [];
    public isViewReady: boolean = false;
    public selectedTab: 'Sorting' | 'Filtering' = 'Sorting';
    public canReset: boolean = false;
    public filterLocation = QueryPropertyLocation;

    @ViewChild('queryForm', { read: NgForm })
    public queryForm?: NgForm;

    constructor(
        helperService: HelperService,
        private readonly _modalReference: ModalReference<QueryViewModel>
    ) {
        this.model = helperService.copy(_modalReference.config.model, (key: any, value: any) => {
            if (value instanceof TemplateRef) {
                return null;
            }

            return value;
        })!;
    }

    public ngOnInit(): void {
        this.setSortDirections();
        this.setSortColumnOptions();
        this.setFilterColumnOptions();
        this.setSortModelValues();
        this.setCanReset();
        this.isViewReady = true;
    }

    public selectTab(tabName: 'Sorting' | 'Filtering'): void {
        this.selectedTab = tabName;
    }

    private getAvailableSortColumns(): QueryColumn[] {
        let result: QueryColumn[] = [];

        if (this.model.columns.length !== this.sortColumns.length) {
            for (let item of this.model.columns) {
                let existing = this.sortColumns.filter(s => s.field === item.field);

                if (existing.length === 0) {
                    result.push(item);
                }
            }
        }

        return result;
    }

    public isSortSelected(fieldName: string): boolean {
        return this.sortColumns
            .filter(s => s.field == fieldName)
            .length > 0;
    }

    public addSortColumn(): void {
        let sortColumns = this.getAvailableSortColumns();

        if (sortColumns.length > 0) {
            this.sortColumns.push({});
        }
    }

    public removeSortColumn(column: SortColumn): void {
        let index = this.sortColumns.indexOf(column);
        this.sortColumns.splice(index, 1);
    }

    private setSortModel(): void {
        this.model.query.sort = {};
        for (let sort of this.sortColumns) {
            this.model!.query!.sort[sort.field!] = sort.value!;
        }
    }

    public addFilter(): void {
        this.model.query.filter = this.model.query.filter || [];

        this.model.query.filter!.push({
            children: []
        });
    }

    public removeFilter(filter: QueryProperty): void {
        let index = this.model.query.filter!.indexOf(filter);
        this.model.query.filter!.splice(index, 1);
    }

    public submitData(): void {
        if (this.queryForm!.valid) {
            this.setSortModel();
            this._modalReference.closeSuccess(this.model);
        }
    }

    public reset(): void {
        this.model.query.sort = {};
        this.model.query.filter = [];
        this.model.wasReset = true;

        this._modalReference.closeSuccess(this.model);
    }

    public cancel(): void {
        this._modalReference.cancel();
    }

    private setSortDirections(): void {
        this.sortDirectionOptions = [
            {
                id: '',
                value: ''
            },
            {
                id: 'Asc',
                value: 'Ascending'
            },
            {
                id: 'Desc',
                value: 'Descending'
            }
        ];
    }

    private setSortColumnOptions(): void {
        this.sortColumnOptions.push({
            id: '',
            value: ''
        });

        for (let column of this.model.columns) {
            this.sortColumnOptions.push(new SelectInputOption(
                column.field,
                column.caption,
                () => this.isSortSelected(column.field)
            ));
        }
    }

    private setFilterColumnOptions(): void {
        this.filterColumnOptions.push({
            id: '',
            value: '',
            type: ColumnType.other
        });

        for (let column of this.model.columns) {
            this.filterColumnOptions.push({
                id: column.field,
                value: column.caption,
                columnValues: column.options,
                type: column.type
            });
        }
    }

    private setSortModelValues(): void {
        if (this.model.query.sort) {
            for (let key in this.model.query.sort) {
                if (!this.isSortSelected(key)) {
                    this.sortColumns.push({
                        field: key,
                        value: this.model.query.sort[key]
                    });
                }
            }
        }
    }

    private setCanReset(): void {
        if (this.model.query.sort) {
            if (Object.keys(this.model.query.sort).length > 0) {
                this.canReset = true;
            }
        }

        if (this.model.query.filter) {
            if (this.model.query.filter.length > 0) {
                this.canReset = true;
            }
        }
    }
}

export interface SortColumn {
    field?: string;
    value?: 'Asc' | 'Desc';
}
