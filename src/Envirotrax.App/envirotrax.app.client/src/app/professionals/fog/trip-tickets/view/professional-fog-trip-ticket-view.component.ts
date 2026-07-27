import { Component, DestroyRef, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { FogTripTicketService } from '../../../../shared/services/fog/fog-trip-ticket.service';
import { FogTripTicket } from '../../../../shared/models/fog/fog-trip-ticket';
import { FOG_VEHICLE_CAPACITY_TYPE_LABELS, FogVehicleCapacityType } from '../../../../shared/models/fog/fog-vehicle-enums';
import { PropertyType } from '../../../../shared/enums/property-type.enum';

@Component({
    standalone: false,
    templateUrl: './professional-fog-trip-ticket-view.component.html'
})
export class ProfessionalFogTripTicketViewComponent implements OnInit {
    public isLoading = true;
    public ticket?: FogTripTicket;

    public interceptorCapacityLabel = '';
    public vehicleCapacityLabel = '';
    public wasteRemovedLabel = '';

    public readonly PropertyType = PropertyType;

    constructor(
        private readonly _destroyRef: DestroyRef,
        private readonly _route: ActivatedRoute,
        private readonly _tripTicketService: FogTripTicketService
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
            this.ticket = await this._tripTicketService.getByIdForProfessional(Number(idParam));
            this.setDisplayValues(this.ticket);
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
