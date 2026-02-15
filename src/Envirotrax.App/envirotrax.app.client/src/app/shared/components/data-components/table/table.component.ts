import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, TemplateRef, Type, ViewChild } from "@angular/core";
import { ModalService } from "@developer-partners/ngx-modal-dialog";
import { ColumnType, QueryColumn, QueryPropertyLocation, SpecialColumnType } from "../sorting-filtering/query-view-model";
import { PageInfo } from "../../../models/page-info";
import { ComparisonOperator, Query, QueryProperty } from "../../../models/query";

@Component({
    selector: 'vp-table',
    templateUrl: './table.component.html',
    standalone: false,
})
export class TableComponent implements OnChanges {
    private static _counter: number = 0;

    @Input()
    public columns: TableColumn<any>[] = null!;

    @Input()
    public data: any[] | undefined = null!;

    @Input()
    public canAdd: boolean = false;

    @Output()
    public add: EventEmitter<void> = new EventEmitter();

    @Input()
    public canEdit: boolean = false;

    @Output()
    public edit: EventEmitter<any> = new EventEmitter();

    @Input()
    public canDelete: boolean = false;

    @Output()
    public delete: EventEmitter<any> = new EventEmitter();

    @Output()
    public reactivate: EventEmitter<any> = new EventEmitter();

    @Input()
    public canViewDetails: boolean = false;

    @Output()
    public viewDetails: EventEmitter<any> = new EventEmitter();

    @Input()
    public canSelect: Boolean = false;

    @Output()
    public select: EventEmitter<any> = new EventEmitter();

    @Input()
    public customActions: TableCustomAction<any>[] = [];

    @Input()
    public pageInfo: PageInfo = null!;

    @Output()
    public pageInfoChange: EventEmitter<PageInfo> = new EventEmitter();

    @Input()
    public query: Query = null!;

    @Output()
    public queryChange: EventEmitter<Query> = new EventEmitter();

    @Output()
    public queryReset: EventEmitter<void> = new EventEmitter();

    @Input()
    public freeTextSearch?: FreeTextSearchSettings;

    @ViewChild('actionsTemplate', { static: true })
    public actionsTemplate: TemplateRef<CellTemplateData<any>> = null!;

    @Input()
    public canHaveSoftDelete: boolean = false;

    @Input()
    public canFilterAdvanced: boolean = false;

    @Input()
    public layoutName?: string;

    public queryColumns?: TableColumn<any>[];
    public searchId: string;
    public resetSearchId: string;
    public showSoftDeleted: boolean = false;

    constructor(
        private readonly _modalService: ModalService
    ) {
        TableComponent._counter = TableComponent._counter + 1;
        this.searchId = `search-${TableComponent._counter}`;
        this.resetSearchId = `reset-search-${TableComponent._counter}`;
    }

    private canHaveAction(): boolean {
        return this.canEdit || this.canDelete || this.customActions?.length > 0
    }

    private setActionsColumn(): void {
        const actionsColumn = this.columns.find(c => c.tag?.specialColumn == SpecialColumnType.ActionsColumn);

        if (this.canHaveAction() && !actionsColumn) {
            this.columns.splice(0, 0, {
                field: '',
                caption: 'Actions',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.actionsTemplate,
                isDownloadExcluded: true,
                tag: {
                    specialColumn: SpecialColumnType.ActionsColumn
                }
            });
        } else {
            if (!this.canHaveAction()) {
                this.columns = this.columns.filter(c => c != actionsColumn);
            }
        }
    }

    public onQueryChange(query: Query): void {
        if (this.pageInfo) {
            this.pageInfo.pageNumber = 1;
        }

        this.queryChange.emit(query);
    }

    public onQueryReset(): void {
        if (this.freeTextSearch) {
            this.freeTextSearch.text = '';
        }

        this.queryReset.emit();
    }

