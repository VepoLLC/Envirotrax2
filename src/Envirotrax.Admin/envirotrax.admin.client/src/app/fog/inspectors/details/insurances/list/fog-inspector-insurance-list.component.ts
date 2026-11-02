import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import {
    CellTemplateData,
    ColumnType,
    DownloadService,
    ModalHelperService,
    TableColumn,
    TableCustomAction,
    TableViewModel,
    ToastService,
    ToastType
} from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import {
    expirationTypeCssClasses,
    unvalidatedExpirationCssClass
} from '../../../../../shared/models/professionals/licenses/professional-user-license';
import { ProfessionalInsurance } from '../../../../../shared/models/professionals/professional-insurance';
import { FogInspectorInsuranceService } from '../../../../../shared/services/fog/fog-inspector-insurance.service';
import {
    AddEditFogInspectorInsuranceComponent,
    FogInspectorInsuranceModalData
} from '../edit/add-edit-fog-inspector-insurance.component';

interface FogInspectorInsuranceVm extends ProfessionalInsurance {
    expirationCssClass: string;
}

@Component({
    selector: 'vp-fog-inspector-insurances',
    templateUrl: './fog-inspector-insurance-list.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
})
export class FogInspectorInsuranceListComponent implements OnInit {
    @Input()
    public professionalId!: number;

    @ViewChild('expirationCell', { static: true })
    public expirationCell?: TemplateRef<CellTemplateData<FogInspectorInsuranceVm>>;

    public table: TableViewModel<FogInspectorInsuranceVm> = {
        query: {
            sort: { insuranceNumber: 'Asc' },
            filter: []
        }
    };

    public readonly customActions: TableCustomAction<FogInspectorInsuranceVm>[] = [
        {
            text: 'View',
            iconClass: 'fa-solid fa-eye',
            action: insurance => this.viewCertificate(insurance)
        }
    ];

    constructor(
        private readonly _insuranceService: FogInspectorInsuranceService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _downloadService: DownloadService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.getInsurances();
    }

    public async getInsurances(): Promise<void> {
        try {
            this.table.isLoading = true;

            const insurances = await this._insuranceService.getAll(
                this.professionalId,
                this.table.items?.pageInfo || {},
                this.table.query
            );

            this.table.items = {
                pageInfo: insurances.pageInfo,
                data: insurances.data.map(insurance => this.toViewModel(insurance))
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    public addInsurance(): void {
        this.showEditor('Add Insurance Policy', {});
    }

    public editInsurance(insurance: ProfessionalInsurance): void {
        this.showEditor('Edit Insurance Policy', insurance);
    }

    public deleteInsurance(insurance: ProfessionalInsurance): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;

                await this._insuranceService.delete(this.professionalId, insurance.id!);

                this._toastService.successFullyDeleted('Insurance');
            } catch (error) {
                this._toastService.show({ text: 'Failed to delete Insurance.', type: ToastType.Error });

                return;
            } finally {
                this.table.isLoading = false;
            }

            await this.getInsurances();
        });
    }

    public async viewCertificate(insurance: ProfessionalInsurance): Promise<void> {
        try {
            this.table.isLoading = true;

            const url = await this._insuranceService.getFileUrl(this.professionalId, insurance.id!);
            this._downloadService.downloadFileFromUrl(url);
        } finally {
            this.table.isLoading = false;
        }
    }

    private showEditor(title: string, insurance: ProfessionalInsurance): void {
        this._modalHelper.show<FogInspectorInsuranceModalData, ProfessionalInsurance>(AddEditFogInspectorInsuranceComponent, {
            title,
            model: { professionalId: this.professionalId, insurance },
            size: ModalSize.medium,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.getInsurances());
    }

    private toViewModel(insurance: ProfessionalInsurance): FogInspectorInsuranceVm {
        return {
            ...insurance,
            expirationCssClass: insurance.expirationDate && insurance.expirationType != null
                ? expirationTypeCssClasses[insurance.expirationType]
                : unvalidatedExpirationCssClass
        };
    }

    private getColumns(): TableColumn<FogInspectorInsuranceVm>[] {
        return [
            { field: 'insuranceNumber', caption: 'Policy Number', type: ColumnType.text },
            { field: 'expirationDate', caption: 'Expiration Date', type: ColumnType.date, cellTemplate: this.expirationCell }
        ];
    }
}
