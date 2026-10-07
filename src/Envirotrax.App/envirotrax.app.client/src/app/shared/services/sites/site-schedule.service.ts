import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { lastValueFrom } from "rxjs";
import { UrlResolverService } from "../helpers/url-resolver.service";
import { AuthService } from "../auth/auth.service";
import { SiteSchedule } from "../../models/sites/site-schedule";
import { ProfessionalType } from "../../models/professionals/licenses/professional-user-license";
import { FeatureType } from "../../models/feature-type";
import { ROLE_DEFINITIONS } from "../../models/role-definitions";

export interface SiteScheduleType {
    professionalType: ProfessionalType;
    name: string;
}

export const SITE_SCHEDULE_TYPE_NAMES: Record<number, string> = {
    [ProfessionalType.Bpat]: 'Backflow Testing',
    [ProfessionalType.CsiInspector]: 'CSI Inspection',
    [ProfessionalType.FogInspector]: 'FOG Inspection',
    [ProfessionalType.FogTransporter]: 'FOG Transportation'
};

@Injectable({
    providedIn: 'root'
})
export class SiteScheduleService {
    constructor(
        private readonly _urlResolver: UrlResolverService,
        private readonly _http: HttpClient,
        private readonly _authService: AuthService
    ) {
    }

    public async getMyScheduleTypes(): Promise<SiteScheduleType[]> {
        const [
            hasBackflowTesting,
            hasCsiInspection,
            hasFogInspection,
            hasFogTransportation,
            isAdmin,
            isBackflowTester,
            isCsiInspector,
            isFogInspector,
            isFogTransporter
        ] = await Promise.all([
            this._authService.hasAnyFeatures(FeatureType.BackflowTesting),
            this._authService.hasAnyFeatures(FeatureType.CsiInspection),
            this._authService.hasAnyFeatures(FeatureType.FogInspection),
            this._authService.hasAnyFeatures(FeatureType.FogTransportation),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.ADMIN),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.BACKFLOW_TESTER),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.CSI_INSPECTOR),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.FOG_INSPECTOR),
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.FOG_TRANSPORTER)
        ]);

        const scheduleTypes: SiteScheduleType[] = [];

        if (hasBackflowTesting && (isBackflowTester || isAdmin)) {
            scheduleTypes.push({ professionalType: ProfessionalType.Bpat, name: SITE_SCHEDULE_TYPE_NAMES[ProfessionalType.Bpat] });
        }

        if (hasCsiInspection && (isCsiInspector || isAdmin)) {
            scheduleTypes.push({ professionalType: ProfessionalType.CsiInspector, name: SITE_SCHEDULE_TYPE_NAMES[ProfessionalType.CsiInspector] });
        }

        if (hasFogInspection && (isFogInspector || isAdmin)) {
            scheduleTypes.push({ professionalType: ProfessionalType.FogInspector, name: SITE_SCHEDULE_TYPE_NAMES[ProfessionalType.FogInspector] });
        }

        if (hasFogTransportation && (isFogTransporter || isAdmin)) {
            scheduleTypes.push({ professionalType: ProfessionalType.FogTransporter, name: SITE_SCHEDULE_TYPE_NAMES[ProfessionalType.FogTransporter] });
        }

        return scheduleTypes;
    }

    public getMySchedules(siteId: number): Promise<SiteSchedule[]> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/sites/${siteId}/schedule`);

        return lastValueFrom(
            this._http.get<SiteSchedule[]>(url)
        );
    }

    public set(siteId: number, schedule: SiteSchedule): Promise<SiteSchedule> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/sites/${siteId}/schedule`);

        return lastValueFrom(
            this._http.put<SiteSchedule>(url, schedule)
        );
    }

    public clear(siteId: number, professionalType: ProfessionalType): Promise<void> {
        const url = this._urlResolver.resolveUrl(`/api/professionals/sites/${siteId}/schedule`);
        const params = new HttpParams().set('professionalType', String(professionalType));

        return lastValueFrom(
            this._http.delete<void>(url, { params })
        );
    }
}
