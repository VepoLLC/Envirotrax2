import { Component } from "@angular/core";
import { ProfessionalUser } from "../../../shared/models/professionals/professional-user";
import { ProfesionalUserService } from "../../../shared/services/professionals/professional-user.service";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { NgForm } from "@angular/forms";
import { InputOption } from "../../../shared/components/input/input.component";

@Component({
    standalone: false,
    templateUrl: './create-professional-user.component.html'
})
export class CreateProfessionalUserComponent {
    public isLoading: boolean = false;
    public validationErrors: string[] = [];
    public user: ProfessionalUser = {};
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
        private readonly _modalReference: ModalReference<ProfessionalUser>,
        private readonly _helper: HelperService
    ) {

    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;
                this.validationErrors = [];

                this.applyJobFunctions();
                const result = await this._userService.add(this.user);
                this._modalReference.closeSuccess(result);
            } catch (e) {
                if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                    throw e;
                }
            } finally {
                this.isLoading = false;
            }
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }

    private applyJobFunctions(): void {
        this.user.isWiseGuy = this.selectedJobFunctions.includes('isWiseGuy');
        this.user.isCsiInspector = this.selectedJobFunctions.includes('isCsiInspector');
        this.user.isBackflowTester = this.selectedJobFunctions.includes('isBackflowTester');
        this.user.isFogInspector = this.selectedJobFunctions.includes('isFogInspector');
        this.user.isFogTransporter = this.selectedJobFunctions.includes('isFogTransporter');
    }
}