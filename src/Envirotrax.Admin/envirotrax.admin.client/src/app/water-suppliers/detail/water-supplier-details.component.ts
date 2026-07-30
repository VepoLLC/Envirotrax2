import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { HelperService, InputOption } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../shared/components/shared.components.module';
import { WindowReference } from '../../window/window-config';
import { WaterSupplier } from '../../shared/models/water-suppliers/water-supplier';
import { GeneralSettings } from '../../shared/models/water-suppliers/general-settings';
import { BackflowOutOfServiceType, BackflowSettings, BackflowTestingMethodType } from '../../shared/models/water-suppliers/backflow-settings';
import { UserAccountPermission, WaterSupplierUserAccount } from '../../shared/models/water-suppliers/water-supplier-user-account';
import { WaterSupplierService } from '../../shared/services/water-suppliers/water-supplier.service';
import { LookupService } from '../../shared/services/lookup/lookup.service';
import { PermissionType } from '../../shared/models/permission-type';

type DetailTab = 'general' | 'userAccounts';

interface PermissionCell {
    label: string;
    text: string;
    cssClass: string;
}

interface PermissionGroup {
    header: string;
    cells: PermissionCell[];
}

interface UserAccountRow {
    contactName: string;
    emailAddress: string;
    cellNumber: string;
    groups: PermissionGroup[];
}

interface PermissionColumn {
    label: string;
    type: PermissionType;
}

@Component({
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
    templateUrl: './water-supplier-details.component.html'
})
export class WaterSupplierDetailsComponent implements OnInit {
    public supplier: WaterSupplier = {};
    public generalSettings: GeneralSettings = {};
    public backflowSettings: BackflowSettings = {};
    public userAccounts: UserAccountRow[] = [];

    public states: InputOption[] = [];

    public readonly permissionGroupHeaders: string[] = [
        'General Permissions',
        'CSI Permissions',
        'Backflow Permissions',
        'FOG Permissions'
    ];

    public readonly testingMethodOptions: InputOption[] = [
        { id: BackflowTestingMethodType.USC, text: 'USC' },
        { id: BackflowTestingMethodType.ASSE, text: 'ASSE' },
        { id: BackflowTestingMethodType.TREEO, text: 'TREEO' }
    ];

    public readonly outOfServiceOptions: InputOption[] = [
        { id: BackflowOutOfServiceType.VepoManaged, text: 'Vepo Managed' },
        { id: BackflowOutOfServiceType.WaterSupplierManaged, text: 'Water Supplier Managed' }
    ];

    public activeTab: DetailTab = 'general';
    public userAccountsInitialized: boolean = false;

    public isLoading: boolean = false;
    public isLoadingUserAccounts: boolean = false;
    public isSaving: boolean = false;
    public isSaved: boolean = false;
    public saveFailed: boolean = false;
    public validationErrors: string[] = [];

    private supplierId: number = 0;

    private readonly _generalColumns: PermissionColumn[] = [
        { label: 'Account Information', type: PermissionType.AccountInformation },
        { label: 'User Accounts', type: PermissionType.Users },
        { label: 'Notifications', type: PermissionType.Notifications },
        { label: 'Property Records', type: PermissionType.Sites }
    ];

    private readonly _csiColumns: PermissionColumn[] = [
        { label: 'CSI Inspections', type: PermissionType.CsiInspections },
        { label: 'Inspector Management', type: PermissionType.CsiInspectors },
        { label: 'Reports', type: PermissionType.CsiReports }
    ];

    private readonly _backflowColumns: PermissionColumn[] = [
        { label: 'Backflow Tests', type: PermissionType.BackflowTests },
        { label: 'BPAT Management', type: PermissionType.BackflowTesters },
        { label: 'Out of Service', type: PermissionType.BackflowOutOfService },
        { label: 'Reports', type: PermissionType.BackflowReports }
    ];

