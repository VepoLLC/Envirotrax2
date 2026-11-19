import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { QueryHelperService } from "../helpers/query-helper.service";
import { PageInfo } from "../../models/page-info";
import { Query } from "../../models/query";
import { PagedData } from "../../models/paged-data";
import { ProfessionalNote } from "../../models/professionals/professional-note";
import { ProfessionalType } from "../../models/professionals/licenses/professional-user-license";

@Injectable({
    providedIn: 'root'
})
export class ProfessionalNoteService {
    private static readonly _professionalUrls: { [type: number]: string } = {
        [ProfessionalType.Bpat]: '/api/backflow/testers',
        [ProfessionalType.CsiInspector]: '/api/csi/inspectors',
        [ProfessionalType.FogInspector]: '/api/fog/inspectors',
        [ProfessionalType.FogTransporter]: '/api/fog/transporters'
    };

    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService,
        private readonly _http: HttpClient
    ) {
    }

    // userId: a user's own comment log instead of the company's.
    public getAll(professionalType: ProfessionalType, professionalId: number, pageInfo: PageInfo, query: Query, userId?: number): Promise<PagedData<ProfessionalNote>> {
        const url = this.resolveNotesUrl(professionalType, professionalId, userId);

        return lastValueFrom(this._http.get<PagedData<ProfessionalNote>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }

    public add(professionalType: ProfessionalType, professionalId: number, note: ProfessionalNote, userId?: number): Promise<ProfessionalNote> {
        const url = this.resolveNotesUrl(professionalType, professionalId, userId);

        return lastValueFrom(this._http.post<ProfessionalNote>(url, note));
    }

    public update(professionalType: ProfessionalType, professionalId: number, note: ProfessionalNote, userId?: number): Promise<ProfessionalNote> {
        const url = this.resolveNotesUrl(professionalType, professionalId, userId, note.id);

        return lastValueFrom(this._http.put<ProfessionalNote>(url, note));
    }

    public delete(professionalType: ProfessionalType, professionalId: number, noteId: number, userId?: number): Promise<void> {
        const url = this.resolveNotesUrl(professionalType, professionalId, userId, noteId);

        return lastValueFrom(this._http.delete<void>(url));
    }

    private resolveNotesUrl(professionalType: ProfessionalType, professionalId: number, userId?: number, noteId?: number): string {
        const professionalUrl = `${ProfessionalNoteService._professionalUrls[professionalType]}/${professionalId}`;
        const notesUrl = userId ? `${professionalUrl}/users/${userId}/notes` : `${professionalUrl}/notes`;

        return this._urlResolver.resolveUrl(noteId ? `${notesUrl}/${noteId}` : notesUrl);
    }
}
