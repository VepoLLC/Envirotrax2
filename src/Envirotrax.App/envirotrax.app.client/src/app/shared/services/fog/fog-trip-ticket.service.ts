import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { FogTripTicket } from "../../models/fog/fog-trip-ticket";
import { FogTripTicketImages } from "../../models/fog/fog-trip-ticket-images";

@Injectable({
    providedIn: 'root'
})
export class FogTripTicketService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public async getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<FogTripTicket>> {
        const url = this._urlResolver.resolveUrl('/api/fog/trip-tickets');

        const observable = this._http.get<PagedData<FogTripTicket>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        });

        return await lastValueFrom(observable);
    }

    public getById(id: number): Promise<FogTripTicket> {
        const url = this._urlResolver.resolveUrl(`/api/fog/trip-tickets/${id}`);
        return lastValueFrom(this._http.get<FogTripTicket>(url));
    }

    public async searchForProfessional(pageInfo: PageInfo, query: Query, waterSupplierId?: number): Promise<PagedData<FogTripTicket>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/fog/trip-tickets');
        let params = this._queryHelper.buildQuery(pageInfo, query);

        if (waterSupplierId != null) {
            params = params.append('waterSupplierId', String(waterSupplierId));
        }

        return await lastValueFrom(this._http.get<PagedData<FogTripTicket>>(url, { params }));
    }

    public submit(ticket: FogTripTicket, images: FogTripTicketImages = {}): Promise<FogTripTicket> {
        const url = this._urlResolver.resolveUrl('/api/professionals/fog/trip-tickets');
        const formData = buildFogTripTicketFormData(ticket);

        if (images.generatorSignature) { formData.append('generatorSignature', images.generatorSignature); }
        if (images.transporterSignature) { formData.append('transporterSignature', images.transporterSignature); }
        if (images.receiverSignature) { formData.append('receiverSignature', images.receiverSignature); }

        return lastValueFrom(this._http.post<FogTripTicket>(url, formData));
    }
}

function buildFogTripTicketFormData(ticket: FogTripTicket): FormData {
    const fd = new FormData();

    const append = (key: string, val: unknown): void => {
        if (val !== null && val !== undefined) {
            fd.append(key, String(val));
        }
    };

    // Nested reference IDs — dot-notation maps to ASP.NET Core model binding
    if (ticket.site?.id != null) { fd.append('site.id', String(ticket.site.id)); }
    if (ticket.waterSupplier?.id != null) { fd.append('waterSupplier.id', String(ticket.waterSupplier.id)); }
    if (ticket.transporter?.id != null) { fd.append('transporter.id', String(ticket.transporter.id)); }

    append('vehicleId', ticket.vehicleId);
    append('receiverDisposalSiteId', ticket.receiverDisposalSiteId);

    // Generator
    append('fogGeneratorContactName', ticket.fogGeneratorContactName);
    append('fogGeneratorPhoneNumber', ticket.fogGeneratorPhoneNumber);
    append('fogGeneratorEmailAddress', ticket.fogGeneratorEmailAddress);
    append('generatorContactName', ticket.generatorContactName);

    // Transporter registration snapshot (computed during verification)
    append('transporterLicenseNumber', ticket.transporterLicenseNumber);
    append('transporterLicenseExpiration', ticket.transporterLicenseExpiration);

    // Interceptor / waste
    append('interceptorType', ticket.interceptorType);
    append('interceptorOtherDescription', ticket.interceptorOtherDescription);
    append('interceptorCapacity', ticket.interceptorCapacity);
    append('interceptorCapacityType', ticket.interceptorCapacityType);
    append('interceptorWasteRemovedAmount', ticket.interceptorWasteRemovedAmount);
    append('interceptorWasteRemovedType', ticket.interceptorWasteRemovedType);
    append('interceptorWasteRemovedDate', ticket.interceptorWasteRemovedDate);

    // Receiver
    append('receiverContactName', ticket.receiverContactName);
    append('receiverWasteDeliveredDate', ticket.receiverWasteDeliveredDate);

    append('comments', ticket.comments);

    return fd;
}
