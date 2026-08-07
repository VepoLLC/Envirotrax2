import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { CellTemplateData, ColumnType, TableColumn } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { CsiInspectionAssembly } from '../../../../shared/models/csi/csi-inspection';
import { CsiInspectionService } from '../../../../shared/services/csi/csi-inspection.service';

@Component({
    selector: 'vp-csi-inspection-assemblies',
    templateUrl: './csi-inspection-assemblies.component.html',
    imports: [CommonModule, SharedComponentsModule]
})
export class CsiInspectionAssembliesComponent implements OnInit {
    @ViewChild('statusCell', { static: true })
    public statusCell?: TemplateRef<CellTemplateData<CsiInspectionAssembly>>;

    @ViewChild('datesCell', { static: true })
    public datesCell?: TemplateRef<CellTemplateData<CsiInspectionAssembly>>;

    @ViewChild('infoCell', { static: true })
    public infoCell?: TemplateRef<CellTemplateData<CsiInspectionAssembly>>;

    @ViewChild('identifiedCell', { static: true })
    public identifiedCell?: TemplateRef<CellTemplateData<CsiInspectionAssembly>>;

    @Input() public inspectionId: number = 0;

    public isLoading: boolean = false;

    public assemblies: CsiInspectionAssembly[] = [];

    public columns: TableColumn<CsiInspectionAssembly>[] = [];

    constructor(private readonly _inspectionService: CsiInspectionService) {

    }

    public async ngOnInit(): Promise<void> {
        this.columns = this.getColumns();

        await this.load();
    }

    private async load(): Promise<void> {
        try {
            this.isLoading = true;
            this.assemblies = await this._inspectionService.getAssemblies(this.inspectionId);
        } finally {
            this.isLoading = false;
        }
    }

    private getColumns(): TableColumn<CsiInspectionAssembly>[] {
        return [
            { field: 'id', caption: 'ID', type: ColumnType.number },
            { field: 'isCurrent', caption: 'Status', type: ColumnType.other, cellTemplate: this.statusCell, queryColumnExcluded: true },
            { field: 'testDate', caption: 'Dates', type: ColumnType.other, cellTemplate: this.datesCell, queryColumnExcluded: true },
            { field: 'serialNumber', caption: 'Serial #', type: ColumnType.text },
            { field: 'assemblyDescription', caption: 'Assembly Information', type: ColumnType.other, cellTemplate: this.infoCell, queryColumnExcluded: true },
            { field: 'visuallyIdentified', caption: '', type: ColumnType.other, cellTemplate: this.identifiedCell, queryColumnExcluded: true }
        ];
    }
}
