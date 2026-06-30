import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { FogInspection } from "../../models/fog/fog-inspection";
import { FogInspectionImages } from "../../models/fog/fog-inspection-images";

@Injectable({
    providedIn: 'root'
})
export class ProfessionalFogInspectionService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    public getAll(pageInfo: PageInfo, query: Query, latestOnly: boolean): Promise<PagedData<FogInspection>> {
        const url = this._urlResolver.resolveUrl('/api/professionals/fog/inspections');
        let params = this._queryHelper.buildQuery(pageInfo, query);
        params = params.append('latestOnly', String(latestOnly));

        return lastValueFrom(this._http.get<PagedData<FogInspection>>(url, { params }));
    }

    public getById(id: number): Promise<FogInspection> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/fog/inspections/${id}`);
        return lastValueFrom(this._http.get<FogInspection>(url));
    }

    public submit(inspection: FogInspection, images: FogInspectionImages = {}): Promise<FogInspection> {
        const url = this._urlResolver.resolveUrl('/api/professionals/fog/inspections');
        const formData = buildFogInspectionFormData(inspection);

        if (images.exteriorImage) { formData.append('exteriorImage', images.exteriorImage); }
        if (images.interiorImage) { formData.append('interiorImage', images.interiorImage); }
        if (images.signatureImage) { formData.append('signatureImage', images.signatureImage); }

        return lastValueFrom(this._http.post<FogInspection>(url, formData));
    }
}

function buildFogInspectionFormData(inspection: FogInspection): FormData {
    const fd = new FormData();

    const append = (key: string, val: unknown): void => {
        if (val !== null && val !== undefined) {
            fd.append(key, String(val));
        }
    };

    // Nested reference IDs — dot-notation maps to ASP.NET Core model binding
    if (inspection.site?.id != null) { fd.append('site.id', String(inspection.site.id)); }
    if (inspection.waterSupplier?.id != null) { fd.append('waterSupplier.id', String(inspection.waterSupplier.id)); }
    if (inspection.inspector?.id != null) { fd.append('inspector.id', String(inspection.inspector.id)); }

    append('inspectionDate', inspection.inspectionDate);
    append('facilityType', inspection.facilityType);
    append('reasonForInspection', inspection.reasonForInspection);

    // Interceptor
    append('interceptorType', inspection.interceptorType);
    append('interceptorOtherDescription', inspection.interceptorOtherDescription);
    append('interceptorCapacity', inspection.interceptorCapacity);
    append('interceptorCapacityType', inspection.interceptorCapacityType);
    append('interceptorLocationDescription', inspection.interceptorLocationDescription);
    append('interceptorComments', inspection.interceptorComments);

    // Condition
    append('maintained', inspection.maintained);
    append('accessible', inspection.accessible);
    append('pastOverflow', inspection.pastOverflow);

    // Chamber
    append('inletChamberWettingHeight', inspection.inletChamberWettingHeight);
    append('inletChamberGreaseBlanket', inspection.inletChamberGreaseBlanket);
    append('inletChamberSediments', inspection.inletChamberSediments);
    append('outletChamberWettingHeight', inspection.outletChamberWettingHeight);
    append('outletChamberGreaseBlanket', inspection.outletChamberGreaseBlanket);
    append('outletChamberSediments', inspection.outletChamberSediments);
    append('inletTeeIntact', inspection.inletTeeIntact);
    append('outletTeeIntact', inspection.outletTeeIntact);
    append('inletTeeVisible', inspection.inletTeeVisible);
    append('outletTeeVisible', inspection.outletTeeVisible);

    // Sampling
    append('sampledFrom', inspection.sampledFrom);
    append('samplingPointAccessible', inspection.samplingPointAccessible);
    append('samplingPointClean', inspection.samplingPointClean);

    // Capacity
    append('inletTotalCapacityPercent', inspection.inletTotalCapacityPercent);
    append('outletTotalCapacityPercent', inspection.outletTotalCapacityPercent);
    append('totalCapacityPercent', inspection.totalCapacityPercent);

    append('inspectionResult', inspection.inspectionResult);

    append('signatureContactName', inspection.signatureContactName);

    append('comments', inspection.comments);

    return fd;
}
