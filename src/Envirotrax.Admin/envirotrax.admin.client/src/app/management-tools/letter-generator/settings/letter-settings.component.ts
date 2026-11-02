import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { HelperService, InputOption, ModalHelperService, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { BackflowExpiredType, BackflowExpiringType, BackflowNonCompliantType } from '../../../shared/models/water-suppliers/backflow-settings';
import { CsiImpendingType, CsiNonCompliantType, CsiPastDueType } from '../../../shared/models/water-suppliers/csi-settings';
import { LetterImageFitType } from '../../../shared/models/water-suppliers/letter-image-fit-type';
import { WaterSupplierDetails } from '../../../shared/models/water-suppliers/water-supplier-details';
import { LookupService } from '../../../shared/services/lookup/lookup.service';
import { WaterSupplierService } from '../../../shared/services/water-suppliers/water-supplier.service';

export interface LetterSettingsModalData {
    id: number;
}

type SettingsTab = 'backflow' | 'csi' | 'contact';
type BackflowLetterTab = 'renewal' | 'expiration' | 'nonCompliant' | 'unknownAssemblies';
type CsiLetterTab = 'impending' | 'pastDue' | 'nonCompliant';

@Component({
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
    templateUrl: './letter-settings.component.html'
})
export class LetterSettingsComponent implements OnInit {
    public details: WaterSupplierDetails = new WaterSupplierDetails();
    public states: InputOption[] = [];

    public activeTab: SettingsTab = 'backflow';
    public activeBackflowLetterTab: BackflowLetterTab = 'renewal';
    public activeCsiLetterTab: CsiLetterTab = 'impending';

    public isLoading: boolean = false;
    public validationErrors: string[] = [];

    public readonly expiringNoticeOptions: InputOption[] = [
        { id: BackflowExpiringType.Off, text: 'Off' },
        { id: BackflowExpiringType.TwoMonths, text: 'Expiring in 2 Months' },
        { id: BackflowExpiringType.NextMonth, text: 'Expiring Next Month' },
        { id: BackflowExpiringType.ThisMonth, text: 'Expiring This Month' }
    ];

    public readonly expiredNoticeOptions: InputOption[] = [
        { id: BackflowExpiredType.Off, text: 'Off' },
        { id: BackflowExpiredType.LastMonth, text: 'Expired Last Month' },
        { id: BackflowExpiredType.TwoMonthsAgo, text: 'Expired 2 Months Ago' },
        { id: BackflowExpiredType.ThreeMonthsAgo, text: 'Expired 3 Months Ago' },
        { id: BackflowExpiredType.ThisMonth, text: 'Expired This Month' }
    ];

    public readonly backflowNonCompliantOptions: InputOption[] = [
        { id: BackflowNonCompliantType.Off, text: 'Off' },
        { id: BackflowNonCompliantType.ThirtyPlusDays, text: 'Expired 30+ Days' },
        { id: BackflowNonCompliantType.SixtyPlusDays, text: 'Expired 60+ Days' },
        { id: BackflowNonCompliantType.NinetyPlusDays, text: 'Expired 90+ Days' },
        { id: BackflowNonCompliantType.OneTwentyPlusDays, text: 'Expired 120+ Days' },
        { id: BackflowNonCompliantType.OneFiftyPlusDays, text: 'Expired 150+ Days' },
        { id: BackflowNonCompliantType.OneEightyPlusDays, text: 'Expired 180+ Days' },
        { id: BackflowNonCompliantType.PastDueLastMonth, text: 'Expired Last Month' },
        { id: BackflowNonCompliantType.ExpiredTwoMonthsAgo, text: 'Expired 2 Months Ago' }
    ];

    public readonly unknownAssembliesOptions: InputOption[] = [
        { id: false, text: 'Off' },
        { id: true, text: 'On' }
    ];

    public readonly impendingNoticeOptions: InputOption[] = [
        { id: CsiImpendingType.Off, text: 'Off' },
        { id: CsiImpendingType.TwoMonths, text: 'Impending in 2 Months' },
        { id: CsiImpendingType.NextMonth, text: 'Impending Next Month' },
        { id: CsiImpendingType.ThisMonth, text: 'Impending This Month' }
    ];

    public readonly pastDueNoticeOptions: InputOption[] = [
        { id: CsiPastDueType.Off, text: 'Off' },
        { id: CsiPastDueType.LastMonth, text: 'Past Due Last Month' },
        { id: CsiPastDueType.TwoMonthsAgo, text: 'Past Due 2 Months Ago' },
        { id: CsiPastDueType.ThreeMonthsAgo, text: 'Past Due 3 Months Ago' },
        { id: CsiPastDueType.ThisMonth, text: 'Past Due This Month' }
    ];

    public readonly csiNonCompliantOptions: InputOption[] = [
        { id: CsiNonCompliantType.Off, text: 'Off' },
        { id: CsiNonCompliantType.ThirtyPlusDays, text: 'Non Compliant 30+ Days' },
        { id: CsiNonCompliantType.SixtyPlusDays, text: 'Non Compliant 60+ Days' },
        { id: CsiNonCompliantType.NinetyPlusDays, text: 'Non Compliant 90+ Days' },
        { id: CsiNonCompliantType.OneTwentyPlusDays, text: 'Non Compliant 120+ Days' },
        { id: CsiNonCompliantType.OneFiftyPlusDays, text: 'Non Compliant 150+ Days' },
        { id: CsiNonCompliantType.OneEightyPlusDays, text: 'Non Compliant 180+ Days' },
        { id: CsiNonCompliantType.PastDueLastMonth, text: 'Non Compliant Last Month' },
        { id: CsiNonCompliantType.ExpiredTwoMonthsAgo, text: 'Non Compliant 2 Months Ago' }
    ];

    public readonly imageFitTypeOptions: InputOption[] = [
        { id: LetterImageFitType.FitToHeight, text: 'Fit to Height' },
        { id: LetterImageFitType.FitToWidth, text: 'Fit to Width' }
    ];

    public readonly fontOptions: InputOption[] = [
        { id: 'Arial', text: 'Arial' }
    ];

    private readonly _waterSupplierId: number;

    constructor(
        private readonly _modalReference: ModalReference<LetterSettingsModalData, WaterSupplierDetails>,
        private readonly _waterSupplierService: WaterSupplierService,
        private readonly _lookupService: LookupService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService
    ) {
        this._waterSupplierId = this._modalReference.config.model!.id;
    }

    public async ngOnInit(): Promise<void> {
        try {
            this.isLoading = true;

            const [states, details] = await Promise.all([
                this._lookupService.getAllStates(),
                this._waterSupplierService.getDetails(this._waterSupplierId)
            ]);

            this.states = [{ id: '', text: '' }, ...states.map(state => ({ id: state.id, text: state.name ?? '' }))];
            this.details = details;
        } finally {
            this.isLoading = false;
        }
    }

    public async save(form: NgForm): Promise<void> {
        this.validationErrors = [];

        if (!form.valid) {
            return;
        }

        try {
            this.isLoading = true;

            const saved = await this._waterSupplierService.updateDetails(this._waterSupplierId, this.details);

            this._toastService.successfullySaved('Letter Settings');
            this._modalReference.closeSuccess(saved);
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this._toastService.failedToSave('Letter Settings');
        } finally {
            this.isLoading = false;
        }
    }

    public close(form: NgForm): void {
        if (!form.dirty) {
            this._modalReference.cancel();
            return;
        }

        this._modalHelper.confirm({
            messages: ['Changes have been made to the water supplier settings. Do you want to close without saving them?']
        }).result().subscribe(() => this._modalReference.cancel());
    }
}
