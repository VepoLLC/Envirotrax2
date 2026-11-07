import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import {
    CellTemplateData,
    ColumnType,
    HelperService,
    InputOption,
    ModalHelperService,
    TableColumn,
    TableViewModel,
    ToastService,
    ToastType
} from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../shared/components/shared.components.module';
import {
    ExpirationType,
    expirationTypeCssClasses,
    LicenseScope,
    ProfessionalLicenseType,
    ProfessionalUserLicense,
    unvalidatedExpirationCssClass
} from '../../shared/models/professionals/licenses/professional-user-license';
import { LicenseFilter, ProfessionalLicense, WaterSupplierLicense } from '../../shared/models/professionals/licenses/water-supplier-license';
import { TceqLicense } from '../../shared/models/professionals/licenses/tceq-license';
import { LicenseService } from '../../shared/services/licenses/license.service';
import { ProfessionalAccountWindowService } from '../professional-account-window.service';
import { TceqLicenseCheckComponent, TceqLicenseCheckModalData } from '../tceq-check/tceq-license-check.component';

const TCEQ_BPAT_LICENSE_TYPE = 'TCEQ - BPAT License';
const HOURS_PER_DAY = 24;

interface LicenseVm extends WaterSupplierLicense {
    licenseTypeOptions: InputOption<ProfessionalLicenseType>[];
    expirationCssClass: string;
    lastCheckedText: string;
    canViewAccount: boolean;
    canCheckLicense: boolean;
    isSelected: boolean;
    isSaved: boolean;
}

