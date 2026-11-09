import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ProfessionalUser } from "../../../../../shared/models/professionals/professional-user";
import { ProfessionalType } from "../../../../../shared/models/professionals/licenses/professional-user-license";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { BackflowTesterUserService } from "../../../../../shared/services/backflow/backflow-tester-user.service";
import { CellTemplateData, ToastService, ColumnType, ModalHelperService, TableColumn } from '@envirotrax/common-ui';
import { AuthService } from "../../../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../../../shared/models/permission-type";
import { FeatureType } from "../../../../../shared/models/feature-type";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { AddEditBackflowTesterUserComponent, BackflowUserModalData } from "../edit/add-edit-backflow-tester-user.component";

@Component({
    selector: 'vp-backflow-tester-users',
    standalone: false,
    templateUrl: './backflow-tester-users.component.html'
})
export class BackflowTesterUsersComponent implements OnInit {
    @Input() public testerId!: number;

    @ViewChild('paymentBypassTemplate', { static: true })
    public paymentBypassTemplate!: TemplateRef<CellTemplateData<ProfessionalUser>>;

    public readonly professionalType = ProfessionalType.Bpat;

    public canManage: boolean = false;
    public canManagePaymentBypass: boolean = false;

    public table: TableViewModel<ProfessionalUser> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    constructor(
        private readonly _service: BackflowTesterUserService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _authService: AuthService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.setPermissions();
        this.table.columns = this.getColumns();
        await this.loadSubAccounts();
    }

    private async setPermissions(): Promise<void> {
        const canEdit = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.BackflowTesters);
        this.canManage = canEdit && await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalUsers);
        this.canManagePaymentBypass = canEdit && await this._authService.hasAnyFeatures(FeatureType.ManagePaymentBypass);
    }

    private getColumns(): TableColumn<ProfessionalUser>[] {
        const columns: TableColumn<ProfessionalUser>[] = [
            {
                field: 'emailAddress',
                caption: 'Email Address',
                type: ColumnType.text
            },
            {
                field: 'contactName',
                caption: 'Contact Name',
                type: ColumnType.text
            },
            {
                field: 'jobTitle',
                caption: 'Job Title',
                type: ColumnType.text
            }
        ];

        if (this.canManagePaymentBypass) {
            columns.push({
                field: '',
                caption: 'Payment Bypass',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.paymentBypassTemplate
            });
        }

        return columns;
    }

    public addUser(): void {
        this._modalHelper.show<BackflowUserModalData, ProfessionalUser>(AddEditBackflowTesterUserComponent, {
            title: 'Add User Account',
            model: { testerId: this.testerId, user: {} },
            size: ModalSize.large,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.loadSubAccounts());
    }

    public editUser(user: ProfessionalUser): void {
        this._modalHelper.show<BackflowUserModalData, ProfessionalUser>(AddEditBackflowTesterUserComponent, {
            title: 'Edit User Account',
            model: { testerId: this.testerId, user },
            size: ModalSize.large,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.loadSubAccounts());
    }

    public deleteUser(user: ProfessionalUser): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;
                await this._service.delete(this.testerId, user.id!);
                this._toastService.successFullyDeleted('User Account');
            } finally {
                this.table.isLoading = false;
            }
            await this.loadSubAccounts();
        });
    }

    public async loadSubAccounts(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._service.getSubAccounts(
                this.testerId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }
}
