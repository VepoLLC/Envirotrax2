import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { CellTemplateData, ColumnType, TableColumn } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { RecordLog, recordLogTypeLabels } from '../../../../shared/models/logs/record-log';
import { CsiInspectionService } from '../../../../shared/services/csi/csi-inspection.service';

interface RecordLogRow extends RecordLog {
    logTypeLabel: string;
}

@Component({
    selector: 'vp-csi-inspection-record-log',
    templateUrl: './csi-inspection-record-log.component.html',
    imports: [CommonModule, SharedComponentsModule]
})
export class CsiInspectionRecordLogComponent implements OnInit {
    @ViewChild('descriptionCell', { static: true })
    public descriptionCell?: TemplateRef<CellTemplateData<RecordLogRow>>;

    @Input() public inspectionId: number = 0;

    public isLoading: boolean = false;

    public logs: RecordLogRow[] = [];

    public columns: TableColumn<RecordLogRow>[] = [];

    constructor(private readonly _inspectionService: CsiInspectionService) {

    }

    public async ngOnInit(): Promise<void> {
        this.columns = this.getColumns();

        await this.reload();
    }

    public async reload(): Promise<void> {
        try {
            this.isLoading = true;

            const logs = await this._inspectionService.getLogs(this.inspectionId);

            this.logs = logs.map(log => ({
                ...log,
                logTypeLabel: log.logType == null ? '' : recordLogTypeLabels[log.logType]
            }));
        } finally {
            this.isLoading = false;
        }
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
