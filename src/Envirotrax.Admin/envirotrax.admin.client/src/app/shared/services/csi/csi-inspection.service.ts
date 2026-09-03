import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { PagedData, PageInfo, Query, QueryHelperService, UrlResolverService } from "@envirotrax/common-ui";
import {
    CsiInspection,
    CsiInspectionAssembly,
    CsiInspectionCounts,
    CsiInspectionDetails,
    CsiInspectionImage,
    CsiPaymentStatus
} from "../../models/csi/csi-inspection";
import { RecordLog } from "../../models/logs/record-log";
import { lastValueFrom } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class CsiInspectionService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {

    }

    public async getAll(
        pageInfo: PageInfo,
        query: Query,
        paymentStatus?: CsiPaymentStatus | null
    ): Promise<PagedData<CsiInspection>> {
        const url = this._urlResolver.resolveUrl('/api/csi/inspections');

        let params = this._queryHelper.buildQuery(pageInfo, query);

        if (paymentStatus != null) {
            params = params.append('paymentStatus', String(paymentStatus));
        }

        const observable = this._http.get<PagedData<CsiInspection>>(url, { params });

        return await lastValueFrom(observable);
    }

    public async get(id: number): Promise<CsiInspectionDetails> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${id}`);

        const observable = this._http.get<CsiInspectionDetails>(url);

        return await lastValueFrom(observable);
    }

    public async update(id: number, waterSupplierId: number, inspection: CsiInspectionDetails): Promise<CsiInspectionDetails> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${id}?waterSupplierId=${waterSupplierId}`);

        const observable = this._http.put<CsiInspectionDetails>(url, inspection);

        return await lastValueFrom(observable);
    }

    public async getCounts(id: number): Promise<CsiInspectionCounts> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${id}/counts`);

        const observable = this._http.get<CsiInspectionCounts>(url);

        return await lastValueFrom(observable);
    }

    public async getAssemblies(id: number): Promise<CsiInspectionAssembly[]> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${id}/assemblies`);

        const observable = this._http.get<CsiInspectionAssembly[]>(url);

        return await lastValueFrom(observable);
    }

    public async getLogs(id: number): Promise<RecordLog[]> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${id}/logs`);

        const observable = this._http.get<RecordLog[]>(url);

        return await lastValueFrom(observable);
    }

    public async getImages(id: number): Promise<CsiInspectionImage[]> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${id}/images`);

        const observable = this._http.get<CsiInspectionImage[]>(url);

        return await lastValueFrom(observable);
    }

    public async addImage(id: number, waterSupplierId: number, file: File, description: string): Promise<CsiInspectionImage> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${id}/images?waterSupplierId=${waterSupplierId}`);

        const formData = new FormData();
        formData.append('image', file, file.name);
        formData.append('description', description);

        const observable = this._http.post<CsiInspectionImage>(url, formData);

        return await lastValueFrom(observable);
    }

    public async deleteImage(id: number, imageId: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${id}/images/${imageId}`);

        const observable = this._http.delete<void>(url);

        await lastValueFrom(observable);
    }
}
