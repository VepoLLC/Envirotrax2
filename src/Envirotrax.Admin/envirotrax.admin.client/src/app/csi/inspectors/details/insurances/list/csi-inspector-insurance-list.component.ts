import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import {
    CellTemplateData,
    ColumnType,
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
import { CsiInspectorInsuranceService } from '../../../../../shared/services/csi/csi-inspector-insurance.service';
import {
    AddEditCsiInspectorInsuranceComponent,
    CsiInspectorInsuranceModalData
} from '../edit/add-edit-csi-inspector-insurance.component';

interface CsiInspectorInsuranceVm extends ProfessionalInsurance {
    expirationCssClass: string;
}

@Component({
    selector: 'vp-csi-inspector-insurances',
    templateUrl: './csi-inspector-insurance-list.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
})
export class CsiInspectorInsuranceListComponent implements OnInit {
    @Input()
    public professionalId!: number;

    @ViewChild('expirationCell', { static: true })
    public expirationCell?: TemplateRef<CellTemplateData<CsiInspectorInsuranceVm>>;

    public table: TableViewModel<CsiInspectorInsuranceVm> = {
        query: {
            sort: { insuranceNumber: 'Asc' },
            filter: []
        }
    };

    public readonly customActions: TableCustomAction<CsiInspectorInsuranceVm>[] = [
        {
            text: 'View',
            iconClass: 'fa-solid fa-eye',
            action: insurance => this.viewCertificate(insurance)
        }
    ];

    constructor(
        private readonly _insuranceService: CsiInspectorInsuranceService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
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
        // The tab has to be opened synchronously inside the click, otherwise the await moves the
        // window.open out of the user-gesture task and pop-up blockers suppress it.
        const certificateWindow = window.open('', '_blank');

        try {
            this.table.isLoading = true;

            const url = await this._insuranceService.getFileUrl(this.professionalId, insurance.id!);

            if (certificateWindow) {
                certificateWindow.location.href = url;
            }
        } catch (error) {
            certificateWindow?.close();

            this._toastService.show({ text: 'Could not open the certificate.', type: ToastType.Error });
        } finally {
            this.table.isLoading = false;
        }
    }

    private showEditor(title: string, insurance: ProfessionalInsurance): void {
        this._modalHelper.show<CsiInspectorInsuranceModalData, ProfessionalInsurance>(AddEditCsiInspectorInsuranceComponent, {
            title,
            model: { professionalId: this.professionalId, insurance },
            size: ModalSize.medium
        }).result().subscribe(() => this.getInsurances());
    }

    private toViewModel(insurance: ProfessionalInsurance): CsiInspectorInsuranceVm {
        return {
            ...insurance,
            expirationCssClass: insurance.expirationDate && insurance.expirationType != null
                ? expirationTypeCssClasses[insurance.expirationType]
                : unvalidatedExpirationCssClass
        };
    }

    private getColumns(): TableColumn<CsiInspectorInsuranceVm>[] {
        return [
            { field: 'insuranceNumber', caption: 'Policy Number', type: ColumnType.text },
            { field: 'expirationDate', caption: 'Expiration Date', type: ColumnType.date, cellTemplate: this.expirationCell }
        ];
    }
}
