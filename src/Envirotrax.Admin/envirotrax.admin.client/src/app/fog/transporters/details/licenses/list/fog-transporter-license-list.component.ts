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
import { ProfessionalLicense } from '../../../../../shared/models/professionals/licenses/professional-license';
import {
    expirationTypeCssClasses,
    unvalidatedExpirationCssClass
} from '../../../../../shared/models/professionals/licenses/professional-user-license';
import { FogTransporterLicenseService } from '../../../../../shared/services/fog/fog-transporter-license.service';
import {
    AddEditFogTransporterLicenseComponent,
    FogTransporterLicenseModalData
} from '../edit/add-edit-fog-transporter-license.component';

interface FogTransporterLicenseVm extends ProfessionalLicense {
    expirationCssClass: string;
}

@Component({
    selector: 'vp-fog-transporter-licenses',
    templateUrl: './fog-transporter-license-list.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
})
export class FogTransporterLicenseListComponent implements OnInit {
    @Input()
    public professionalId!: number;

    @ViewChild('expirationCell', { static: true })
    public expirationCell?: TemplateRef<CellTemplateData<FogTransporterLicenseVm>>;

    public table: TableViewModel<FogTransporterLicenseVm> = {
        query: {
            sort: { expirationDate: 'Desc' },
            filter: []
        }
    };

    constructor(
        private readonly _licenseService: FogTransporterLicenseService,
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
        this.showEditor('Add Registration', {});
    }

    public editLicense(license: ProfessionalLicense): void {
        this.showEditor('Edit Registration', license);
    }

    public deleteLicense(license: ProfessionalLicense): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;

                await this._licenseService.delete(this.professionalId, license.id!);

                this._toastService.successFullyDeleted('Registration');
            } catch (error) {
                this._toastService.show({ text: 'Failed to delete Registration.', type: ToastType.Error });

                return;
            } finally {
                this.table.isLoading = false;
            }

            await this.getLicenses();
        });
    }

    private showEditor(title: string, license: ProfessionalLicense): void {
        this._modalHelper.show<FogTransporterLicenseModalData, ProfessionalLicense>(AddEditFogTransporterLicenseComponent, {
            title,
            model: { professionalId: this.professionalId, license },
            size: ModalSize.medium,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.getLicenses());
    }

    private toViewModel(license: ProfessionalLicense): FogTransporterLicenseVm {
        return {
            ...license,
            expirationCssClass: license.expirationDate && license.expirationType != null
                ? expirationTypeCssClasses[license.expirationType]
                : unvalidatedExpirationCssClass
        };
    }

    private getColumns(): TableColumn<FogTransporterLicenseVm>[] {
        return [
            { field: 'licenseNumber', caption: 'Registration Number', type: ColumnType.text },
            { field: 'licenseType.name', caption: 'Type', type: ColumnType.text },
            { field: 'expirationDate', caption: 'Expiration Date', type: ColumnType.date, cellTemplate: this.expirationCell }
        ];
    }
}
