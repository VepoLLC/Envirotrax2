import { Component, OnInit } from "@angular/core";
import { ProfessionalUser } from "../../../shared/models/professionals/professional-user";
import { ProfesionalUserService } from "../../../shared/services/professionals/professional-user.service";
import { ActivatedRoute } from "@angular/router";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";
import { ModalHelperService } from "../../../shared/services/helpers/modal-helper.service";
import { ToastService } from "../../../shared/services/toast.service";
import { InputOption } from "../../../shared/components/input/input.component";

@Component({
    standalone: false,
    templateUrl: './edit-professional-user.component.html'
})
export class EditProfessionalUserComponent implements OnInit {
    public user: ProfessionalUser = {};
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public selectedJobFunctions: string[] = [];

    public readonly jobFunctionOptions: InputOption[] = [
        { id: 'isWiseGuy', text: 'Wise Guy' },
        { id: 'isCsiInspector', text: 'CSI Inspector' },
        { id: 'isBackflowTester', text: 'Backflow Tester' },
        { id: 'isFogInspector', text: 'FOG Inspector' },
        { id: 'isFogTransporter', text: 'FOG Transporter' }
    ];

    constructor(
        private readonly _userService: ProfesionalUserService,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _helper: HelperService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this._activatedRoute.paramMap.subscribe(async params => {
            const userId = params.get('id');

            if (userId) {
                await this.getUser(+userId);
            }
        });
    }

    private async getUser(id: number): Promise<void> {
        try {
            this.isLoading = true;
            this.user = await this._userService.get(id);
            this.selectedJobFunctions = this.getJobFunctionsFromUser(this.user);
        } finally {
            this.isLoading = false;
        }
    }

    public resendInvitation(): void {
        this._modalHelper.confirm({
            messages: ['Are you sure you want to resend the invitation to this user?']
        }).result().subscribe(async () => {
            try {
                this.isLoading = true;
                await this._userService.resendInvitation(this.user.id!);
                this._toastService.successfullySaved('Invitation');
            } finally {
                this.isLoading = false;
            }
        });
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                this.applyJobFunctions();
                await this._userService.update(this.user);
                this._toastService.successfullySaved('User');
            } catch (error) {
                if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                    throw error;
                }

                this._toastService.failedToSave('User');
            } finally {
                this.isLoading = false;
            }
        }
    }

    private getJobFunctionsFromUser(user: ProfessionalUser): string[] {
        const selected: string[] = [];
        if (user.isWiseGuy) selected.push('isWiseGuy');
        if (user.isCsiInspector) selected.push('isCsiInspector');
        if (user.isBackflowTester) selected.push('isBackflowTester');
        if (user.isFogInspector) selected.push('isFogInspector');
        if (user.isFogTransporter) selected.push('isFogTransporter');
        return selected;
    }

    private applyJobFunctions(): void {
        this.user.isWiseGuy = this.selectedJobFunctions.includes('isWiseGuy');
        this.user.isCsiInspector = this.selectedJobFunctions.includes('isCsiInspector');
        this.user.isBackflowTester = this.selectedJobFunctions.includes('isBackflowTester');
        this.user.isFogInspector = this.selectedJobFunctions.includes('isFogInspector');
        this.user.isFogTransporter = this.selectedJobFunctions.includes('isFogTransporter');
    }
}