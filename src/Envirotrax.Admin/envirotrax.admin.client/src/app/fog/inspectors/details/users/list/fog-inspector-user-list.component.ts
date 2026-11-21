import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { CheckboxCellComponent, ColumnType, ModalHelperService, TableColumn, TableCustomAction, TableViewModel, ToastService, ToastType } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { MODULE_COLORS } from '../../../../../shared/models/module-colors';
import { ProfessionalUser } from '../../../../../shared/models/professionals/professional';
import { ProfessionalUserWindowModel } from '../../../../../shared/models/professionals/professional-user-window';
import { AuthService } from '../../../../../shared/services/auth/auth.service';
import { FogInspectorUserService } from '../../../../../shared/services/fog/fog-inspector-user.service';
import { WindowService } from '../../../../../shared/services/window.service';
import { FogInspectorUserDetailsComponent } from '../details/fog-inspector-user-details.component';
import { AddEditFogInspectorUserComponent, FogInspectorUserModalData } from '../edit/add-edit-fog-inspector-user.component';

@Component({
    selector: 'vp-fog-inspector-users',
    templateUrl: './fog-inspector-user-list.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
})
export class FogInspectorUserListComponent implements OnInit {
    @Input()
    public professionalId!: number;

    @Input()
    public companyName = '';

    public table: TableViewModel<ProfessionalUser> = {
        query: {
            sort: { contactName: 'Asc' },
            filter: []
        }
    };

    public readonly customActions: TableCustomAction<ProfessionalUser>[] = [
        {
            text: 'Login',
            iconClass: 'fa-solid fa-right-to-bracket',
            action: user => this.loginAsUser(user)
        },
        {
            text: 'Turn On/Off Payment Bypass',
            iconClass: 'fa-solid fa-hand-holding-dollar',
            action: user => this.updatePaymentBypass(user, !user.isFogInspectionPaymentBypassed)
        }
    ];

    constructor(
        private readonly _userService: FogInspectorUserService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _authService: AuthService,
        private readonly _windowService: WindowService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.getUsers();
    }

    public async getUsers(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._userService.getAll(
                this.professionalId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public addUser(): void {
        this.showEditor('Add User Account', {});
    }

    // The user's details open in a window of their own, where the account is also edited.
    public viewUser(user: ProfessionalUser): void {
        this._windowService.addWindow<FogInspectorUserDetailsComponent, ProfessionalUserWindowModel>(FogInspectorUserDetailsComponent, {
            title: [this.companyName, user.contactName, user.emailAddress].filter(part => part).join(' - '),
            color: MODULE_COLORS.FOG,
            model: { professionalId: this.professionalId, userId: user.id! }
        });
    }

    public loginAsUser(user: ProfessionalUser): void {
        this._authService.loginAsProfessionalUser(this.professionalId, user.id!);
    }

    public updatePaymentBypass(user: ProfessionalUser, isPaymentBypassed: boolean): void {
        const message = isPaymentBypassed
            ? `Are you sure that you want to turn on payment bypass for ${user.contactName}? Their FOG inspection checkouts will be free.`
            : `Are you sure that you want to turn off payment bypass for ${user.contactName}? Their FOG inspection checkouts will require payment.`;

        this._modalHelper.confirm({ messages: [message] }).result().subscribe(async () => {
            try {
                this.table.isLoading = true;

                await this._userService.updatePaymentBypass(this.professionalId, user.id!, isPaymentBypassed);

                this._toastService.show({
                    text: isPaymentBypassed ? 'Payment bypass turned on.' : 'Payment bypass turned off.',
                    type: ToastType.Success
                });
            } finally {
                this.table.isLoading = false;
            }

            await this.getUsers();
        });
    }

    public deleteUser(user: ProfessionalUser): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;

                await this._userService.delete(this.professionalId, user.id!);

                this._toastService.successFullyDeleted('User Account');
            } catch (error) {
                this._toastService.show({ text: 'Failed to delete User Account.', type: ToastType.Error });

                return;
            } finally {
                this.table.isLoading = false;
            }

            await this.getUsers();
        });
    }

    private showEditor(title: string, user: ProfessionalUser): void {
        this._modalHelper.show<FogInspectorUserModalData, ProfessionalUser>(AddEditFogInspectorUserComponent, {
            title,
            model: { professionalId: this.professionalId, user },
            size: ModalSize.medium
        }).result().subscribe(() => this.getUsers());
    }

    private getColumns(): TableColumn<ProfessionalUser>[] {
        return [
            { field: 'emailAddress', caption: 'User ID', type: ColumnType.text },
            { field: 'contactName', caption: 'Contact Name', type: ColumnType.text },
            { field: 'jobTitle', caption: 'Job Title', type: ColumnType.text },
            { field: 'isFogInspectionPaymentBypassed', caption: 'Payment Bypass', cellComponent: CheckboxCellComponent, type: ColumnType.text }
        ];
    }
}
