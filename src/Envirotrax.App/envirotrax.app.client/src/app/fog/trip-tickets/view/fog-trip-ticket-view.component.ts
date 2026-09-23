import { Component, DestroyRef, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { FogTripTicketService } from '../../../shared/services/fog/fog-trip-ticket.service';
import { FogTripTicket } from '../../../shared/models/fog/fog-trip-ticket';
import { FOG_VEHICLE_CAPACITY_TYPE_LABELS, FogVehicleCapacityType } from '../../../shared/models/fog/fog-vehicle-enums';
import { PropertyType } from '../../../shared/enums/property-type.enum';
import { DownloadService } from '../../../shared/services/download.service';

@Component({
    standalone: false,
    templateUrl: './fog-trip-ticket-view.component.html'
})
export class FogTripTicketViewComponent implements OnInit {
    public isLoading = true;
    public ticket?: FogTripTicket;

    public interceptorCapacityLabel = '';
    public vehicleCapacityLabel = '';
    public wasteRemovedLabel = '';

    public readonly PropertyType = PropertyType;

    constructor(
        private readonly _destroyRef: DestroyRef,
        private readonly _route: ActivatedRoute,
        private readonly _tripTicketService: FogTripTicketService,
        private readonly _downloadService: DownloadService
    ) {}

    public ngOnInit(): void {
        this._route.paramMap
            .pipe(takeUntilDestroyed(this._destroyRef))
            .subscribe((params: ParamMap) => this.loadTicket(params.get('id')));
    }

    private async loadTicket(idParam: string | null): Promise<void> {
        if (!idParam) {
            this.isLoading = false;
            return;
        }

        try {
            this.isLoading = true;
            this.ticket = await this._tripTicketService.getById(Number(idParam));
            this.setDisplayValues(this.ticket);
        } finally {
            this.isLoading = false;
        }
    }

    public async toggleApproval(): Promise<void> {
        if (!this.ticket) {
            return;
        }

        try {
            this.isLoading = true;
            this.ticket = await this._tripTicketService.updateApproval(this.ticket.id, !this.ticket.disapproved);
            this.setDisplayValues(this.ticket);
        } finally {
            this.isLoading = false;
        }
    }

    public async exportPdf(): Promise<void> {
        if (!this.ticket) {
            return;
        }

        try {
            this.isLoading = true;
            
            const blob = await this._tripTicketService.getPdf(this.ticket.id);
            this._downloadService.downloadFileFromBlob(blob);
        } finally {
            this.isLoading = false;
        }
    }

    private setDisplayValues(ticket: FogTripTicket): void {
        this.interceptorCapacityLabel = this.getCapacityLabel(ticket.interceptorCapacityType);
        this.vehicleCapacityLabel = this.getCapacityLabel(ticket.vehicleCapacityType);
        this.wasteRemovedLabel = this.getCapacityLabel(ticket.interceptorWasteRemovedType);
    }

    private getCapacityLabel(capacityType?: FogVehicleCapacityType): string {
        if (capacityType == null) {
            return '';
        }
        return FOG_VEHICLE_CAPACITY_TYPE_LABELS[capacityType] ?? '';
    }
}
