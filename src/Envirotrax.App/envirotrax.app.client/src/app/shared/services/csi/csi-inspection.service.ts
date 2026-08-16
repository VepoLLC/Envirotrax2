import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { CsiInspection } from "../../models/csi/csi-inspection";
import { CsiInspectionImage } from "../../models/csi/csi-inspection-image";
import { DownloadEndpoint } from "../../models/download-config";

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

    public async getAll(pageInfo: PageInfo, query: Query, subAccountWaterSupplierId?: number | null): Promise<PagedData<CsiInspection>> {
        const url = this._urlResolver.resolveUrl('/api/csi/inspections');

        let params = this._queryHelper.buildQuery(pageInfo, query);

        if (subAccountWaterSupplierId != null) {
            params = params.append('subAccountWaterSupplierId', String(subAccountWaterSupplierId));
        }

        const observable = this._http.get<PagedData<CsiInspection>>(url, { params });

        return await lastValueFrom(observable);
    }

    public getAllEndpoint(): DownloadEndpoint {
        const url = this._urlResolver.resolveUrl('/api/csi/inspections');

        return {
            method: 'GET',
            url: url
        };
    }

    public getAllPdfEndpoint(): DownloadEndpoint {
        const url = this._urlResolver.resolveUrl('/api/csi/inspections/pdf');

        return {
            method: 'GET',
            url: url
        };
    }

    public get(id: number): Promise<CsiInspection> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${id}`);

        return lastValueFrom(
            this._http.get<CsiInspection>(url)
        );
    }

    public add(inspection: CsiInspection): Promise<CsiInspection> {
        const url = this._urlResolver.resolveUrl('/api/csi/inspections');

        return lastValueFrom(
            this._http.post<CsiInspection>(url, inspection)
        );
    }

    public update(inspection: CsiInspection): Promise<CsiInspection> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${inspection.id}`);

        return lastValueFrom(
            this._http.put<CsiInspection>(url, inspection)
        );
    }

    public delete(id: number): Promise<CsiInspection> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${id}`);

        return lastValueFrom(
            this._http.delete<CsiInspection>(url)
        );
    }

    public getProfessionalInspection(id: number): Promise<CsiInspection> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/csi/inspections/${id}`);
        return lastValueFrom(this._http.get<CsiInspection>(url));
    }

    public submit(inspection: CsiInspection): Promise<CsiInspection> {
        const url = this._urlResolver.resolveUrl('/api/professionals/csi/inspections/submit');
        return lastValueFrom(this._http.post<CsiInspection>(url, inspection));
    }

    public updateApproval(id: number, request: { disapproved: boolean; disapprovedReason?: string | null }): Promise<CsiInspection> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${id}/approval`);
        return lastValueFrom(this._http.put<CsiInspection>(url, request));
    }

    public getPdf(id: number): Promise<Blob> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${id}/pdf`);
        return lastValueFrom(
            this._http.get(url, { responseType: 'blob' })
        );
    }

    public getProfessionalInspections(pageInfo: PageInfo, query: Query, latestOnly: boolean): Promise<PagedData<CsiInspection>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/csi/inspections');
        let params: HttpParams = this._queryHelper.buildQuery(pageInfo, query);
        params = params.append('latestOnly', String(latestOnly));
        return lastValueFrom(this._http.get<PagedData<CsiInspection>>(url, { params }));
    }

    public deleteForProfessional(id: number): Promise<CsiInspection> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/csi/inspections/${id}`);
        return lastValueFrom(this._http.delete<CsiInspection>(url));
    }

    public getPdfForProfessional(id: number): Promise<Blob> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/csi/inspections/${id}/pdf`);
        return lastValueFrom(this._http.get(url, { responseType: 'blob' }));
    }

    public getImages(inspectionId: number): Promise<CsiInspectionImage[]> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${inspectionId}/images`);
        return lastValueFrom(this._http.get<CsiInspectionImage[]>(url));
    }

    public deleteImage(inspectionId: number, imageId: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/${inspectionId}/images/${imageId}`);
        return lastValueFrom(this._http.delete<void>(url));
    }

    public getProfessionalImages(inspectionId: number): Promise<CsiInspectionImage[]> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/csi/inspections/${inspectionId}/images`);
        return lastValueFrom(this._http.get<CsiInspectionImage[]>(url));
    }

    public addImage(inspectionId: number, description: string | null, file: File): Promise<CsiInspectionImage> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/csi/inspections/${inspectionId}/images`);
        const formData = new FormData();
        if (description) formData.append('description', description);
        formData.append('image', file);
        return lastValueFrom(this._http.post<CsiInspectionImage>(url, formData));
    }

    public deleteProfessionalImage(inspectionId: number, imageId: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/csi/inspections/${inspectionId}/images/${imageId}`);
        return lastValueFrom(this._http.delete<void>(url));
    }
}
