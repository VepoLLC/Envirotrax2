import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { ColumnType, ModalHelperService, TableColumn, TableViewModel, ToastService, ToastType } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../shared/components/shared.components.module';
import { ProfessionalUser } from '../../../../../shared/models/professionals/professional';
import { CsiInspectorUserService } from '../../../../../shared/services/csi/csi-inspector-user.service';
import { AddEditCsiInspectorUserComponent, CsiInspectorUserModalData } from '../edit/add-edit-csi-inspector-user.component';

@Component({
    selector: 'vp-csi-inspector-users',
    templateUrl: './csi-inspector-user-list.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
})
export class CsiInspectorUserListComponent implements OnInit {
    @Input()
    public professionalId!: number;

    public table: TableViewModel<ProfessionalUser> = {
        query: {
            sort: { contactName: 'Asc' },
            filter: []
        }
    };

    constructor(
        private readonly _userService: CsiInspectorUserService,
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
        this._modalHelper.show<CsiInspectorUserModalData, ProfessionalUser>(AddEditCsiInspectorUserComponent, {
            title,
            model: { professionalId: this.professionalId, user },
            size: ModalSize.medium
        }).result().subscribe(() => this.getUsers());
    }

    private getColumns(): TableColumn<ProfessionalUser>[] {
        return [
            { field: 'emailAddress', caption: 'User ID', type: ColumnType.text },
            { field: 'contactName', caption: 'Contact Name', type: ColumnType.text },
            { field: 'jobTitle', caption: 'Job Title', type: ColumnType.text }
        ];
    }
}
