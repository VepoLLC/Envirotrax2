import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import {
    CellTemplateData,
    ColumnType,
    ModalHelperService,
    TableColumn,
    TableViewModel,
    ToastService
} from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../shared.components.module';
import { ProfessionalNote } from '../../../models/professionals/professional-note';
import { ProfessionalType } from '../../../models/professionals/licenses/professional-user-license';
import { ProfessionalNoteService } from '../../../services/professionals/professional-note.service';
import { AddEditProfessionalNoteComponent, ProfessionalNoteModalData } from '../edit/add-edit-professional-note.component';

@Component({
    selector: 'vp-professional-notes',
    templateUrl: './professional-notes.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
    styles: [`
        .vp-note-text {
            white-space: pre-wrap;
        }
    `]
})
export class ProfessionalNotesComponent implements OnInit {
    @Input()
    public professionalId!: number;

    @Input()
    public professionalType!: ProfessionalType;

    @ViewChild('createdTemplate', { static: true })
    public createdTemplate!: TemplateRef<CellTemplateData<ProfessionalNote>>;

    @ViewChild('noteTextTemplate', { static: true })
    public noteTextTemplate!: TemplateRef<CellTemplateData<ProfessionalNote>>;

    public table: TableViewModel<ProfessionalNote> = {
        query: { sort: {}, filter: [] }
    };

    constructor(
        private readonly _service: ProfessionalNoteService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        await this.loadNotes();
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

    public addNote(): void {
        this.showEditor('Add Comment', {});
    }

    public editNote(note: ProfessionalNote): void {
        this.showEditor('Edit Comment', note);
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

    private showEditor(title: string, note: ProfessionalNote): void {
        this._modalHelper.show<ProfessionalNoteModalData, ProfessionalNote>(AddEditProfessionalNoteComponent, {
            title,
            model: { professionalType: this.professionalType, professionalId: this.professionalId, note },
            size: ModalSize.large,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.loadNotes());
    }

    private getColumns(): TableColumn<ProfessionalNote>[] {
        return [
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
    }
}
