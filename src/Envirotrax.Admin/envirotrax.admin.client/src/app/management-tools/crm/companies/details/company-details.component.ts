import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { HelperService, InputOption, ModalHelperService, ToastService, allSettled } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { Company, ContactType, contactTypeIconClasses, contactTypeOptions } from '../../../../shared/models/crm/company';
import { State } from '../../../../shared/models/lookup/state';
import { AppUser } from '../../../../shared/models/users/app-user';
import { AuthService } from '../../../../shared/services/auth/auth.service';
import { CompanyService } from '../../../../shared/services/crm/company.service';
import { LookupService } from '../../../../shared/services/lookup/lookup.service';
import { WindowReference } from '../../../../window/window-config';
import { CompanyContactListComponent } from './contacts/list/company-contact-list.component';

export interface CompanyWindowModel {
    id?: number;

    // Called after the company is saved or deleted, so the search window that opened it can reload.
    onChanged?: () => void;
}

interface NextContactInterval {
    days?: number;
    months?: number;
}

// The intervals V1 Vepo Manager's Set Next Date offers, inferred from the gaps between the last and next contact
// dates it saved.
const NEXT_CONTACT_INTERVALS: Record<string, NextContactInterval> = {
    '1w': { days: 7 },
    '2w': { days: 14 },
    '3w': { days: 21 },
    '1m': { months: 1 },
    '2m': { months: 2 },
    '3m': { months: 3 },
    '6m': { months: 6 },
    '1y': { months: 12 }
};

/**
 * V1 Vepo Manager's contact window: the company's contact information, contact settings and additional contacts.
 * The contact log comes with a later ticket.
 */
@Component({
    templateUrl: './company-details.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        CompanyContactListComponent
    ],
    providers: [DatePipe],
    styles: `
    /* Side by side when the window fits two 34rem columns (the widest Contact Settings row with its label beside it),
       otherwise each section takes the full width. */
    .company-details-column {
        flex: 1 1 34rem;
        min-width: 0;
    }
    `
})
export class CompanyDetailsComponent implements OnInit {
    private static _newWindowCounter: number = 0;

    public companyId: number = 0;

    // Several of these windows can be open at once, so element ids must be unique per window
    // or a label's `for` would toggle the checkbox in a different window.
    public idPrefix: string = 'company';

    public company: Company = { contactType: ContactType.ExistingClient };

    public states: InputOption<State>[] = [];
    public users: InputOption<AppUser>[] = [];

    public readonly contactTypeOptions: InputOption[] = contactTypeOptions;

    public readonly nextContactIntervalOptions: InputOption[] = [
        { id: '', text: '' },
        { id: '1w', text: '1 Week' },
        { id: '2w', text: '2 Weeks' },
        { id: '3w', text: '3 Weeks' },
        { id: '1m', text: '1 Month' },
        { id: '2m', text: '2 Months' },
        { id: '3m', text: '3 Months' },
        { id: '6m', text: '6 Months' },
        { id: '1y', text: '1 Year' }
    ];

    // The select options carry string ids, so the selects bind these and map back to the company on change.
    public contactTypeId: string = String(ContactType.ExistingClient);
    public stateId: string = '';
    public lastContactedById: string = '';
    public nextContactInterval: string = '';

    public contactTypeIconClass: string = contactTypeIconClasses[ContactType.ExistingClient];

    public isLoading: boolean = false;
    public isSaving: boolean = false;
    public isSaved: boolean = false;
    public validationErrors: string[] = [];

    constructor(
        private readonly _windowReference: WindowReference<CompanyWindowModel>,
        private readonly _companyService: CompanyService,
        private readonly _lookupService: LookupService,
        private readonly _authService: AuthService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _helper: HelperService,
        private readonly _toastService: ToastService,
        private readonly _datePipe: DatePipe
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.companyId = this._windowReference.config.model?.id ?? 0;
        this.idPrefix = this.companyId
            ? `company-${this.companyId}`
            : `company-new-${++CompanyDetailsComponent._newWindowCounter}`;

        try {
            this.isLoading = true;

            const [states, users, company] = await allSettled([
                this._lookupService.getStatesAsOptions(true),
                this._companyService.getCrmUsers(),
                this.companyId ? this._companyService.get(this.companyId) : Promise.resolve(this.company)
            ]);

            this.states = states;
            this.users = [
                { id: '', text: '' },
                ...users.data.map(user => this.toUserOption({ id: user.id, email: user.emailAddress }))
            ];

            this.setCompany(company);
        } finally {
            this.isLoading = false;
        }
    }

