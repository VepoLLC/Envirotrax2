import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { ProfessionalUser } from "../../models/professionals/professional-user";
import { ProfessionalType } from "../../models/professionals/licenses/professional-user-license";

@Injectable({
    providedIn: 'root'
})
export class ProfessionalPaymentBypassService {
    private static readonly _professionalUrls: { [type: number]: string } = {
        [ProfessionalType.Bpat]: '/api/backflow/testers',
        [ProfessionalType.CsiInspector]: '/api/csi/inspectors',
        [ProfessionalType.FogInspector]: '/api/fog/inspectors',
        [ProfessionalType.FogTransporter]: '/api/fog/transporters'
    };

    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    public update(professionalType: ProfessionalType, professionalId: number, userId: number, isPaymentBypassed: boolean): Promise<ProfessionalUser> {
        const professionalUrl = ProfessionalPaymentBypassService._professionalUrls[professionalType];
        const url = this._urlResolver.resolveUrl(`${professionalUrl}/${professionalId}/users/${userId}/payment-bypass`);

        return lastValueFrom(this._http.put<ProfessionalUser>(url, { isPaymentBypassed }));
    }
}
