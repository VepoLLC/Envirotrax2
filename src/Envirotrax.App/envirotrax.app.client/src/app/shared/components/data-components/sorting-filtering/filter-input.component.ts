import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { ControlContainer, NgForm } from "@angular/forms";
import { ColumnType, QueryColumnOption } from "./query-view-model";
import { QueryProperty } from "../../../models/query";

@Component({
    selector: 'vp-filter-input',
    templateUrl: './filter-input.component.html',
    standalone: false,
    viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})
export class FilterInputComponent implements OnInit {
    @Input()
    public queryForm?: NgForm;

    @Input()
    public columns: FilterColumnOption[] = [];

    @Input()
    public index: number = 0;

    @Input()
    public model?: QueryProperty;

    @Output()
    public onDelete: EventEmitter<void> = new EventEmitter<void>();

    public selectedColumn?: FilterColumnOption;
    public columnType = ColumnType;
    public options = OptionsHelper;

    public ngOnInit(): void {
        if (!this.model!.logicalOperator) {
            this.model!.logicalOperator = 'And';
        }

        this.setSelectedColumn();
        this.setSelectedColumnSearchTypes();
    }

    private setSelectedColumnSearchTypes(): void {
        if (this.selectedColumn) {
            if (this.model!.isValueNull) {
                this.selectedColumn.searchTypes = this.options.otherOptions;
                return;
            }

            switch (this.selectedColumn.type) {
                case ColumnType.text:
                    this.selectedColumn.searchTypes = this.options.textOptions;
                    this.selectedColumn.inputType = 'text';
                    break;
                case ColumnType.number:
                    this.selectedColumn.searchTypes = this.options.dateAndNumberOptions;
                    this.selectedColumn.inputType = 'number';
                    break;
                case ColumnType.date:
                    this.selectedColumn.searchTypes = this.options.dateAndNumberOptions;
                    this.selectedColumn.inputType = 'date';
                    break;
                default:
                    this.selectedColumn.searchTypes = this.options.otherOptions;

                    if (this.selectedColumn.columnValues) {
                        this.selectedColumn.inputType = 'select';
                    } else {
                        this.selectedColumn.inputType = 'text';
                    }
            }
        }
    }

    private setSelectedColumn(): void {
        for (let item of this.columns) {
            if (item.id == this.model!.columnName) {
                this.selectedColumn = item;
                return;
            }
        }

        this.selectedColumn = undefined;
    }

    public columnSelected(): void {
        this.model!.comparisonOperator = undefined;
        this.model!.value = '';

        if (this.columns) {
            this.setSelectedColumn();
            this.setSelectedColumnSearchTypes();
        }
    }

    public valueNullChange(): void {
        this.model!.value = '';

        if (this.model!.comparisonOperator != 'Eq' && this.model!.comparisonOperator != 'NotEq') {
            this.model!.comparisonOperator = undefined;
        }

        this.setSelectedColumn();
        this.setSelectedColumnSearchTypes();
    }
}

export interface SelectOption {
    id?: string;
    value?: string;
    disabled?: boolean;
}

export interface FilterColumnOption extends SelectOption {
    type: ColumnType;
    searchTypes?: SelectOption[];
    columnValues?: QueryColumnOption[]
    inputType?: 'text' | 'number' | 'date' | 'select'
}

class OptionsHelper {
    public static textOptions: SelectOption[] = [
        {
            id: '',
            value: ''
        },
        {
            id: 'Eq',
            value: 'Equals'
        },
        {
            id: 'NotEq',
            value: 'Not Equal'
        },
        {
            id: 'StW',
            value: 'Starts with'
        },
        {
            id: 'NotStW',
            value: 'Not Start with'
        },
        {
            id: 'EndW',
            value: 'Ends with'
        },
        {
            id: 'NotEndW',
            value: 'Not End with'
        },
        {
            id: 'Ct',
            value: 'Contains'
        },
        {
            id: 'NotCt',
            value: 'Not Contain'
        }
    ];

    public static dateAndNumberOptions: SelectOption[] = [
        {
            id: '',
            value: ''
        },
        {
            id: 'Eq',
            value: 'Equals'
        },
        {
            id: 'NotEq',
            value: 'Not Equal'
        },
        {
            id: 'Gt',
            value: 'Greater than'
        },
        {
            id: 'Gte',
            value: 'Greater than or Equal'
        },
        {
            id: 'Lt',
            value: 'Less than'
        },
        {
            id: 'Lte',
            value: 'Less than or Equal'
        }
    ];

    public static otherOptions: SelectOption[] = [
        {
            id: '',
            value: ''
        },
        {
            id: 'Eq',
            value: 'Equals'
        },
        {
            id: 'NotEq',
            value: 'Not Equal'
        }
    ];

    public static logicalOperators: SelectOption[] = [
        {
            id: '',
            value: ''
        },
        {
            id: 'And',
            value: 'And'
        },
        {
            id: 'Or',
            value: 'Or'
        }
    ];
}

export class SelectInputOption implements SelectOption {
    private _disabledFunc?: () => boolean;

    public id: string;
    public value: string;

    public get disabled(): boolean {
        if (this._disabledFunc) {
            return this._disabledFunc();
        }

        return false;
    }

    public set disabled(value: boolean) {
        this._disabledFunc = () => value;
    }

    constructor(
        key: string,
        value: string,
        disabled?: () => boolean
    ) {
        this.id = key;
        this.value = value;
        this._disabledFunc = disabled;
    }
}
