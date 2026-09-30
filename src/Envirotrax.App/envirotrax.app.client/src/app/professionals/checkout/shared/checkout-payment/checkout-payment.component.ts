import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from "@angular/core";
import { NgForm } from "@angular/forms";
import { InputOption } from '@envirotrax/common-ui';
import { State } from "../../../../shared/models/lookup/state";
import { CreditCardPayment } from "../../../../shared/models/payments/credit-card-payment";
import { ProfessionalUser } from "../../../../shared/models/professionals/professional-user";
import { ProfesionalUserService } from "../../../../shared/services/professionals/professional-user.service";
import { LookupService } from "../../../../shared/services/lookup/lookup.service";
import { CreditCardPaymentComponent, CreditCardToken } from "../../../../shared/components/credit-card-payment/credit-card-payment.component";

@Component({
    selector: 'vp-checkout-payment',
    standalone: false,
    templateUrl: './checkout-payment.component.html'
})
export class CheckoutPaymentComponent implements OnInit {
    @Input() public cardCharge = 0;
    @Input() public isLoading = false;
    @Input() public validationErrors: string[] = [];

    @Output() public paymentSubmitted = new EventEmitter<CreditCardPayment | undefined>();

    public professionalUser: ProfessionalUser = {};
    public states: InputOption<State>[] = [];

    @ViewChild(CreditCardPaymentComponent)
    public creditCardPayment?: CreditCardPaymentComponent;

    constructor(
        private readonly _professionalUserService: ProfesionalUserService,
        private readonly _lookupService: LookupService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        const [currentUser, states] = await Promise.all([
            this._professionalUserService.getMyData(),
            this._lookupService.getAllStatesAsOptions(true)
        ]);

        this.professionalUser = currentUser;
        this.states = states;
    }

    public reset(): void {
        this.creditCardPayment?.reset();
    }

    public stateChanged(stateId: number): void {
        this.professionalUser.billingState = stateId ? { id: stateId } : undefined;
    }

    public onCardTokenCaptured(token: CreditCardToken, form: NgForm): void {
        if (form.invalid) {
            return;
        }

        this.paymentSubmitted.emit({
            dataDescriptor: token.dataDescriptor,
            dataValue: token.dataValue,
            billingFirstName: this.professionalUser.billingFirstName!,
            billingLastName: this.professionalUser.billingLastName!,
            billingAddress: this.professionalUser.billingAddress!,
            billingCity: this.professionalUser.billingCity!,
            billingState: this.professionalUser.billingState!,
            billingZipCode: this.professionalUser.billingZipCode!
        });
    }

    public completeSubmission(): void {
        this.paymentSubmitted.emit(undefined);
    }
}
