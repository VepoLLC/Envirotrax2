import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { HelperService, InputOption, RecordLog, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { FogTripTicket, FogTripTicketUpdateRequest } from '../../../shared/models/fog/fog-trip-ticket';
import { State } from '../../../shared/models/lookup/state';
import { MODULE_COLORS } from '../../../shared/models/module-colors';
import { FogTripTicketService } from '../../../shared/services/fog/fog-trip-ticket.service';
import { LookupService } from '../../../shared/services/lookup/lookup.service';
import { WindowService } from '../../../shared/services/window.service';
import { SiteEditComponent } from '../../../sites/edit/site-edit.component';
import { WindowReference } from '../../../window/window-config';
import { FogTripTicketGeneratorComponent } from './generator/fog-trip-ticket-generator.component';
import { FogTripTicketReceiverComponent } from './receiver/fog-trip-ticket-receiver.component';
import { FogTripTicketTransporterComponent } from './transporter/fog-trip-ticket-transporter.component';

/**
 * V1 Vepo Manager's trip ticket window. Water Supplier and Transporter are read-only; the Generator, Remarks and
 * the receiver's contact name and delivery date are edited through child sections that share this component's
 * NgForm and ticket object. One Save at the bottom submits the whole page.
 */
@Component({
    templateUrl: './fog-trip-ticket-details.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        FogTripTicketGeneratorComponent,
        FogTripTicketTransporterComponent,
        FogTripTicketReceiverComponent
    ],
})
export class FogTripTicketDetailsComponent implements OnInit {
    public id: number = 0;

    public isLoading: boolean = false;
    public isLoadingRecordLogs: boolean = false;
    public isSaving: boolean = false;

    public ticket: FogTripTicket = {};
    public recordLogs: RecordLog[] = [];
    public stateOptions: InputOption<State>[] = [];
    public validationErrors: string[] = [];

    // Display values, computed once after load - never called from the template.
    public waterSupplierHeader: string = 'Water Supplier';
    public waterSupplierCityStateZip: string = '';
    public recordLogTitle: string = 'Record Log';

    constructor(
        private readonly _windowReference: WindowReference<{ id?: number }>,
        private readonly _tripTicketService: FogTripTicketService,
        private readonly _lookupService: LookupService,
        private readonly _windowService: WindowService,
        private readonly _toastService: ToastService,
        private readonly _helper: HelperService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this.id = this._windowReference.config.model?.id ?? 0;

        await Promise.all([
            this.loadTicket(),
            this.loadStates(),
            this.loadRecordLogs()
        ]);
    }

    public async save(detailsForm: NgForm): Promise<void> {
        this.validationErrors = this.validateForSave();

        if (!detailsForm.valid || this.validationErrors.length > 0) {
            return;
        }

        try {
            this.isSaving = true;

            this.ticket = await this._tripTicketService.update(
                this.id,
                this.ticket.waterSupplier?.id ?? 0,
                this.buildUpdateRequest());

            this.setDisplayValues(this.ticket);
            detailsForm.form.markAsPristine();

            this._toastService.successfullySaved('FOG Trip Ticket');

            await this.loadRecordLogs();
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }

            this._toastService.failedToSave('FOG Trip Ticket');
        } finally {
            this.isSaving = false;
        }
    }

    public openSite(): void {
        const siteId = this.ticket.site?.id;

        if (siteId == null) {
            return;
        }

        this._windowService.addWindow(SiteEditComponent, {
            title: this.ticket.site?.accountNumber ?? 'Site',
            color: MODULE_COLORS.SITE,
            model: {
                siteId: siteId,
                waterSupplierId: this.ticket.waterSupplier?.id
            }
        });
    }

    /** [required] already rejects an empty amount; the submit form also rejects 0, which [min]="0" lets through. */
    private validateForSave(): string[] {
        const amount = this._helper.toNumberOrUndefined(this.ticket.interceptorWasteRemovedAmount);

        return amount != null && amount <= 0 ? ['The amount of waste removed must be greater than 0.'] : [];
    }

    /** vp-input hands back the raw input text, so numbers and cleared dates are converted before the PUT. */
    private buildUpdateRequest(): FogTripTicketUpdateRequest {
        const ticket = this.ticket;
        const helper = this._helper;

        return {
            propertyType: ticket.propertyType,
            propertyBusinessName: ticket.propertyBusinessName,
            propertyStreetNumber: ticket.propertyStreetNumber,
            propertyStreetName: ticket.propertyStreetName,
            propertyNumber: ticket.propertyNumber,
            propertyCity: ticket.propertyCity,
            propertyState: ticket.propertyState,
            propertyZip: ticket.propertyZip,
            fogGeneratorPhoneNumber: ticket.fogGeneratorPhoneNumber,
            fogGeneratorEmailAddress: ticket.fogGeneratorEmailAddress,
            generatorContactName: ticket.generatorContactName,

            interceptorType: ticket.interceptorType,
            interceptorOtherDescription: ticket.interceptorOtherDescription,
            interceptorCapacity: helper.toNumberOrUndefined(ticket.interceptorCapacity) ?? 0,
            interceptorCapacityType: ticket.interceptorCapacityType,
            interceptorWasteRemovedAmount: helper.toNumberOrUndefined(ticket.interceptorWasteRemovedAmount) ?? 0,
            interceptorWasteRemovedType: ticket.interceptorWasteRemovedType,
            interceptorWasteRemovedDate: helper.toTextOrUndefined(ticket.interceptorWasteRemovedDate),

            receiverContactName: ticket.receiverContactName,
            receiverWasteDeliveredDate: helper.toTextOrUndefined(ticket.receiverWasteDeliveredDate),
            comments: ticket.comments
        };
    }

    private async loadStates(): Promise<void> {
        this.stateOptions = await this._lookupService.getStatesAsOptions();
    }

    private async loadTicket(): Promise<void> {
        if (!this.id) {
            return;
        }

        try {
            this.isLoading = true;
            this.ticket = await this._tripTicketService.get(this.id);
            this.setDisplayValues(this.ticket);
        } finally {
            this.isLoading = false;
        }
    }

    private async loadRecordLogs(): Promise<void> {
        if (!this.id) {
            return;
        }

        try {
            this.isLoadingRecordLogs = true;
            this.recordLogs = await this._tripTicketService.getLogs(this.id);
            this.recordLogTitle = `Record Log (${this.recordLogs.length})`;
        } finally {
            this.isLoadingRecordLogs = false;
        }
    }

    private setDisplayValues(ticket: FogTripTicket): void {
        const supplier = ticket.waterSupplier;

        this.waterSupplierHeader = supplier?.name ? `Water Supplier - ${supplier.name}` : 'Water Supplier';

        this.waterSupplierCityStateZip = [supplier?.city ? `${supplier.city},` : '', supplier?.state?.code, supplier?.zipCode]
            .filter(part => part)
            .join(' ');
    }
}
