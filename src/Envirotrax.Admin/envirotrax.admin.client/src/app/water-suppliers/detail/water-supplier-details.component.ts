import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { HelperService, InputOption } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../shared/components/shared.components.module';
import { WindowReference } from '../../window/window-config';
import { WaterSupplier } from '../../shared/models/water-suppliers/water-supplier';
import { GeneralSettings } from '../../shared/models/water-suppliers/general-settings';
import { BackflowOutOfServiceType, BackflowSettings, BackflowTestingMethodType } from '../../shared/models/water-suppliers/backflow-settings';
import { WaterSupplierUser } from '../../shared/models/water-suppliers/water-supplier-user';
import { WaterSupplierService } from '../../shared/services/water-suppliers/water-supplier.service';
import { WaterSupplierUserService } from '../../shared/services/water-suppliers/water-supplier-user.service';
import { LookupService } from '../../shared/services/lookup/lookup.service';

type DetailTab = 'general' | 'users';

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
    public users: WaterSupplierUser[] = [];

    public states: InputOption[] = [];

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
    public usersInitialized: boolean = false;

    public isLoading: boolean = false;
    public isLoadingUsers: boolean = false;
    public isSaving: boolean = false;
    public isSaved: boolean = false;
    public saveFailed: boolean = false;
    public validationErrors: string[] = [];

    private supplierId: number = 0;

    constructor(
        private readonly _waterSupplierService: WaterSupplierService,
        private readonly _waterSupplierUserService: WaterSupplierUserService,
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

        if (tab === 'users' && !this.usersInitialized) {
            this.usersInitialized = true;
            this.loadUsers();
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

    private async loadUsers(): Promise<void> {
        try {
            this.isLoadingUsers = true;

            this.users = await this._waterSupplierUserService.getAll(this.supplierId);
        } finally {
            this.isLoadingUsers = false;
        }
    }
}
