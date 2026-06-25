import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { FogDisposalSiteCandidate } from "../../../../../shared/models/fog/fog-disposal-site-candidate";
import { PhysicalType, PHYSICAL_TYPE_LABELS } from "../../../../../shared/models/fog/fog-disposal-site-enums";
import { ProfessionalFogDisposalSiteService } from "../../../../../shared/services/fog/professional-fog-disposal-site.service";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { ToastService } from "../../../../../shared/services/toast.service";
import { CellTemplateData, ColumnType, ModalHelperService, TableColumn } from "@envirotrax/common-ui";

@Component({
    standalone: false,
    templateUrl: './professional-fog-disposal-site-list.component.html'
})
export class ProfessionalFogDisposalSiteListComponent implements OnInit {
    public table: TableViewModel<FogDisposalSiteCandidate> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        }
    };

    @ViewChild('registrationCell', { static: true })
    private registrationCellTemplate!: TemplateRef<CellTemplateData<FogDisposalSiteCandidate>>;

    @ViewChild('wasteTypesCell', { static: true })
    private wasteTypesCellTemplate!: TemplateRef<CellTemplateData<FogDisposalSiteCandidate>>;

    constructor(
        private readonly _siteService: ProfessionalFogDisposalSiteService,
        private readonly _toastService: ToastService,
        private readonly _modalHelper: ModalHelperService
    ) {}

    public ngOnInit(): void {
        this.table.columns = this.getColumns();
        this.loadSites();
    }

    public getPhysicalTypeLabel(physicalType: PhysicalType | undefined): string {
        if (physicalType == null) {
            return '';
        }
        return PHYSICAL_TYPE_LABELS[physicalType] ?? '';
    }

    private getColumns(): TableColumn<FogDisposalSiteCandidate>[] {
        return [
            {
                field: 'name',
                caption: 'Disposal Facility',
                type: ColumnType.text
            },
            {
                field: 'registrationNumber',
                caption: 'Registration Number',
                type: ColumnType.text
            },
            {
                field: 'county',
                caption: 'County',
                type: ColumnType.text
            },
            {
                field: 'physicalType',
                caption: 'Waste Types',
                type: ColumnType.text,
                cellTemplate: this.wasteTypesCellTemplate
            },
            {
                field: '',
                caption: 'Registration',
                type: ColumnType.other,
                queryColumnExcluded: true,
                headerCssClass: 'text-center',
                cellTemplate: this.registrationCellTemplate
            }
        ];
    }

    public async loadSites(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._siteService.getAvailable(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public toggle(row: FogDisposalSiteCandidate): void {
        if (row.isActive) {
            this._modalHelper.showDeleteConfirmation()
                .result()
                .subscribe(() => this.setRegistration(row, false));
        } else {
            this.setRegistration(row, true);
        }
    }

    private async setRegistration(row: FogDisposalSiteCandidate, isActive: boolean): Promise<void> {
        try {
            this.table.isLoading = true;
            await this._siteService.setRegistration(row.id!, isActive);

            if (isActive) {
                this._toastService.successfullySaved('Disposal site');
            } else {
                this._toastService.successFullyDeleted('Disposal site');
            }

            await this.loadSites();
        } finally {
            this.table.isLoading = false;
        }
    }
}
