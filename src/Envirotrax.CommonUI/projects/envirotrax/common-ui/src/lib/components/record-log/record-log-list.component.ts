import { Component, Input, OnChanges, OnInit, SimpleChanges, TemplateRef, ViewChild } from '@angular/core';
import { CellTemplateData, TableColumn } from '../data-components/table/table-models';
import { ColumnType } from '../data-components/sorting-filtering/query-view-model';
import { RecordLog, recordLogTypeLabels } from '../../models/record-log';

interface RecordLogRow extends RecordLog {
    logTypeLabel: string;
}

@Component({
    selector: 'vp-record-log-list',
    templateUrl: './record-log-list.component.html',
    standalone: false,
})
export class RecordLogListComponent implements OnInit, OnChanges {
    @ViewChild('descriptionCell', { static: true })
    public descriptionCell?: TemplateRef<CellTemplateData<RecordLogRow>>;

    @Input() public logs: RecordLog[] = [];
    @Input() public isLoading: boolean = false;
    @Input() public recordLabel: string = 'record';

    public rows: RecordLogRow[] = [];
    public columns: TableColumn<RecordLogRow>[] = [];

    public ngOnInit(): void {
        this.columns = this.getColumns();
        this.rows = this.mapRows(this.logs);
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if (changes['logs']) {
            this.rows = this.mapRows(this.logs);
        }
    }

    private mapRows(logs: RecordLog[]): RecordLogRow[] {
        return logs.map(log => ({
            ...log,
            logTypeLabel: log.logType == null ? '' : recordLogTypeLabels[log.logType]
        }));
    }

    private getColumns(): TableColumn<RecordLogRow>[] {
        return [
            { field: 'logDate', caption: 'Log Date', type: ColumnType.date },
            { field: 'user.email', caption: 'User ID', type: ColumnType.text },
            { field: 'logTypeLabel', caption: 'Type', type: ColumnType.text },
            { field: 'description', caption: 'Description', type: ColumnType.other, cellTemplate: this.descriptionCell, queryColumnExcluded: true }
        ];
    }
}
