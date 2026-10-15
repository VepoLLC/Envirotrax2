import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import {
    CellTemplateData,
    ColumnType,
    InputOption,
    ModalHelperService,
    QueryProperty,
    TableColumn,
    TableViewModel,
    ToastService
} from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { FogDisposalSite, FogDisposalSiteRow, physicalTypeOptions } from '../../../shared/models/fog/fog-disposal-site';
import { State } from '../../../shared/models/lookup/state';
import { FogDisposalSiteService } from '../../../shared/services/fog/fog-disposal-site.service';
import { LookupService } from '../../../shared/services/lookup/lookup.service';
import { AddEditFogDisposalSiteComponent, FogDisposalSiteModalData } from '../edit/add-edit-fog-disposal-site.component';

@Component({
    templateUrl: './fog-disposal-site-list.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class FogDisposalSiteListComponent implements OnInit {
    @ViewChild('cityStateCell', { static: true })
    public cityStateCell?: TemplateRef<CellTemplateData<FogDisposalSiteRow>>;

    public showResults: boolean = false;

    public table: TableViewModel<FogDisposalSiteRow> = {
        query: {
            sort: { name: 'Asc' },
            filter: []
        }
    };

    public readonly physicalTypeOptions: InputOption[] = [{ id: '', text: 'Any Value' }, ...physicalTypeOptions];

    private stateOptions: InputOption<State>[] = [];
    private stateCodes = new Map<number, string>();

    constructor(
        private readonly _disposalSiteService: FogDisposalSiteService,
        private readonly _lookupService: LookupService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.loadStates();
    }

    public onFilterChange(queryProperties: QueryProperty[]): void {
        this.table.query.filter = queryProperties;
    }

    public async search(searchForm: NgForm): Promise<void> {
        if (!searchForm.valid) {
            return;
        }

        // A new search starts again from the first page.
        if (this.table.items) {
            this.table.items.pageInfo = { ...this.table.items.pageInfo, pageNumber: 1 };
        }

        await this.getSites();

        this.showResults = true;
    }

    public async getSites(): Promise<void> {
        try {
            this.table.isLoading = true;

            const sites = await this._disposalSiteService.getAll(this.table.items?.pageInfo || {}, this.table.query);

            this.table.items = {
                pageInfo: sites.pageInfo,
                data: sites.data.map(site => this.toRow(site))
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    public addSite(): void {
        this.showEditor('Add Disposal Site', {});
    }

    public editSite(row: FogDisposalSiteRow): void {
        const { cityState, ...site } = row;

        this.showEditor('Edit Disposal Site', site);
    }

    public deleteSite(row: FogDisposalSiteRow): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;

                await this._disposalSiteService.delete(row.id!);

                this._toastService.successFullyDeleted('Disposal Site');
            } finally {
                this.table.isLoading = false;
            }

            await this.getSites();
        });
    }

    private showEditor(title: string, site: FogDisposalSite): void {
        this._modalHelper.show<FogDisposalSiteModalData, FogDisposalSite>(AddEditFogDisposalSiteComponent, {
            title,
            model: { site, stateOptions: this.stateOptions },
            size: ModalSize.large,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.getSites());
    }

    // The list carries only the state id, so the codes come from the cached states lookup - one request for
    // the whole window, which the editor's State dropdown reuses.
    private async loadStates(): Promise<void> {
        this.stateOptions = await this._lookupService.getStatesAsOptions();
        this.stateCodes = new Map(this.stateOptions.map(option => [option.data!.id!, option.data!.code ?? '']));
    }

    private toRow(site: FogDisposalSite): FogDisposalSiteRow {
        const stateCode = site.state?.id != null ? this.stateCodes.get(site.state.id) : undefined;

        return {
            ...site,
            cityState: [site.city, stateCode].filter(part => part).join(', ')
        };
    }

    private getColumns(): TableColumn<FogDisposalSiteRow>[] {
        return [
            { field: 'name', caption: 'Name', type: ColumnType.text },
            { field: 'city', caption: 'City, State', type: ColumnType.text, cellTemplate: this.cityStateCell },
            { field: 'registrationNumber', caption: 'Registration Number', type: ColumnType.text }
        ];
    }
}
