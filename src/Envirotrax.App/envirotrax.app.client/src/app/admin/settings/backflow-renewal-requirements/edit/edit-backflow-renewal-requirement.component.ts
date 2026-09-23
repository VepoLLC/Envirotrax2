import { Component } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { InputOption, ToastService } from '@envirotrax/common-ui';
import { BackflowRenewalRequirement } from "../../../../shared/models/settings/backflow-renewal-requirement";
import { BackflowRenewalRequirementService } from "../../../../shared/services/settings/backflow-renewal-requirement.service";
import { PropertyType } from "../../../../shared/enums/property-type.enum";
import { HelperService } from "../../../../shared/services/helpers/helper.service";

@Component({
    standalone: false,
    templateUrl: './edit-backflow-renewal-requirement.component.html'
})
export class EditBackflowRenewalRequirementComponent {
    public requirement: BackflowRenewalRequirement;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public readonly propertyTypeOptions: InputOption[] = [
        { id: PropertyType.Residential, text: 'Residential' },
        { id: PropertyType.Commercial, text: 'Commercial' }
    ];

    public readonly assemblyTypeOptions: InputOption[] = [
        { id: 'All', text: 'All Assembly Types' },
        { id: 'DC', text: 'Double Check Valve' },
        { id: 'DCD', text: 'Double Check Detector' },
        { id: 'DCD2', text: 'Double Check Detector Type II' },
        { id: 'RP', text: 'Reduced Pressure Principle' },
        { id: 'RPPD', text: 'Reduced Pressure Principle Detector' },
        { id: 'RPPD2', text: 'Reduced Pressure Principle Detector Type II' },
        { id: 'PVB', text: 'Pressure Vacuum Breaker' },
        { id: 'SVB', text: 'Spill-Resistant Pressure Vacuum Breaker' },
        { id: 'AG', text: 'Air Gap' }
    ];

    public readonly hazardTypeOptions: InputOption[] = [
        { id: 'All', text: 'All Hazard Types' },
        { id: 'Agricultural/Feed Lot', text: 'Agricultural/Feed Lot' },
        { id: 'Domestic/Premises Isolation', text: 'Domestic/Premises Isolation' },
        { id: 'Fire System', text: 'Fire System' },
        { id: 'Gas Station/Car Wash', text: 'Gas Station/Car Wash' },
        { id: 'Irrigation - Non Chemical', text: 'Irrigation - Non Chemical' },
        { id: 'Irrigation - Chemical Feed', text: 'Irrigation - Chemical Feed' },
        { id: 'Laundry/Cleaners', text: 'Laundry/Cleaners' },
        { id: 'Medical/Dental/Laboratory/Mortuary', text: 'Medical/Dental/Laboratory/Mortuary' },
        { id: 'Nails/Salon/Grooming', text: 'Nails/Salon/Grooming' },
        { id: 'Pool/Recreation/Athletics', text: 'Pool/Recreation/Athletics' },
        { id: 'Restaurant/Vending/Grocery', text: 'Restaurant/Vending/Grocery' },
        { id: 'Fire Hydrant/Temporary Construction', text: 'Fire Hydrant/Temporary Construction' },
        { id: 'Fountains/Garden Ponds/Water Features', text: 'Fountains/Garden Ponds/Water Features' },
        { id: 'Water Softener', text: 'Water Softener' },
        { id: 'Other', text: 'Other' }
    ];

    public readonly yearsOptions: InputOption[] = [
        { id: 1, text: '1' },
        { id: 2, text: '2' },
        { id: 3, text: '3' },
        { id: 4, text: '4' },
        { id: 5, text: '5' }
    ];

    constructor(
        private readonly _service: BackflowRenewalRequirementService,
        private readonly _modalReference: ModalReference<BackflowRenewalRequirement>,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this.requirement = { ...this._modalReference.config.model! };
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                const result = this.requirement.id
                    ? await this._service.update(this.requirement)
                    : await this._service.add(this.requirement);

                this._toastService.successfullySaved('Renewal Requirement');
                this._modalReference.closeSuccess(result);
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }

                this._toastService.failedToSave('Renewal Requirement');
            } finally {
                this.isLoading = false;
            }
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