    public onContactTypeChange(value: string): void {
        this.company.contactType = Number(value) as ContactType;
        this.contactTypeIconClass = contactTypeIconClasses[this.company.contactType];
    }

    public onStateChange(value: string): void {
        this.company.state = this.states.find(option => option.id === value)?.data ?? null;
    }

    public onLastContactedByChange(value: string): void {
        this.company.lastContactedBy = this.users.find(option => option.id === value)?.data ?? null;
    }

    public async setLastContactedByToMe(): Promise<void> {
        const userId = await this._authService.getUserId();

        if (!userId) {
            return;
        }

        const email = await this._authService.getUserEmail();

        this.addUserOption({ id: userId, email });

        this.lastContactedById = String(userId);
        this.onLastContactedByChange(this.lastContactedById);
    }

    public setNextContactDate(): void {
        const interval = NEXT_CONTACT_INTERVALS[this.nextContactInterval];

        if (!interval) {
            return;
        }

        const today = new Date();
        const nextContactDate = interval.months
            ? this.addMonths(today, interval.months)
            : new Date(today.getFullYear(), today.getMonth(), today.getDate() + (interval.days ?? 0));

        this.company.nextContactDate = this._datePipe.transform(nextContactDate, 'yyyy-MM-dd');
    }

    public sendEmail(): void {
        if (this.company.emailAddress) {
            window.location.href = `mailto:${this.company.emailAddress}`;
        }
    }

    public async save(detailsForm: NgForm): Promise<void> {
        this.isSaved = false;
        this.validationErrors = [];

        if (!detailsForm.valid) {
            return;
        }

        try {
            this.isSaving = true;

            const saved = this.company.id
                ? await this._companyService.update(this.company)
                : await this._companyService.add(this.company);

            this.setCompany(saved);

            this.isSaved = true;
            this._toastService.successfullySaved('Company');
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this._toastService.failedToSave('Company');
            return;
        } finally {
            this.isSaving = false;
        }

        this._windowReference.config.model?.onChanged?.();
    }

    public deleteCompany(): void {
        if (!this.company.id) {
            this.close();
            return;
        }

        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.isSaving = true;

                await this._companyService.delete(this.company.id!);

                this._toastService.successFullyDeleted('Company');
            } finally {
                this.isSaving = false;
            }

            this._windowReference.config.model?.onChanged?.();
            this.close();
        });
    }

    public close(): void {
        this._windowReference.close();
    }

    private setCompany(company: Company): void {
        this.company = company;
        this.companyId = company.id ?? 0;

        this.contactTypeId = String(company.contactType ?? ContactType.ExistingClient);
        this.contactTypeIconClass = contactTypeIconClasses[company.contactType ?? ContactType.ExistingClient];
        this.stateId = company.state?.id == null ? '' : String(company.state.id);

        // A migrated company can name a V1 administrator who isn't a user of the CRM's water supplier.
        if (company.lastContactedBy?.id != null) {
            this.addUserOption(company.lastContactedBy);
        }

        this.lastContactedById = company.lastContactedBy?.id == null ? '' : String(company.lastContactedBy.id);

        if (company.id) {
            this._windowReference.config.title = `${company.id} - ${company.name}`;
        }
    }

    // Keeps the day of the month, or the month's last day when the target month is shorter (Jan 31 + 1 month = Feb 28).
    private addMonths(date: Date, months: number): Date {
        const firstOfTargetMonth = new Date(date.getFullYear(), date.getMonth() + months, 1);
        const lastDayOfTargetMonth = new Date(firstOfTargetMonth.getFullYear(), firstOfTargetMonth.getMonth() + 1, 0).getDate();

        return new Date(firstOfTargetMonth.getFullYear(), firstOfTargetMonth.getMonth(), Math.min(date.getDate(), lastDayOfTargetMonth));
    }

    private addUserOption(user: AppUser): void {
        const id = String(user.id);
        const existing = this.users.find(option => option.id === id);

        if (existing) {
            // The save response names the user by id only, so keep the email the option already has.
            existing.data = { ...user, email: user.email ?? existing.data?.email };
            return;
        }

        this.users = [...this.users, this.toUserOption(user)];
    }

    private toUserOption(user: AppUser): InputOption<AppUser> {
        return {
            id: String(user.id),
            text: user.email ?? String(user.id),
            data: user
        };
    }
}
