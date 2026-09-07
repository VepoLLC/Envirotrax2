import { Component, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { ModalReference, ModalSize } from "@developer-partners/ngx-modal-dialog";
import { InputOption, ModalHelperService, ToastService } from "@envirotrax/common-ui";
import { NotificationSetting } from "../../../shared/models/notifications/notification-setting";
import { NotificationSettingService } from "../../../shared/services/notifications/notification-setting.service";
import { NotificationOptionsService } from "../../../shared/services/notifications/notification-options.service";
import { WaterSupplierUser } from "../../../shared/models/users/water-supplier-user";
import { WaterSupplierUserLookupComponent } from "../../../shared/components/lookups/water-supplier-user-lookup/water-supplier-user-lookup.component";
import { HelperService } from "../../../shared/services/helpers/helper.service";

@Component({
    standalone: false,
    templateUrl: './edit-notification-setting.component.html'
})
export class EditNotificationSettingComponent implements OnInit {
    public setting: NotificationSetting;
    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public readonly reasonForTestOptions: InputOption[];
    public readonly intervalOptions: InputOption[];
    public readonly deliveryTypeOptions: InputOption[];
    public readonly colorOptions: string[];

    constructor(
        private readonly _service: NotificationSettingService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _modalReference: ModalReference<NotificationSetting>,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService,
        options: NotificationOptionsService
    ) {
        this.setting = { ...this._modalReference.config.model! };

        this.reasonForTestOptions = options.reasonForTestOptions;
        this.intervalOptions = options.intervalOptions;
        this.deliveryTypeOptions = options.deliveryTypeOptions;
        this.colorOptions = options.colorOptions;
    }

    public async ngOnInit(): Promise<void> {
        if (this.setting.id) {
            await this.getSetting(this.setting.id);
        }
    }

    private async getSetting(id: number): Promise<void> {
        try {
            this.isLoading = true;

            this.setting = await this._service.get(id);
        } finally {
            this.isLoading = false;
        }
    }

    public lookupAccount(): void {
        this._modalHelper.show<WaterSupplierUser>(WaterSupplierUserLookupComponent, {
            title: 'Accounts',
            size: ModalSize.large
        }).result()
            .subscribe(user => {
                this.setting.userId = user.id;
                this.setting.user = user;
            });
    }

    public setColor(color: string): void {
        this.setting.color = color;
    }

    public onPropertyTypeAnyChanged(): void {
        if (this.setting.propertyTypeAny) {
            this.setting.propertyTypeResidential = false;
            this.setting.propertyTypeCommercial = false;
        }
    }

    public onFilterAnyChanged(): void {
        if (this.setting.filterAny) {
            this.setting.filterFailedTest = false;
            this.setting.filterPassingTest = false;
            this.setting.filterUnknownSerialNumber = false;
            this.setting.filterInactiveProperty = false;
            this.setting.filterNonCompliance = false;
            this.setting.filterPotableNonPotableMismatch = false;
            this.setting.filterDuplicateTest = false;
            this.setting.filterOutOfService = false;
            this.setting.filterContainsRemarks = false;
            this.setting.filterBackflowNotProperlyInstalled = false;
            this.setting.filterFeeExempt = false;
            this.setting.filterHasOnSiteSewageFacility = false;
            this.setting.filterHasAuxWaterSupply = false;
            this.setting.filterSubmissionDaysExceeded = false;
            this.setting.filterSubmissionDaysExceededDays = 0;
        }
    }

    public onHazardTypeAnyChanged(): void {
        if (this.setting.hazardTypeAny) {
            this.setting.hazardTypeAgriculturalFeedLot = false;
            this.setting.hazardTypeDomesticPremisesIsolation = false;
            this.setting.hazardTypeFireSystem = false;
            this.setting.hazardTypeFireHydrantTemporaryConstruction = false;
            this.setting.hazardTypeGasStationCarWash = false;
            this.setting.hazardTypeIrrigationNonChemical = false;
            this.setting.hazardTypeIrrigationChemicalFeed = false;
            this.setting.hazardTypeLaundryCleaners = false;
            this.setting.hazardTypeMedicalDentalLaboratoryMortuary = false;
            this.setting.hazardTypeNailsSalonGrooming = false;
            this.setting.hazardTypePoolRecreationAthletics = false;
            this.setting.hazardTypeRestaurantVendingGrocery = false;
            this.setting.hazardTypeFountainsGardenPondsWaterFeatures = false;
            this.setting.hazardTypeWaterSoftener = false;
            this.setting.hazardTypeOther = false;
        }
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                const result = this.setting.id
                    ? await this._service.update(this.setting)
                    : await this._service.add(this.setting);

                this._toastService.successfullySaved('Notification Setting');
                this._modalReference.closeSuccess(result);
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }

                this._toastService.failedToSave('Notification Setting');
            } finally {
                this.isLoading = false;
            }
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
