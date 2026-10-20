import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { HelperService, InputOption, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { State } from '../../../shared/models/lookup/state';
import { Professional } from '../../../shared/models/professionals/professional';
import { BackflowTesterService } from '../../../shared/services/backflow/backflow-tester.service';
import { LookupService } from '../../../shared/services/lookup/lookup.service';
import { WindowReference } from '../../../window/window-config';
import { BackflowTesterGaugeListComponent } from './gauges/list/backflow-tester-gauge-list.component';
import { BackflowTesterInsuranceListComponent } from './insurances/list/backflow-tester-insurance-list.component';
import { BackflowTesterLicenseListComponent } from './licenses/list/backflow-tester-license-list.component';
import { BackflowTesterUserListComponent } from './users/list/backflow-tester-user-list.component';
import { BackflowTesterWaterSupplierListComponent } from './water-suppliers/list/backflow-tester-water-supplier-list.component';

/**
 * The account fields are saved by the Save button. Registrations, user accounts, licenses, insurances and gauges
 * each save through their own edit dialogs, the way V1's BPAT account page edited them one record at a time.
 */
@Component({
    templateUrl: './backflow-tester-details.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        BackflowTesterWaterSupplierListComponent,
        BackflowTesterUserListComponent,
        BackflowTesterLicenseListComponent,
        BackflowTesterInsuranceListComponent,
        BackflowTesterGaugeListComponent
    ],
})
export class BackflowTesterDetailsComponent implements OnInit {
    public professionalId: number = 0;

    // Several of these windows can be open at once, so element ids must be unique per window
    // or a label's `for` would toggle the checkbox in a different window.
    public idPrefix: string = 'backflow-tester';

    public professional: Professional = {};

    public states: InputOption<State>[] = [];

    // The state options carry string ids, so the selects bind these and map back to the professional on change.
    public stateId: string = '';
    public defaultStateId: string = '';

    public isLoading: boolean = false;
    public isSaving: boolean = false;
    public isSaved: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _windowReference: WindowReference<Professional>,
        private readonly _testerService: BackflowTesterService,
        private readonly _lookupService: LookupService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.professionalId = this._windowReference.config.model?.id ?? 0;
        this.idPrefix = `backflow-tester-${this.professionalId}`;

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
        this.isSaved = false;
        this.validationErrors = [];

        if (!detailsForm.valid) {
            return;
        }

        try {
            this.isSaving = true;

            this.professional = await this._testerService.update(this.professional);
            this.setStateIds();

            this.isSaved = true;
            this._toastService.successfullySaved('Backflow Tester Account');
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this._toastService.failedToSave('Backflow Tester Account');
        } finally {
            this.isSaving = false;
        }
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
            this.professional = await this._testerService.get(this.professionalId);
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
