import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { CheckboxCellComponent, ColumnType, ModalHelperService, TableColumn, TableCustomAction, TableViewModel, ToastService, ToastType } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { ProfessionalUser } from '../../../../../shared/models/professionals/professional';
import { FogTransporterUserService } from '../../../../../shared/services/fog/fog-transporter-user.service';
import { AddEditFogTransporterUserComponent, FogTransporterUserModalData } from '../edit/add-edit-fog-transporter-user.component';

@Component({
    selector: 'vp-fog-transporter-users',
    templateUrl: './fog-transporter-user-list.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
})
export class FogTransporterUserListComponent implements OnInit {
    @Input()
    public professionalId!: number;

    public table: TableViewModel<ProfessionalUser> = {
        query: {
            sort: { contactName: 'Asc' },
            filter: []
        }
    };

    public readonly customActions: TableCustomAction<ProfessionalUser>[] = [
        {
            text: 'Turn On/Off Payment Bypass',
            iconClass: 'fa-solid fa-hand-holding-dollar',
            action: user => this.updatePaymentBypass(user, !user.isFogTransportationPaymentBypassed)
        }
    ];

    constructor(
        private readonly _userService: FogTransporterUserService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
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

    public editUser(user: ProfessionalUser): void {
        this.showEditor('Edit User Account', user);
    }

    public updatePaymentBypass(user: ProfessionalUser, isPaymentBypassed: boolean): void {
        const message = isPaymentBypassed
            ? `Are you sure that you want to turn on payment bypass for ${user.contactName}? Their trip ticket checkouts will be free.`
            : `Are you sure that you want to turn off payment bypass for ${user.contactName}? Their trip ticket checkouts will require payment.`;

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
        this._modalHelper.show<FogTransporterUserModalData, ProfessionalUser>(AddEditFogTransporterUserComponent, {
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
            { field: 'isFogTransportationPaymentBypassed', caption: 'Payment Bypass', cellComponent: CheckboxCellComponent, type: ColumnType.text }
        ];
    }
}
