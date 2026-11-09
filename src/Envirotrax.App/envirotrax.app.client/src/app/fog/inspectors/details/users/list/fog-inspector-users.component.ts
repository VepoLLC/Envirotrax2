import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ProfessionalUser } from "../../../../../shared/models/professionals/professional-user";
import { ProfessionalType } from "../../../../../shared/models/professionals/licenses/professional-user-license";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { FogInspectorSubAccountsService } from "../../../../../shared/services/fog/fog-inspector-user.service";
import { AuthService } from "../../../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../../../shared/models/permission-type";
import { FeatureType } from "../../../../../shared/models/feature-type";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { EditFogInspectorUserComponent, FogUserModalData } from "../edit/edit-fog-inspector-user.component";
import { CellTemplateData, ColumnType, ModalHelperService, TableColumn } from "@envirotrax/common-ui";

@Component({
    selector: 'vp-fog-inspector-users',
    standalone: false,
    templateUrl: './fog-inspector-users.component.html'
})
export class FogInspectorUsersComponent implements OnInit {
    @Input() public inspectorId!: number;

    @ViewChild('paymentBypassTemplate', { static: true })
    public paymentBypassTemplate!: TemplateRef<CellTemplateData<ProfessionalUser>>;

    public readonly professionalType = ProfessionalType.FogInspector;

    public canManage: boolean = false;
    public canManagePaymentBypass: boolean = false;

    public table: TableViewModel<ProfessionalUser> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    constructor(
        private readonly _service: FogInspectorSubAccountsService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _authService: AuthService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.setPermissions();
        this.setupColumns();
        await this.loadSubAccounts();
    }

    private async setPermissions(): Promise<void> {
        const canEdit = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.FogInspectors);
        this.canManage = canEdit && await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalUsers);
        this.canManagePaymentBypass = canEdit && await this._authService.hasAnyFeatures(FeatureType.ManagePaymentBypass);
    }

    private setupColumns(): void {
        this.table.columns = this.getColumns();
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
        this._modalHelper.show<FogUserModalData, ProfessionalUser>(EditFogInspectorUserComponent, {
            title: 'Add User Account',
            model: { inspectorId: this.inspectorId, user: {} },
            size: ModalSize.large,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.loadSubAccounts());
    }

    public editUser(user: ProfessionalUser): void {
        this._modalHelper.show<FogUserModalData, ProfessionalUser>(EditFogInspectorUserComponent, {
            title: 'Edit User Account',
            model: { inspectorId: this.inspectorId, user },
            size: ModalSize.large,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.loadSubAccounts());
    }

    public async loadSubAccounts(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._service.getSubAccounts(
                this.inspectorId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }
}