    public sortColumn(column: TableColumn<any>): void {
        if (this.query && column.field) {
            if (!column.queryColumnExcluded) {
                if (this.query?.sort) {
                    for (const key in this.query.sort) {
                        if (key !== column.field && this.query.sort.hasOwnProperty(key)) {
                            delete this.query.sort[key];
                        }
                    }
                }

                this.query.sort = this.query.sort || {};

                switch (this.query.sort[column.field]) {
                    case 'Asc':
                        this.query.sort[column.field] = 'Desc';
                        break
                    case 'Desc':
                        delete this.query.sort[column.field];
                        break
                    default:
                        this.query.sort[column.field] = 'Asc';
                }

                this.query = { ...this.query };
            }

            this.onQueryChange(this.query);
        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if (changes['columns']?.currentValue) {
            this.setActionsColumn();

            if (this.columns.length > 0) {
                this.queryColumns = this.columns.filter(c => !c.queryColumnExcluded);
            }
        }

        if ((changes['columns']?.currentValue || changes['freeTextSearch']?.currentValue) &&
            (this.columns && this.freeTextSearch)) {
            if (!this.freeTextSearch.placeholder) {
                const columnCaptions = this.freeTextSearch
                    .searchQuery
                    .map(property => property.placeholder || this.columns?.find(c => c.field == property.field)?.caption)
                    .filter(property => !!property);

                this.freeTextSearch.placeholder = 'Enter text to search by ' + columnCaptions.join(', ');
            }
        }
    }

    public onAdd(): void {
        this.add.emit();
    }

    public onEdit(rowData: any): void {
        this.edit.emit(rowData);
    }

    public onDelete(rowData: any): void {
        this.delete.emit(rowData);
    }

    public onReactivate(rowData: any): void {
        this.reactivate.emit(rowData);
    }

    public onViewDetails(rowData: any): void {
        this.viewDetails.emit(rowData);
    }

    public onSelect(rowData: any): void {
        if (this.canSelect) {
            this.select.emit(rowData);
        } else if (this.canEdit) {
            this.onEdit(rowData);
        }
    }

    public customActionClicked(customAction: TableCustomAction<any>, record: any): void {
        if (customAction.action) {
            customAction.action(record);
        }
    }

    private createFreeTextProperty(property: FreeTextQuery, value: string): QueryProperty {
        return {
            columnName: property.field,
            logicalOperator: property.logicalOperator || 'Or',
            value: property.multiWordSearch ? value.split(' ')[0] : value,
            comparisonOperator: property.operator || 'Ct',
            tag: {
                location: QueryPropertyLocation.SearchBar
            }
        };
    }

    public performSearch(): void {
        if (this.freeTextSearch) {
            this.query.filter = this.query.filter!.filter(p => p.tag?.location != QueryPropertyLocation.SearchBar);

            if (this.freeTextSearch.text) {
                const freeTextQueryProperty: QueryProperty = {
                    columnName: this.freeTextSearch.searchQuery.find(Boolean)?.field,
                    children: [],
                    tag: {
                        location: QueryPropertyLocation.SearchBar
                    }
                };

                for (let property of this.freeTextSearch.searchQuery) {
                    let queryItem = this.createFreeTextProperty(property, this.freeTextSearch.text);
                    freeTextQueryProperty.children!.push(queryItem);

                    if (property.multiWordSearch) {
                        let multipleWords = this.freeTextSearch
                            .text
                            .split(' ')
                            .filter(t => !!t);

                        for (let word of multipleWords) {
                            if (queryItem.value != word) {
                                freeTextQueryProperty.children!.push(this.createFreeTextProperty(property, word));
                            }
                        }
                    }
                }

                this.query.filter.push(freeTextQueryProperty);
            }

            this.onQueryChange(this.query);
        }
    }

    public resetFreeTextSearch(): void {
        this.freeTextSearch!.text = '';
        this.performSearch();
    }

    public showSoftDelete(): void {
        if (!this.showSoftDeleted) {
            this.query.filter?.push({
                columnName: 'deletedTime',
                value: '',
                isValueNull: true
            });
        }
        else {
            this.query.filter?.splice(this.query.filter?.findIndex(e => e.columnName === 'deletedTime'), 1);
        }

        this.onQueryChange(this.query);

        this.showSoftDeleted = !this.showSoftDeleted;
    }
}



export interface TableColumn<T> extends QueryColumn {
    /**
     * Tells whether this column should be excluded from the table columns.
     * Use this if you want to have only queryable property for this field, but not a column.
     */
    isTableColumnExcluded?: boolean;

    /**
     * Tells whether this should be excluded from the queryable fields.
     * Use this if you want to exclude this field from table columns, but us it in sorting and filtering.
     */
    queryColumnExcluded?: boolean;

    /**
     * Tells whether the table column is invisible, but can be added back to table column from the column manager menu.
     */
    isInvisible?: boolean;

    /**
     * Custom cell template HTML to display.
     */
    cellTemplate?: TemplateRef<CellTemplateData<T>>;

    /**
     * Custom Angular component to use.
     */
    cellComponent?: Type<any>

    /**
    * Custom table column size match with Bootstrap's col classe
    */
    size?: ColumnSize;

    /**
     * Tells whether the column should be exluded from the downloaded CSV file.
     */
    isDownloadExcluded?: boolean;
}

//export { ColumnType, QueryColumnOption } from '../sorting-filtering/query-view-model'

export interface CellTemplateData<T> {
    rowData?: T;
    column?: TableColumn<T>;
}

export interface TableCustomAction<T> {
    text: string;
    iconClass?: string;
    action: (record: T) => Promise<void> | void;
    hideAction?: (row: T) => boolean;
}

export enum ColumnSize {
    OneUnit = 'col-md-1',
    TwoUnits = 'col-md-2',
    ThreeUnits = 'col-md-3',
    FourUnit = 'col-md-4',
    FiveUnits = 'col-md-5',
    SixUnits = 'col-md-6',
    SevenUnit = 'col-md-7',
    EightUnits = 'col-md-8',
    NineUnits = 'col-md-9',
    TenUnit = 'col-md-10',
    ElevenUnits = 'col-md-11',
    TwelveUnits = 'col-md-12'
}

export interface FreeTextSearchSettings {
    placeholder?: string;
    text?: string;
    searchQuery: FreeTextQuery[];
}

export interface FreeTextQuery {
    field?: string;
    operator?: ComparisonOperator;
    logicalOperator?: 'And' | 'Or';
    multiWordSearch?: boolean
    placeholder?: string;
}