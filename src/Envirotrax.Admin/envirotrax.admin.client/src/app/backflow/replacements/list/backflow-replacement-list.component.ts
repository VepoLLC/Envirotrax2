import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
    CellTemplateData,
    ColumnType,
    TableColumn,
    TableCustomAction,
    TableViewModel,
    ToastService,
    ToastType
} from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { BackflowReplacement } from '../../../shared/models/backflow/backflow-replacement';
import { PropertyType } from '../../../shared/models/sites/site';
import { BackflowReplacementService } from '../../../shared/services/backflow/backflow-replacement.service';
import { WindowService } from '../../../shared/services/window.service';
import { BackflowTestDetailsComponent } from '../../tests/details/backflow-test-details.component';
import { SiteEditComponent } from '../../../sites/edit/site-edit.component';

@Component({
    templateUrl: './backflow-replacement-list.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class BackflowReplacementListComponent implements OnInit {
    private static windowCount: number = 0;

    @ViewChild('holdCell', { static: true })
    public holdCell?: TemplateRef<CellTemplateData<BackflowReplacement>>;

    @ViewChild('propertyCell', { static: true })
    public propertyCell?: TemplateRef<CellTemplateData<BackflowReplacement>>;

    @ViewChild('assemblyCell', { static: true })
    public assemblyCell?: TemplateRef<CellTemplateData<BackflowReplacement>>;

    @ViewChild('replacedCell', { static: true })
    public replacedCell?: TemplateRef<CellTemplateData<BackflowReplacement>>;

    public readonly propertyType = PropertyType;

    public readonly idPrefix: string;

    public onHold: boolean = false;

    public headerText: string = 'Replacement Assembly Test Submissions';

    public table: TableViewModel<BackflowReplacement> = {
        query: {
            sort: {},
            filter: []
        }
    };

    public readonly customActions: TableCustomAction<BackflowReplacement>[] = [
        {
            text: 'Clear',
            iconClass: 'fa-solid fa-eraser',
            action: replacement => this.clear(replacement)
        }
    ];

    constructor(
        private readonly _replacementService: BackflowReplacementService,
        private readonly _windowService: WindowService,
        private readonly _toastService: ToastService
    ) {
        BackflowReplacementListComponent.windowCount = BackflowReplacementListComponent.windowCount + 1;

        this.idPrefix = `backflow-replacement-${BackflowReplacementListComponent.windowCount}`;
    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.getReplacements();
    }

    public async getReplacements(): Promise<void> {
        try {
            this.table.isLoading = true;

            this.table.items = await this._replacementService.getAll(
                this.table.items?.pageInfo || {},
                this.table.query,
                this.onHold);
        } finally {
            this.table.isLoading = false;
        }

        this.headerText = `Replacement Assembly Test Submissions (${this.table.items?.pageInfo?.totalItems ?? 0})`;
    }

    public async onHoldChange(): Promise<void> {
        if (this.table.items?.pageInfo) {
            this.table.items.pageInfo.pageNumber = 1;
        }

        await this.getReplacements();
    }

    public async toggleHold(replacement: BackflowReplacement, event: Event): Promise<void> {
        const checkbox = event.target as HTMLInputElement;
        const onHold = checkbox.checked;

        try {
            this.table.isLoading = true;

            await this._replacementService.updateHold(replacement.id!, replacement.waterSupplier!.id!, onHold);

            this._toastService.show({ text: 'Saved', type: ToastType.Success });
        } finally {
            this.table.isLoading = false;

            await this.getReplacements();
        }
    }

    public async clear(replacement: BackflowReplacement): Promise<void> {
        try {
            this.table.isLoading = true;

            await this._replacementService.updateCleared(replacement.id!, replacement.waterSupplier!.id!, true);

            this._toastService.show({ text: 'Cleared', type: ToastType.Success });
        } finally {
            this.table.isLoading = false;

            await this.getReplacements();
        }
    }

    public openSite(replacement: BackflowReplacement): void {
        const siteId = replacement.site?.id;

        if (siteId == null) {
            return;
        }

        this._windowService.addWindow(SiteEditComponent, {
            title: this.buildSiteWindowTitle(siteId, replacement),
            model: {
                siteId: siteId,
                waterSupplierId: replacement.waterSupplier?.id
            }
        });
    }

    public openTest(replacement: BackflowReplacement): void {
        this._windowService.addWindow(BackflowTestDetailsComponent, {
            title: this.buildTestWindowTitle(replacement),
            model: { id: replacement.id }
        });
    }

    public async openReplacedAssembly(replacement: BackflowReplacement): Promise<void> {
        let replaced: BackflowReplacement | null = null;

        try {
            this.table.isLoading = true;

            replaced = await this._replacementService.getReplacedAssembly(replacement.id!);
        } finally {
            this.table.isLoading = false;
        }

        if (replaced == null) {
            this._toastService.show({
                text: `No assemblies found for serial number "${replacement.replacementAssembly}".`,
                type: ToastType.Error
            });

            return;
        }

        this.openTest(replaced);
    }

    private getColumns(): TableColumn<BackflowReplacement>[] {
        return [
            {
                field: 'validationReplacementOnHold',
                caption: 'Hold',
                type: ColumnType.other,
                cellTemplate: this.holdCell,
                queryColumnExcluded: true
            },
            {
                field: 'propertyStreetName',
                caption: 'Property Information',
                type: ColumnType.other,
                cellTemplate: this.propertyCell,
                queryColumnExcluded: true
            },
            {
                field: 'serialNumber',
                caption: 'Assembly Information',
                type: ColumnType.other,
                cellTemplate: this.assemblyCell,
                queryColumnExcluded: true
            },
            {
                field: 'replacementAssembly',
                caption: 'Replaced Assembly',
                type: ColumnType.other,
                cellTemplate: this.replacedCell,
                queryColumnExcluded: true
            }
        ];
    }

    private buildSiteWindowTitle(siteId: number, replacement: BackflowReplacement): string {
        const address = this.buildAddress(replacement);

        return address ? `${siteId} - ${address}` : String(siteId);
    }

    private buildTestWindowTitle(replacement: BackflowReplacement): string {
        const address = this.buildAddress(replacement);

        return address ? `${replacement.id} - ${address}` : String(replacement.id);
    }

    private buildAddress(replacement: BackflowReplacement): string {
        let address = `${replacement.propertyStreetNumber ?? ''} ${replacement.propertyStreetName ?? ''}`.trim();

        if (replacement.propertyNumber) {
            address = `${address} #${replacement.propertyNumber}`.trim();
        }

        return address;
    }
}
