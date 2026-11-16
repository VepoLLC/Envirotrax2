import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { ModalHelperService } from "@envirotrax/common-ui";
import { Professional } from "../../../../../shared/models/professionals/professional";
import { ProfessionalUser } from "../../../../../shared/models/professionals/professional-user";
import { ProfessionalType } from "../../../../../shared/models/professionals/licenses/professional-user-license";
import { PermissionAction, PermissionType } from "../../../../../shared/models/permission-type";
import { FeatureType } from "../../../../../shared/models/feature-type";
import { AuthService } from "../../../../../shared/services/auth/auth.service";
import { BackflowTesterManagementService } from "../../../../../shared/services/backflow/backflow-tester-management.service";
import { BackflowTesterUserService } from "../../../../../shared/services/backflow/backflow-tester-user.service";
import { AddEditBackflowTesterUserComponent, BackflowUserModalData } from "../edit/add-edit-backflow-tester-user.component";

@Component({
    standalone: false,
    templateUrl: './backflow-tester-user-details.component.html'
})
export class BackflowTesterUserDetailsComponent implements OnInit, OnDestroy {
    public readonly ProfessionalType = ProfessionalType;

    public professionalId: number | null = null;
    public userId: number | null = null;
    public isLoading: boolean = false;
    public isUserLoading: boolean = false;
    public professional: Professional | null = null;
    public user: ProfessionalUser | null = null;
    public canManageUser: boolean = false;

    private _routeSub?: Subscription;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _testerService: BackflowTesterManagementService,
        private readonly _userService: BackflowTesterUserService,
        private readonly _authService: AuthService,
        private readonly _modalHelper: ModalHelperService
    ) { }

    public async ngOnInit(): Promise<void> {
        this._routeSub = this._activatedRoute.paramMap.subscribe(async params => {
            this.professionalId = Number(params.get('id')) || null;
            this.userId = Number(params.get('userId')) || null;

            await this.loadAccount();
        });

        const canModify = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.BackflowTesters);
        this.canManageUser = canModify && await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalUsers);
    }

    public ngOnDestroy(): void {
        this._routeSub?.unsubscribe();
    }

    public editUser(): void {
        this._modalHelper.show<BackflowUserModalData, ProfessionalUser>(AddEditBackflowTesterUserComponent, {
            title: 'Edit User Account',
            model: { testerId: this.professionalId!, user: this.user! },
            size: ModalSize.large,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.loadUser());
    }

    private async loadAccount(): Promise<void> {
        this.professional = null;
        this.user = null;

        if (!this.professionalId || !this.userId) {
            return;
        }

        try {
            this.isLoading = true;

            [this.professional, this.user] = await Promise.all([
                this._testerService.getAccountInfo(this.professionalId),
                this._userService.get(this.professionalId, this.userId)
            ]);
        } finally {
            this.isLoading = false;
        }
    }

    private async loadUser(): Promise<void> {
        try {
            this.isUserLoading = true;
            this.user = await this._userService.get(this.professionalId!, this.userId!);
        } finally {
            this.isUserLoading = false;
        }
    }
}
