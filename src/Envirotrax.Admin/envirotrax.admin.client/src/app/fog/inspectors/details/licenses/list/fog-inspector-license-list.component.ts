import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import {
    CellTemplateData,
    ColumnType,
    ModalHelperService,
    TableColumn,
    TableViewModel,
    ToastService,
    ToastType
} from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import {
    expirationTypeCssClasses,
    ProfessionalUserLicense,
    unvalidatedExpirationCssClass
} from '../../../../../shared/models/professionals/licenses/professional-user-license';
import { FogInspectorLicenseService } from '../../../../../shared/services/fog/fog-inspector-license.service';
import {
    AddEditFogInspectorLicenseComponent,
    FogInspectorLicenseModalData
} from '../edit/add-edit-fog-inspector-license.component';

interface FogInspectorLicenseVm extends ProfessionalUserLicense {
    expirationCssClass: string;
}

@Component({
    selector: 'vp-fog-inspector-licenses',
    templateUrl: './fog-inspector-license-list.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
})
export class FogInspectorLicenseListComponent implements OnInit {
    @Input()
    public professionalId!: number;

    @ViewChild('expirationCell', { static: true })
    public expirationCell?: TemplateRef<CellTemplateData<FogInspectorLicenseVm>>;

    public table: TableViewModel<FogInspectorLicenseVm> = {
        query: {
            sort: { expirationDate: 'Desc' },
            filter: []
        }
    };

    constructor(
        private readonly _licenseService: FogInspectorLicenseService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.getLicenses();
    }

    public async getLicenses(): Promise<void> {
        try {
            this.table.isLoading = true;

            const licenses = await this._licenseService.getAll(
                this.professionalId,
                this.table.items?.pageInfo || {},
                this.table.query
            );

            this.table.items = {
                pageInfo: licenses.pageInfo,
                data: licenses.data.map(license => this.toViewModel(license))
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    public addLicense(): void {
        this.showEditor('Add License', {});
    }

    public editLicense(license: ProfessionalUserLicense): void {
        this.showEditor('Edit License', license);
    }

    public deleteLicense(license: ProfessionalUserLicense): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;

                await this._licenseService.delete(this.professionalId, license.id!);

                this._toastService.successFullyDeleted('License');
            } catch (error) {
                this._toastService.show({ text: 'Failed to delete License.', type: ToastType.Error });

                return;
            } finally {
                this.table.isLoading = false;
            }

            await this.getLicenses();
        });
    }

    private showEditor(title: string, license: ProfessionalUserLicense): void {
        this._modalHelper.show<FogInspectorLicenseModalData, ProfessionalUserLicense>(AddEditFogInspectorLicenseComponent, {
            title,
            model: { professionalId: this.professionalId, license },
            size: ModalSize.medium,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.getLicenses());
    }

    private toViewModel(license: ProfessionalUserLicense): FogInspectorLicenseVm {
        return {
            ...license,
            expirationCssClass: license.expirationDate && license.expirationType != null
                ? expirationTypeCssClasses[license.expirationType]
                : unvalidatedExpirationCssClass
        };
    }

    private getColumns(): TableColumn<FogInspectorLicenseVm>[] {
        return [
            { field: 'licenseNumber', caption: 'License Number', type: ColumnType.text },
            { field: 'licenseType.name', caption: 'Type', type: ColumnType.text },
            { field: 'user.emailAddress', caption: 'User', type: ColumnType.text },
            { field: 'expirationDate', caption: 'Expiration Date', type: ColumnType.date, cellTemplate: this.expirationCell }
        ];
    }
}
