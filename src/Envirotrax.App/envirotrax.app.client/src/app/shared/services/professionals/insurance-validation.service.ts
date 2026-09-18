import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { UrlResolverService } from '../helpers/url-resolver.service';
import { InsuranceValidation } from '../../models/professionals/insurance-validation';
import { ProfessionalType } from '../../models/professionals/licenses/professional-user-license';

@Injectable({
    providedIn: 'root'
})
export class InsuranceValidationService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient
    ) {
    }

    // Deliberately uncached, unlike FogSettingsService/BackflowSettingsService: staff can validate a
    // policy mid-session, and a stale "awaiting validation" is exactly the bug this feature fixes.
    public validate(waterSupplierId: number, professionalType: ProfessionalType): Promise<InsuranceValidation> {
        const url = this._urlResolver.resolveUrl('/api/professionals/insurance-status');

        return lastValueFrom(this._http.get<InsuranceValidation>(url, {
            params: { waterSupplierId, professionalType }
        }));
    }
}
