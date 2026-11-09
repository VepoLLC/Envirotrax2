import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { ToastService, ToastType } from '@envirotrax/common-ui';
import { ProfessionalUser } from "../../models/professionals/professional-user";
import { ProfessionalType } from "../../models/professionals/licenses/professional-user-license";
import { ProfessionalPaymentBypassService } from "../../services/professionals/professional-payment-bypass.service";

@Component({
    selector: 'vp-professional-payment-bypass',
    standalone: false,
    templateUrl: './professional-payment-bypass.component.html'
})
export class ProfessionalPaymentBypassComponent implements OnInit {
    private static readonly _paymentBypassFields: { [type: number]: keyof ProfessionalUser } = {
        [ProfessionalType.Bpat]: 'isBackflowTestingPaymentBypassed',
        [ProfessionalType.CsiInspector]: 'isCsiInspectionPaymentBypassed',
        [ProfessionalType.FogInspector]: 'isFogInspectionPaymentBypassed',
        [ProfessionalType.FogTransporter]: 'isFogTransportationPaymentBypassed'
    };

    @Input() public professionalType!: ProfessionalType;
    @Input() public professionalId!: number;
    @Input() public user!: ProfessionalUser;

    @Output() public loadingChange = new EventEmitter<boolean>();

    public isPaymentBypassed = false;
    public isLoading = false;

    constructor(
        private readonly _service: ProfessionalPaymentBypassService,
        private readonly _toastService: ToastService
    ) { }

    public ngOnInit(): void {
        this.isPaymentBypassed = this.readPaymentBypassed(this.user);
    }

    public async togglePaymentBypass(): Promise<void> {
        try {
            this.setLoading(true);

            const updated = await this._service.update(this.professionalType, this.professionalId, this.user.id!, !this.isPaymentBypassed);
            this.isPaymentBypassed = this.readPaymentBypassed(updated);

            this._toastService.show({
                text: this.isPaymentBypassed ? 'Payment bypass turned on.' : 'Payment bypass turned off.',
                type: ToastType.Success
            });
        } finally {
            this.setLoading(false);
        }
    }

    private setLoading(isLoading: boolean): void {
        this.isLoading = isLoading;
        this.loadingChange.emit(isLoading);
    }

    private readPaymentBypassed(user: ProfessionalUser): boolean {
        const field = ProfessionalPaymentBypassComponent._paymentBypassFields[this.professionalType];

        return !!user[field];
    }
}