@Component({
    selector: 'vp-license-list',
    templateUrl: './license-list.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class LicenseListComponent implements OnInit {
    @Input()
    public licenseFilter!: LicenseFilter;

    @ViewChild('accountCell', { static: true })
    public accountCell?: TemplateRef<CellTemplateData<LicenseVm>>;

    @ViewChild('licenseTypeCell', { static: true })
    public licenseTypeCell?: TemplateRef<CellTemplateData<LicenseVm>>;

    @ViewChild('licenseNumberCell', { static: true })
    public licenseNumberCell?: TemplateRef<CellTemplateData<LicenseVm>>;

    @ViewChild('expirationCell', { static: true })
    public expirationCell?: TemplateRef<CellTemplateData<LicenseVm>>;

    @ViewChild('lastCheckedCell', { static: true })
    public lastCheckedCell?: TemplateRef<CellTemplateData<LicenseVm>>;

    @ViewChild('actionsCell', { static: true })
    public actionsCell?: TemplateRef<CellTemplateData<LicenseVm>>;

    public table: TableViewModel<LicenseVm> = {
        query: {
            sort: {},
            filter: []
        }
    };

    private _licenseTypes: ProfessionalLicenseType[] = [];

    constructor(
        private readonly _licenseService: LicenseService,
        private readonly _accountWindowService: ProfessionalAccountWindowService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        try {
            this.table.isLoading = true;

            this._licenseTypes = await this._licenseService.getTypes();
        } finally {
            this.table.isLoading = false;
        }

        await this.getLicenses();
    }

    public async getLicenses(): Promise<void> {
        try {
            this.table.isLoading = true;

            const licenses = await this._licenseService.getAll(
                this.licenseFilter,
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

    public selectLicense(selected: LicenseVm): void {
        if (!this.table.items || !this.table.items.data.includes(selected)) {
            return;
        }

        for (const license of this.table.items.data) {
            license.isSelected = license === selected;
        }
    }

    public async saveLicense(license: LicenseVm): Promise<void> {
        try {
            this.table.isLoading = true;

            if (license.licenseScope === LicenseScope.Company) {
                const saved = await this._licenseService.updateCompanyLicense(this.toCompanyLicense(license));

                this.applySaved(license, saved.expirationDate, saved.expirationType);
            } else {
                const saved = await this._licenseService.update(this.toUserLicense(license));

                this.applySaved(license, saved.expirationDate, saved.expirationType);
            }

            this._toastService.successfullySaved('License');
        } finally {
            this.table.isLoading = false;
        }
    }

    public viewAccount(license: LicenseVm): void {
        this._accountWindowService.open(license);
    }

    public async checkLicense(license: LicenseVm): Promise<void> {
        const tceqLicense = await this.findTceqLicense(license);

        if (!tceqLicense) {
            this._toastService.show({
                text: 'License was not found in TCEQ.',
                type: ToastType.Warning
            });

            return;
        }

        license.hoursSinceLastCheck = 0;
        license.lastCheckedText = this.getLastCheckedText(0);

        this._modalHelper.show<TceqLicenseCheckModalData, void>(TceqLicenseCheckComponent, {
            title: license.licenseNumber!,
            model: { license, tceqLicense },
            size: ModalSize.large,
            mode: 'disableFullScreen'
        }).result().subscribe(async () => {
            await this.saveLicense(license);
        });
    }

    private async findTceqLicense(license: LicenseVm): Promise<TceqLicense | null> {
        try {
            this.table.isLoading = true;

            return await this._licenseService.checkTceq(license.id!);
        } finally {
            this.table.isLoading = false;
        }
    }

    private applySaved(license: LicenseVm, expirationDate?: string, expirationType?: ExpirationType): void {
        license.expirationDate = expirationDate;
        license.expirationType = expirationType;
        license.expirationCssClass = this.getExpirationCssClass(expirationDate, expirationType);
        license.isSaved = true;
    }

    private toUserLicense(license: LicenseVm): ProfessionalUserLicense {
        return {
            id: license.id,
            user: { id: license.userId },
            professionalType: license.professionalType,
            licenseType: { id: license.licenseTypeId },
            licenseNumber: license.licenseNumber,
            expirationDate: this._helper.toTextOrUndefined(license.expirationDate)
        };
    }

    private toCompanyLicense(license: LicenseVm): ProfessionalLicense {
        return {
            id: license.id,
            professionalType: license.professionalType,
            licenseType: { id: license.licenseTypeId },
            licenseNumber: license.licenseNumber,
            expirationDate: this._helper.toTextOrUndefined(license.expirationDate)
        };
    }

    private toViewModel(license: WaterSupplierLicense): LicenseVm {
        return {
            ...license,
            licenseTypeOptions: this.getLicenseTypeOptions(license),
            expirationCssClass: this.getExpirationCssClass(license.expirationDate, license.expirationType),
            lastCheckedText: this.getLastCheckedText(license.hoursSinceLastCheck),
            canViewAccount: this._accountWindowService.canOpen(license.professionalType),
            canCheckLicense: license.licenseTypeName === TCEQ_BPAT_LICENSE_TYPE,
            isSelected: false,
            isSaved: false
        };
    }

    private getLastCheckedText(hoursSinceLastCheck?: number): string {
        if (hoursSinceLastCheck == null) {
            return '---';
        }

        const days = Math.floor(hoursSinceLastCheck / HOURS_PER_DAY);
        const hours = hoursSinceLastCheck % HOURS_PER_DAY;

        return `${days}d ${hours}h`;
    }

    private getLicenseTypeOptions(license: WaterSupplierLicense): InputOption<ProfessionalLicenseType>[] {
        const options: InputOption<ProfessionalLicenseType>[] = [];

        for (const type of this._licenseTypes) {
            if (type.professionalType === license.professionalType && type.licenseScope === license.licenseScope) {
                options.push({ id: type.id, text: type.name, data: type });
            }
        }

        return options;
    }

    private getExpirationCssClass(expirationDate?: string, expirationType?: ExpirationType): string {
        if (!expirationDate || expirationType == null) {
            return unvalidatedExpirationCssClass;
        }

        return expirationTypeCssClasses[expirationType];
    }

    private getColumns(): TableColumn<LicenseVm>[] {
        return [
            { field: 'userEmail', caption: 'User ID', type: ColumnType.text },
            { field: 'companyName', caption: 'Account', type: ColumnType.text, cellTemplate: this.accountCell },
            { field: 'licenseTypeName', caption: 'License Type', type: ColumnType.text, cellTemplate: this.licenseTypeCell },
            { field: 'licenseNumber', caption: 'License #', type: ColumnType.text, cellTemplate: this.licenseNumberCell },
            { field: 'expirationDate', caption: 'Expiration', type: ColumnType.date, cellTemplate: this.expirationCell },
            { field: 'lastCheckDate', caption: 'Last Checked', type: ColumnType.dateTime, cellTemplate: this.lastCheckedCell },
            { field: 'id', caption: '', type: ColumnType.other, cellTemplate: this.actionsCell, queryColumnExcluded: true }
        ];
    }
}
