import { Component, Input, OnInit } from "@angular/core";
import { ProfessionalUser } from "../../../../../shared/models/professionals/professional-user";
import { TableViewModel } from "../../../../../shared/models/table-view-model";
import { FogTransporterSubAccountsService } from "../../../../../shared/services/fog/fog-transporter-user.service";
import { AuthService } from "../../../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../../../shared/models/permission-type";
import { FeatureType } from "../../../../../shared/models/feature-type";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { EditFogTransporterUserComponent, FogUserModalData } from "../edit/edit-fog-transporter-user.component";
import { ColumnType, ModalHelperService, TableColumn } from "@envirotrax/common-ui";

@Component({
    selector: 'vp-fog-transporter-users',
    standalone: false,
    templateUrl: './fog-transporter-users.component.html'
})
export class FogTransporterUsersComponent implements OnInit {
    @Input() public transporterId!: number;

    public canManage: boolean = false;

    public table: TableViewModel<ProfessionalUser> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    constructor(
        private readonly _service: FogTransporterSubAccountsService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _authService: AuthService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.setPermissions();
        this.setupColumns();
        await this.loadSubAccounts();
    }

    private async setPermissions(): Promise<void> {
        const canEdit = await this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.FogTransporters);
        this.canManage = canEdit && await this._authService.hasAnyFeatures(FeatureType.ManageProfessionalUsers);
    }

    private setupColumns(): void {
        this.table.columns = this.getColumns();
    }

    private getColumns(): TableColumn<ProfessionalUser>[] {
        return [
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
    }

    public addUser(): void {
        this._modalHelper.show<FogUserModalData, ProfessionalUser>(EditFogTransporterUserComponent, {
            title: 'Add Sub Account',
            model: { transporterId: this.transporterId, user: {} },
            size: ModalSize.large
        }).result().subscribe(() => this.loadSubAccounts());
    }

    public editUser(user: ProfessionalUser): void {
        this._modalHelper.show<FogUserModalData, ProfessionalUser>(EditFogTransporterUserComponent, {
            title: 'Edit Sub Account',
            model: { transporterId: this.transporterId, user },
            size: ModalSize.large
        }).result().subscribe(() => this.loadSubAccounts());
    }

    public async loadSubAccounts(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._service.getSubAccounts(
                this.transporterId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }
}
