import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, InputOption, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { FogDisposalSite, PhysicalType, physicalTypeOptions } from '../../../shared/models/fog/fog-disposal-site';
import { State } from '../../../shared/models/lookup/state';
import { FogDisposalSiteService } from '../../../shared/services/fog/fog-disposal-site.service';

export interface FogDisposalSiteModalData {
    site: FogDisposalSite;
    stateOptions: InputOption<State>[];
}

@Component({
    templateUrl: './add-edit-fog-disposal-site.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class AddEditFogDisposalSiteComponent {
    public site: FogDisposalSite;
    public stateId: string;
    public physicalTypeId: string;
    public latitude: string;
    public longitude: string;

    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public readonly isEditMode: boolean;
    public readonly stateOptions: InputOption<State>[];
    public readonly physicalTypeOptions: InputOption[] = physicalTypeOptions;

    constructor(
        private readonly _modalReference: ModalReference<FogDisposalSiteModalData, FogDisposalSite>,
        private readonly _disposalSiteService: FogDisposalSiteService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        const { site, stateOptions } = this._modalReference.config.model!;

        this.site = { ...site };
        this.isEditMode = !!site.id;
        this.stateOptions = [{ id: '', text: '' }, ...stateOptions];

        // The selects and number inputs bind strings; they are converted back when the site is saved.
        this.stateId = site.state?.id == null ? '' : String(site.state.id);
        this.physicalTypeId = String(site.physicalType ?? PhysicalType.Landfill);
        this.latitude = site.latitude == null ? '' : String(site.latitude);
        this.longitude = site.longitude == null ? '' : String(site.longitude);
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;

            const site = this.buildSite();

            const saved = this.isEditMode
                ? await this._disposalSiteService.update(site)
                : await this._disposalSiteService.add(site);

            this._toastService.successfullySaved('Disposal Site');
            this._modalReference.closeSuccess(saved);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this._toastService.failedToSave('Disposal Site');
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }

    private buildSite(): FogDisposalSite {
        return {
            ...this.site,
            state: this.stateId ? { id: Number(this.stateId) } : null,
            physicalType: Number(this.physicalTypeId) as PhysicalType,
            latitude: this.toCoordinate(this.latitude),
            longitude: this.toCoordinate(this.longitude)
        };
    }

    private toCoordinate(value: string): number | null {
        if (value == null || value === '') {
            return null;
        }

        const coordinate = Number(value);

        return isNaN(coordinate) ? null : coordinate;
    }
}
