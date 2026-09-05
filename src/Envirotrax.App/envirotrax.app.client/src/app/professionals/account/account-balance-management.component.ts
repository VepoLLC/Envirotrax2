import { Component, OnInit, ViewChild } from "@angular/core";
import { NgForm } from "@angular/forms";
import { LookupService } from "../../shared/services/lookup/lookup.service";
import { State } from "../../shared/models/lookup/state";
import { Professional } from "../../shared/models/professionals/professional";
import { ProfessionalUser } from "../../shared/models/professionals/professional-user";
import { ProfesisonalService } from "../../shared/services/professionals/professional.service";
import { ProfesionalUserService } from "../../shared/services/professionals/professional-user.service";
import { HelperService } from "../../shared/services/helpers/helper.service";
import { InputOption, ToastService } from '@envirotrax/common-ui';
import { CreditCardPaymentComponent, CreditCardToken } from "../../shared/components/credit-card-payment/credit-card-payment.component";
import { ProfessionalAccountBalance } from "../../shared/models/professionals/professional-account-balance";

@Component({
    standalone: false,
    templateUrl: './account-balance-management.component.html'
})
export class AccountBalanceManagementComponent implements OnInit {
    @ViewChild(CreditCardPaymentComponent)
    public creditCardPayment!: CreditCardPaymentComponent;

    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public professional: Professional = {};
    public professionalUser: ProfessionalUser = {};
    public amountToAdd?: number;
    public cardToken?: CreditCardToken;

    public states: InputOption<State>[] = [];

    constructor(
        private readonly _lookupService: LookupService,
        private readonly _professionalService: ProfesisonalService,
        private readonly _professionalUserService: ProfesionalUserService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) { }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;

            const [states, professional, professionalUser] = await Promise.all([
                this._lookupService.getAllStatesAsOptions(true),
                this._professionalService.getLoggedInProfessional(),
                this._professionalUserService.getMyData()
            ]);

            this.states = states;
            this.professional = professional;
            this.professionalUser = professionalUser;
        } finally {
            this.isLoading = false;
        }
    }

    public stateChanged(stateId: number): void {
        this.professionalUser.billingState = stateId ? { id: stateId } : undefined;
    }

    public onTokenCaptured(token: CreditCardToken, form: NgForm): void {
        this.cardToken = token;
        this.save(form);
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        const amountEntered = this.amountToAdd != null && (this.amountToAdd as unknown as string) !== '';
        if (amountEntered && !this.isPositiveNumber(this.amountToAdd)) {
            this.validationErrors.push('Adding amount must be a positive number.');
        }

        if (form.valid && !this.validationErrors.length) {
            try {
                this.isLoading = true;

                const request: ProfessionalAccountBalance = {
                    amountToAdd: amountEntered ? Number(this.amountToAdd) : 0,
                    dataDescriptor: this.cardToken!.dataDescriptor,
                    dataValue: this.cardToken!.dataValue,
                    billingFirstName: this.professionalUser.billingFirstName!,
                    billingLastName: this.professionalUser.billingLastName!,
                    billingAddress: this.professionalUser.billingAddress!,
                    billingCity: this.professionalUser.billingCity!,
                    billingState: this.professionalUser.billingState!,
                    billingZipCode: this.professionalUser.billingZipCode!
                };

                this.professional = await this._professionalService.updateMyAccountBalance(request);

                this.amountToAdd = undefined;
                this.cardToken = undefined;
                this.creditCardPayment.reset();

                // Clear the form's submitted/touched state so the now-empty (but optional again)
                // Amount to Add field doesn't keep showing a stale "required" validation error.
                form.resetForm({ ...form.value, amountToAdd: undefined });

                this._toastService.successfullySaved('Account Balance');
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }

                // The card token is single-use — even on a decline, it's already spent on
                // Authorize.Net's side, so require a fresh "Enter Card Details" before retrying.
                this.cardToken = undefined;
                this.creditCardPayment.reset();

                this._toastService.failedToSave('Account Balance');
            } finally {
                this.isLoading = false;
            }
        }
    }

    private isPositiveNumber(value: unknown): boolean {
        const parsed = Number(value);
        return !isNaN(parsed) && parsed > 0;
    }
}
