import { Component, Injectable, Injector, Input, OnInit, Type } from "@angular/core";
import { TableColumn } from "./../table.component";

@Component({
    selector: 'vp-table-cell',
    templateUrl: './table-cell.component.html',
    standalone: false,
})
export class TableCellComponent implements OnInit {
    @Input()
    public column: TableColumn<any> = null!;

    @Input()
    public rowData: any;

    @Input()
    public componentType: Type<any> = null!;

    public cellData: any;
    public cellInjector?: Injector;

    constructor(private readonly _injector: Injector) {

    }

    protected getCellData(fieldName: string): any {
        let obj = this.rowData;
        let parts = fieldName.split('.');
        let result;

        for (let part of parts) {
            if (obj) {
                result = obj[part];
                obj = result;
            }
        }

        return result;
    }

    public ngOnInit(): void {
        this.cellData = this.getCellData(this.column.field);

        if (this.componentType) {
            this.cellInjector = this.createRowInjector<any>(this.rowData, this.column);
        }
    }

    public createRowInjector<T>(row: T, column: TableColumn<T>): Injector {
        const data = new TableRowData<T>();

        data.rowData = row;
        data.column = column;

        return Injector.create({
            providers: [
                { provide: TableRowData<T>, useValue: data },
            ],
            parent: this._injector
        });
    }
}

@Injectable()
export class TableRowData<T> {
    public rowData: T = null!;
    public column: TableColumn<T> = null!;
}