import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { CellTemplateData, ColumnType, ModalHelperService, TableColumn, ToastService } from '@envirotrax/common-ui';
import { ProfessionalNote } from "../../../models/professionals/professional-note";
import { ProfessionalType } from "../../../models/professionals/licenses/professional-user-license";
import { PermissionAction, PermissionType } from "../../../models/permission-type";
import { TableViewModel } from "../../../models/table-view-model";
import { AuthService } from "../../../services/auth/auth.service";
import { ProfessionalNoteService } from "../../../services/professionals/professional-note.service";
import { AddEditProfessionalNoteComponent, ProfessionalNoteModalData } from "../edit/add-edit-professional-note.component";

@Component({
    selector: 'vp-professional-notes',
    standalone: false,
    templateUrl: './professional-notes.component.html',
    styles: [`
        .vp-note-text {
            white-space: pre-wrap;
        }
    `]
})
export class ProfessionalNotesComponent implements OnInit {
    private static readonly _permissionTypes: { [type: number]: PermissionType } = {
        [ProfessionalType.Bpat]: PermissionType.BackflowTesters,
        [ProfessionalType.CsiInspector]: PermissionType.CsiInspectors,
        [ProfessionalType.FogInspector]: PermissionType.FogInspectors,
        [ProfessionalType.FogTransporter]: PermissionType.FogTransporters
    };

    @Input() public professionalId!: number;
    @Input() public professionalType!: ProfessionalType;

    @ViewChild('createdTemplate', { static: true })
    public createdTemplate!: TemplateRef<CellTemplateData<ProfessionalNote>>;

    @ViewChild('noteTextTemplate', { static: true })
    public noteTextTemplate!: TemplateRef<CellTemplateData<ProfessionalNote>>;

    @ViewChild('actionsTemplate', { static: true })
    public actionsTemplate!: TemplateRef<CellTemplateData<ProfessionalNote>>;

    public canModify: boolean = false;

    public table: TableViewModel<ProfessionalNote> = {
        columns: [],
        query: { sort: {}, filter: [] }
    };

    constructor(
        private readonly _service: ProfessionalNoteService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService,
        private readonly _authService: AuthService
    ) { }

    public async ngOnInit(): Promise<void> {
        const permissionType = ProfessionalNotesComponent._permissionTypes[this.professionalType];
        this.canModify = await this._authService.hasAnyPermisison(PermissionAction.CanModify, permissionType);
        this.table.columns = this.getColumns();

        await this.loadNotes();
    }

    private getColumns(): TableColumn<ProfessionalNote>[] {
        const columns: TableColumn<ProfessionalNote>[] = [
            {
                field: 'createdTime',
                caption: 'Date',
                type: ColumnType.dateTime,
                headerCssClass: 'text-center',
                cellTemplate: this.createdTemplate
            },
            {
                field: 'noteText',
                caption: 'Comment',
                type: ColumnType.text,
                headerCssClass: 'text-center w-100',
                cellTemplate: this.noteTextTemplate
            }
        ];

        if (this.canModify) {
            columns.push({
                field: '',
                caption: '',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.actionsTemplate
            });
        }

        return columns;
    }

    public addNote(): void {
        this.showEditModal('Add Comment', {});
    }

    public editNote(note: ProfessionalNote): void {
        this.showEditModal('Edit Comment', note);
    }

    private showEditModal(title: string, note: ProfessionalNote): void {
        this._modalHelper.show<ProfessionalNoteModalData, ProfessionalNote>(AddEditProfessionalNoteComponent, {
            title,
            model: { professionalType: this.professionalType, professionalId: this.professionalId, note },
            size: ModalSize.large,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.loadNotes());
    }

    public deleteNote(note: ProfessionalNote): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;
                await this._service.delete(this.professionalType, this.professionalId, note.id!);
                this._toastService.successFullyDeleted('Comment');
            } finally {
                this.table.isLoading = false;
            }

            await this.loadNotes();
        });
    }

    public async loadNotes(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._service.getAll(
                this.professionalType,
                this.professionalId,
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }
}
