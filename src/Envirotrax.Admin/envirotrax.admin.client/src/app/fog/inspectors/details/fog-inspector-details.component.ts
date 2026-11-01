import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { HelperService, InputOption, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { State } from '../../../shared/models/lookup/state';
import { Professional } from '../../../shared/models/professionals/professional';
import { FogInspectorService } from '../../../shared/services/fog/fog-inspector.service';
import { LookupService } from '../../../shared/services/lookup/lookup.service';
import { WindowReference } from '../../../window/window-config';
import { FogInspectorInsuranceListComponent } from './insurances/list/fog-inspector-insurance-list.component';
import { FogInspectorLicenseListComponent } from './licenses/list/fog-inspector-license-list.component';
import { FogInspectorUserListComponent } from './users/list/fog-inspector-user-list.component';
import { FogInspectorWaterSupplierListComponent } from './water-suppliers/list/fog-inspector-water-supplier-list.component';

@Component({
    templateUrl: './fog-inspector-details.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        FogInspectorWaterSupplierListComponent,
        FogInspectorUserListComponent,
        FogInspectorLicenseListComponent,
        FogInspectorInsuranceListComponent
    ],
})
export class FogInspectorDetailsComponent implements OnInit {
    public professionalId: number = 0;

    public idPrefix: string = 'fog-inspector';

    public professional: Professional = {};

    public states: InputOption<State>[] = [];

    public stateId: string = '';
    public defaultStateId: string = '';

    public isLoading: boolean = false;
    public isSaving: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _windowReference: WindowReference<Professional>,
        private readonly _fogInspectorService: FogInspectorService,
        private readonly _lookupService: LookupService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.professionalId = this._windowReference.config.model?.id ?? 0;
        this.idPrefix = `fog-inspector-${this.professionalId}`;

        await Promise.all([
            this.loadStates(),
            this.loadProfessional()
        ]);
    }

    public onStateChange(value: string): void {
        this.professional.state = this.states.find(option => option.id === value)?.data;
    }

    public onDefaultStateChange(value: string): void {
        this.professional.defaultState = this.states.find(option => option.id === value)?.data;
    }

    public async save(detailsForm: NgForm): Promise<void> {
        this.validationErrors = [];

        if (!detailsForm.valid) {
            return;
        }

        try {
            this.isSaving = true;

            this.professional = await this._fogInspectorService.update(this.normalizeContacts(this.professional));
            this.setStateIds();

            this._toastService.successfullySaved('FOG Inspector Account');
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this._toastService.failedToSave('FOG Inspector Account');
        } finally {
            this.isSaving = false;
        }
    }

    /**
     * vp-input hands back the raw <input> string, so a cleared field arrives as ''. The DTO's [EmailAddress] and
     * [Phone] accept null but reject '', so a cleared email or phone would fail validation instead of clearing.
     * Sending undefined leaves the property out of the JSON, which the server binds as null.
     */
    private normalizeContacts(professional: Professional): Professional {
        return {
            ...professional,
            companyEmail: professional.companyEmail || undefined,
            phoneNumber: professional.phoneNumber || undefined,
            cellPhoneNumber: professional.cellPhoneNumber || undefined,
            faxNumber: professional.faxNumber || undefined
        };
    }

    private async loadStates(): Promise<void> {
        this.states = await this._lookupService.getStatesAsOptions(true);
    }

    private async loadProfessional(): Promise<void> {
        if (!this.professionalId) {
            return;
        }

        try {
            this.isLoading = true;
            this.professional = await this._fogInspectorService.get(this.professionalId);
            this.setStateIds();
        } finally {
            this.isLoading = false;
        }
    }

    private setStateIds(): void {
        this.stateId = this.professional.state?.id == null ? '' : String(this.professional.state.id);
        this.defaultStateId = this.professional.defaultState?.id == null ? '' : String(this.professional.defaultState.id);
    }
}
