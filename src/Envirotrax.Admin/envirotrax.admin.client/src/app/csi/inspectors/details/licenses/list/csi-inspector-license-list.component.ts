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
import { CsiInspectorLicenseService } from '../../../../../shared/services/csi/csi-inspector-license.service';
import {
    AddEditCsiInspectorLicenseComponent,
    CsiInspectorLicenseModalData
} from '../edit/add-edit-csi-inspector-license.component';

interface CsiInspectorLicenseVm extends ProfessionalUserLicense {
    expirationCssClass: string;
}

@Component({
    selector: 'vp-csi-inspector-licenses',
    templateUrl: './csi-inspector-license-list.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
})
export class CsiInspectorLicenseListComponent implements OnInit {
    @Input()
    public professionalId!: number;

    @ViewChild('expirationCell', { static: true })
    public expirationCell?: TemplateRef<CellTemplateData<CsiInspectorLicenseVm>>;

    public table: TableViewModel<CsiInspectorLicenseVm> = {
        query: {
            sort: { expirationDate: 'Desc' },
            filter: []
        }
    };

    constructor(
        private readonly _licenseService: CsiInspectorLicenseService,
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
        this._modalHelper.show<CsiInspectorLicenseModalData, ProfessionalUserLicense>(AddEditCsiInspectorLicenseComponent, {
            title,
            model: { professionalId: this.professionalId, license },
            size: ModalSize.medium
        }).result().subscribe(() => this.getLicenses());
    }

    // Only the badge class is computed here; the display columns bind their navigation paths directly
    // so the grid keeps server-side sorting.
    private toViewModel(license: ProfessionalUserLicense): CsiInspectorLicenseVm {
        return {
            ...license,
            expirationCssClass: license.expirationDate && license.expirationType != null
                ? expirationTypeCssClasses[license.expirationType]
                : unvalidatedExpirationCssClass
        };
    }

    private getColumns(): TableColumn<CsiInspectorLicenseVm>[] {
        return [
            { field: 'licenseNumber', caption: 'License Number', type: ColumnType.text },
            { field: 'licenseType.name', caption: 'Type', type: ColumnType.text },
            { field: 'user.emailAddress', caption: 'User', type: ColumnType.text },
            { field: 'expirationDate', caption: 'Expiration Date', type: ColumnType.date, cellTemplate: this.expirationCell }
        ];
    }
}