    private readonly _fogColumns: PermissionColumn[] = [
        { label: 'Trip Tickets', type: PermissionType.FogTripTickets },
        { label: 'Vehicle Management', type: PermissionType.FogVehicles },
        { label: 'Transporter Management', type: PermissionType.FogTransporters },
        { label: 'Inspections', type: PermissionType.FogInspections },
        { label: 'Inspector Management', type: PermissionType.FogInspectors },
        { label: 'Reports', type: PermissionType.FogReports }
    ];

    constructor(
        private readonly _waterSupplierService: WaterSupplierService,
        private readonly _lookupService: LookupService,
        private readonly _helper: HelperService,
        private readonly _windowRef: WindowReference<{ id?: number }>
    ) {

    }

    public ngOnInit(): void {
        const id = this._windowRef.config.model?.id;

        if (id) {
            this.supplierId = id;
            this.loadDetails();
        }
    }

    public setActiveTab(tab: DetailTab): void {
        this.activeTab = tab;

        if (tab === 'userAccounts' && !this.userAccountsInitialized) {
            this.userAccountsInitialized = true;
            this.loadUserAccounts();
        }
    }

    private async loadDetails(): Promise<void> {
        try {
            this.isLoading = true;

            const [states, details] = await Promise.all([
                this._lookupService.getAllStates(),
                this._waterSupplierService.getDetails(this.supplierId)
            ]);

            this.states = [{ id: '', text: '' }, ...states.map(state => ({ id: state.id, text: state.name ?? '' }))];

            this.supplier = details.waterSupplier;
            this.generalSettings = details.generalSettings;
            this.backflowSettings = details.backflowSettings;
        } finally {
            this.isLoading = false;
        }
    }

    public async save(form: NgForm): Promise<void> {
        if (!form.valid) {
            return;
        }

        this.isSaved = false;
        this.saveFailed = false;
        this.validationErrors = [];

        try {
            this.isSaving = true;

            const saved = await this._waterSupplierService.updateDetails(this.supplierId, {
                waterSupplier: this.supplier,
                generalSettings: this.generalSettings,
                backflowSettings: this.backflowSettings
            });

            this.supplier = saved.waterSupplier;
            this.generalSettings = saved.generalSettings;
            this.backflowSettings = saved.backflowSettings;

            this.isSaved = true;
        } catch (error) {
            const validationErrors: string[] = [];

            this.saveFailed = !this._helper.parseValidationErrors(error, validationErrors);
            this.validationErrors = validationErrors;
        } finally {
            this.isSaving = false;
        }
    }

    private async loadUserAccounts(): Promise<void> {
        try {
            this.isLoadingUserAccounts = true;

            const accounts = await this._waterSupplierService.getUserAccounts(this.supplierId);

            this.userAccounts = accounts.map(account => this.toRow(account));
        } finally {
            this.isLoadingUserAccounts = false;
        }
    }

    private toRow(account: WaterSupplierUserAccount): UserAccountRow {
        const byType = new Map<PermissionType, UserAccountPermission>();

        for (const permission of account.permissions ?? []) {
            if (permission.permission != null) {
                byType.set(permission.permission, permission);
            }
        }

        return {
            contactName: account.contactName ?? '',
            emailAddress: account.emailAddress ?? '',
            cellNumber: account.cellNumber ?? '',
            groups: [
                { header: 'General', cells: this._generalColumns.map(column => this.toCell(column, byType)) },
                { header: 'CSI', cells: this._csiColumns.map(column => this.toCell(column, byType)) },
                { header: 'Backflow', cells: this._backflowColumns.map(column => this.toCell(column, byType)) },
                { header: 'FOG', cells: this._fogColumns.map(column => this.toCell(column, byType)) }
            ]
        };
    }

    private toCell(column: PermissionColumn, byType: Map<PermissionType, UserAccountPermission>): PermissionCell {
        const permission = byType.get(column.type);

        if (permission?.canModify) {
            return { label: column.label, text: 'Modify', cssClass: 'text-success' };
        }

        if (permission?.canView) {
            return { label: column.label, text: 'View', cssClass: 'text-primary' };
        }

        return { label: column.label, text: 'Deny', cssClass: 'text-danger' };
    }
}
