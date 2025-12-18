import { Component } from "@angular/core";
import { TableColumn } from "../table.component";
import { TableRowData } from "./table-cell.component";

@Component({
    templateUrl: './checkbox-cell.component.html',
    standalone: false,
})
export class CheckboxCellComponent<T extends Record<string, any>> {
    public rowData: T;
    public column: TableColumn<T>;

    constructor(
        tableRowData: TableRowData<T>
    ) {
        this.rowData = tableRowData.rowData;
        this.column = tableRowData.column;
    }
}