import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { FogDisposalSite } from "../../../../../shared/models/fog/fog-disposal-site";
import { PhysicalType, PHYSICAL_TYPE_LABELS } from "../../../../../shared/models/fog/fog-disposal-site-enums";
import { ProfessionalFogDisposalSiteService } from "../../../../../shared/services/fog/professional-fog-disposal-site.service";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { MAX_PAGE_SIZE } from "../../../../../shared/models/page-info";
import { ToastService } from "../../../../../shared/services/toast.service";
import { CellTemplateData, ColumnType, ModalHelperService, TableColumn } from "@envirotrax/common-ui";

@Component({
    standalone: false,
    templateUrl: './professional-fog-disposal-site-list.component.html'
})
export class ProfessionalFogDisposalSiteListComponent implements OnInit {
    public table: TableViewModel<FogDisposalSiteVm> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        }
    };

    @ViewChild('registrationCell', { static: true })
    private registrationCellTemplate!: TemplateRef<CellTemplateData<FogDisposalSiteVm>>;

    @ViewChild('wasteTypesCell', { static: true })
    private wasteTypesCellTemplate!: TemplateRef<CellTemplateData<FogDisposalSiteVm>>;

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

    private getColumns(): TableColumn<FogDisposalSiteVm>[] {
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
                field: 'Registration',
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

            const [allSites, registeredSites] = await Promise.all([
                this._siteService.getAll(this.table.items?.pageInfo || {}, this.table.query),
                this._siteService.getRegistered({ pageSize: MAX_PAGE_SIZE }, {})
            ]);

            this.table.items = {
                pageInfo: allSites.pageInfo,
                data: allSites.data.map(site => ({
                    ...site,
                    selected: registeredSites.data.find(registered => registered.id === site.id)
                }))
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    public toggle(site: FogDisposalSiteVm): void {
        if (site.selected) {
            this._modalHelper.showDeleteConfirmation()
                .result()
                .subscribe(() => this.setRegistration(site, false));
        } else {
            this.setRegistration(site, true);
        }
    }

    private async setRegistration(site: FogDisposalSiteVm, isActive: boolean): Promise<void> {
        try {
            this.table.isLoading = true;
            await this._siteService.setRegistration(site.id!, isActive);

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

interface FogDisposalSiteVm extends FogDisposalSite {
    selected?: FogDisposalSite;
}
