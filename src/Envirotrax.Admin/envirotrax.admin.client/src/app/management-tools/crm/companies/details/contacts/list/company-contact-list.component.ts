import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { ColumnType, InputOption, ModalHelperService, TableColumn, TableViewModel, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../../../shared/components/shared.components.module';
import { CompanyContact } from '../../../../../../shared/models/crm/company-contact';
import { State } from '../../../../../../shared/models/lookup/state';
import { CompanyContactService } from '../../../../../../shared/services/crm/company-contact.service';
import { AddEditCompanyContactComponent, CompanyContactModalData } from '../edit/add-edit-company-contact.component';

interface CompanyContactRow extends CompanyContact {
    stateCode: string;
}

/**
 * V1 Vepo Manager's Additional Contacts: the other people to contact at a company.
 */
@Component({
    selector: 'vp-company-contacts',
    templateUrl: './company-contact-list.component.html',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
})
export class CompanyContactListComponent implements OnInit {
    @Input()
    public companyId!: number;

    // The company window's states, with the blank option first. The contact editor uses them too.
    @Input()
    public states: InputOption<State>[] = [];

    // Without a sort of its own, the list keeps V1's order: the order the contacts were added.
    public table: TableViewModel<CompanyContactRow> = {
        query: {
            sort: {},
            filter: []
        }
    };

    private _stateCodes = new Map<number, string>();

    constructor(
        private readonly _contactService: CompanyContactService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();

        // The contacts carry only the state id, so the grid takes the codes from the window's states.
        this._stateCodes = new Map(this.states
            .filter(option => option.data?.id != null)
            .map(option => [option.data!.id!, option.data!.code ?? '']));

        await this.getContacts();
    }

    public async getContacts(): Promise<void> {
        try {
            this.table.isLoading = true;

            const contacts = await this._contactService.getAll(
                this.companyId,
                this.table.items?.pageInfo || {},
                this.table.query
            );

            this.table.items = {
                pageInfo: contacts.pageInfo,
                data: contacts.data.map(contact => this.toRow(contact))
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    public addContact(): void {
        this.showEditor('Add Contact', {});
    }

    public editContact(row: CompanyContactRow): void {
        const { stateCode, ...contact } = row;

        this.showEditor('Edit Contact', contact);
    }

    public deleteContact(row: CompanyContactRow): void {
        this._modalHelper.showDeleteConfirmation().result().subscribe(async () => {
            try {
                this.table.isLoading = true;

                await this._contactService.delete(this.companyId, row.id!);

                this._toastService.successFullyDeleted('Contact');
            } finally {
                this.table.isLoading = false;
            }

            await this.getContacts();
        });
    }

    private showEditor(title: string, contact: CompanyContact): void {
        this._modalHelper.show<CompanyContactModalData, CompanyContact>(AddEditCompanyContactComponent, {
            title,
            model: { companyId: this.companyId, contact, states: this.states },
            size: ModalSize.large,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.getContacts());
    }

    private toRow(contact: CompanyContact): CompanyContactRow {
        return {
            ...contact,
            stateCode: contact.state?.id != null ? this._stateCodes.get(contact.state.id) ?? '' : ''
        };
    }

    // V1's columns, in V1's order.
    private getColumns(): TableColumn<CompanyContactRow>[] {
        return [
            { field: 'position', caption: 'Position', type: ColumnType.text },
            { field: 'contactName', caption: 'Contact Name', type: ColumnType.text },
            { field: 'address', caption: 'Address', type: ColumnType.text },
            { field: 'city', caption: 'City', type: ColumnType.text },
            { field: 'stateCode', caption: 'State', type: ColumnType.text, sortColumnExcluded: true, queryColumnExcluded: true },
            { field: 'zipCode', caption: 'ZIP', type: ColumnType.text },
            { field: 'phoneNumber', caption: 'Phone Number', type: ColumnType.text },
            { field: 'cellNumber', caption: 'Cell Number', type: ColumnType.text },
            { field: 'emailAddress', caption: 'Email Address', type: ColumnType.text }
        ];
    }
}
