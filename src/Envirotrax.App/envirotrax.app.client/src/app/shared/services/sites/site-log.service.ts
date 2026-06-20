import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { UrlResolverService } from '../helpers/url-resolver.service';
import { QueryHelperService } from '../helpers/query-helper.service';
import { PageInfo } from '../../models/page-info';
import { Query } from '../../models/query';
import { PagedData } from '../../models/paged-data';
import { SiteLog } from '../../models/sites/site-log';
import { SiteLogType } from '../../models/sites/site-log-type.enum';

@Injectable({ providedIn: 'root' })
export class SiteLogService {
    constructor(
        private readonly _http: HttpClient,
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService
    ) {}

    public getAll(siteId: number, pageInfo: PageInfo, query: Query): Promise<PagedData<SiteLog>> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}/logs`);
        return lastValueFrom(this._http.get<PagedData<SiteLog>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public add(siteId: number, log: SiteLog, file: File | null): Promise<SiteLog> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}/logs`);
        return lastValueFrom(this._http.post<SiteLog>(url, this.buildFormData(siteId, log, file)));
    }

    public update(siteId: number, log: SiteLog, file: File | null): Promise<SiteLog> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}/logs/${log.id}`);
        return lastValueFrom(this._http.put<SiteLog>(url, this.buildFormData(siteId, log, file)));
    }

    public delete(siteId: number, id: number): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/sites/${siteId}/logs/${id}`);
        return lastValueFrom(this._http.delete<void>(url));
    }

    private buildFormData(siteId: number, log: SiteLog, file: File | null): FormData {
        const formData = new FormData();
        formData.append('logType', (log.logType ?? SiteLogType.Note).toString());
        if (log.noteText) formData.append('noteText', log.noteText);
        if (log.reviewDate) formData.append('reviewDate', log.reviewDate);
        const assemblyId = log.assemblyId ?? log.assembly?.id;
        if (assemblyId != null) formData.append('assembly.id', assemblyId.toString());
        formData.append('skipFile', (log.skipFile ?? false).toString());
        if (log.fileAttachmentName) formData.append('fileAttachmentName', log.fileAttachmentName);
        if (file) formData.append('file', file);
        return formData;
    }
}
