import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { UrlResolverService } from '../helpers/url-resolver.service';
import { CsiInspection } from '../../models/csi/csi-inspection';
import {
    CsiSubmissionCreateViewModel,
    CsiSubmissionSaveRequest
} from '../../models/csi/csi-submission-create-view-model';

@Injectable({
    providedIn: 'root'
})
export class CsiSubmissionService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {}

    public getCreateViewModel(siteId: number): Promise<CsiSubmissionCreateViewModel> {
        const url = this._urlResolver.resolveUrl(`/api/csi/inspections/submission/create/${siteId}`);
        return lastValueFrom(this._http.get<CsiSubmissionCreateViewModel>(url));
    }

    public submit(request: CsiSubmissionSaveRequest): Promise<CsiInspection> {
        const url = this._urlResolver.resolveUrl('/api/csi/inspections/submission');
        return lastValueFrom(this._http.post<CsiInspection>(url, request));
    }
}
