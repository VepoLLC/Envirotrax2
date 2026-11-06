import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
    CellTemplateData,
    ColumnType,
    DownloadService,
    HelperService,
    TableColumn,
    TableViewModel,
    ToastService
} from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import {
    ExpirationType,
    expirationTypeCssClasses,
    unvalidatedExpirationCssClass
} from '../../../shared/models/professionals/licenses/professional-user-license';
import { ProfessionalInsurance } from '../../../shared/models/professionals/professional-insurance';
import { WaterSupplierInsurance } from '../../../shared/models/professionals/water-supplier-insurance';
import { InsuranceService } from '../../../shared/services/licenses/insurance.service';
import { ProfessionalAccountWindowService } from '../../professional-account-window.service';

interface InsuranceVm extends WaterSupplierInsurance {
    expirationCssClass: string;
    canViewAccount: boolean;
    isSelected: boolean;
    isSaved: boolean;
}

@Component({
    selector: 'vp-insurance-list',
    templateUrl: './insurance-list.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class InsuranceListComponent implements OnInit {
    @ViewChild('accountCell', { static: true })
    public accountCell?: TemplateRef<CellTemplateData<InsuranceVm>>;

    @ViewChild('policyNumberCell', { static: true })
    public policyNumberCell?: TemplateRef<CellTemplateData<InsuranceVm>>;

    @ViewChild('coverageCell', { static: true })
    public coverageCell?: TemplateRef<CellTemplateData<InsuranceVm>>;

    @ViewChild('expirationCell', { static: true })
    public expirationCell?: TemplateRef<CellTemplateData<InsuranceVm>>;

    @ViewChild('actionsCell', { static: true })
    public actionsCell?: TemplateRef<CellTemplateData<InsuranceVm>>;

    public table: TableViewModel<InsuranceVm> = {
        query: {
            sort: {},
            filter: []
        }
    };

    constructor(
        private readonly _insuranceService: InsuranceService,
        private readonly _accountWindowService: ProfessionalAccountWindowService,
        private readonly _downloadService: DownloadService,
        private readonly _helper: HelperService,
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

    public selectInsurance(selected: InsuranceVm): void {
        if (!this.table.items || !this.table.items.data.includes(selected)) {
            return;
        }

        for (const insurance of this.table.items.data) {
            insurance.isSelected = insurance === selected;
        }
    }

    public async saveInsurance(insurance: InsuranceVm): Promise<void> {
        try {
            this.table.isLoading = true;

            const saved = await this._insuranceService.update(this.toProfessionalInsurance(insurance));

            insurance.expirationDate = saved.expirationDate;
            insurance.expirationCssClass = this.getExpirationCssClass(saved.expirationDate, saved.expirationType);
            insurance.isSaved = true;

            this._toastService.successfullySaved('Insurance');
        } finally {
            this.table.isLoading = false;
        }
    }

    public viewAccount(insurance: InsuranceVm): void {
        this._accountWindowService.open(insurance);
    }

    public async viewCertificate(insurance: InsuranceVm): Promise<void> {
        try {
            this.table.isLoading = true;

            const url = await this._insuranceService.getFileUrl(insurance.id!);

            this._downloadService.downloadFileFromUrl(url);
        } finally {
            this.table.isLoading = false;
        }
    }

    private toProfessionalInsurance(insurance: InsuranceVm): ProfessionalInsurance {
        return {
            id: insurance.id,
            professional: { id: insurance.professionalId },
            insuranceNumber: insurance.insuranceNumber,
            coverageAmount: this._helper.toNumberOrUndefined(insurance.coverageAmount),
            expirationDate: this._helper.toTextOrUndefined(insurance.expirationDate)
        };
    }

    private toViewModel(insurance: WaterSupplierInsurance): InsuranceVm {
        return {
            ...insurance,
            expirationCssClass: unvalidatedExpirationCssClass,
            canViewAccount: this._accountWindowService.canOpen(insurance.professionalType),
            isSelected: false,
            isSaved: false
        };
    }

    private getExpirationCssClass(expirationDate?: string, expirationType?: ExpirationType): string {
        if (!expirationDate || expirationType == null) {
            return unvalidatedExpirationCssClass;
        }

        return expirationTypeCssClasses[expirationType];
    }

    private getColumns(): TableColumn<InsuranceVm>[] {
        return [
            { field: 'userEmail', caption: 'User ID', type: ColumnType.text },
            { field: 'companyName', caption: 'Account', type: ColumnType.text, cellTemplate: this.accountCell },
            { field: 'insuranceNumber', caption: 'Policy Number', type: ColumnType.text, cellTemplate: this.policyNumberCell },
            { field: 'coverageAmount', caption: 'Coverage', type: ColumnType.number, cellTemplate: this.coverageCell },
            { field: 'expirationDate', caption: 'Expiration', type: ColumnType.date, cellTemplate: this.expirationCell },
            { field: 'id', caption: '', type: ColumnType.other, cellTemplate: this.actionsCell, queryColumnExcluded: true }
        ];
    }
}
