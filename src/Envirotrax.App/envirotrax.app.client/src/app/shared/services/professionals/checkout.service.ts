import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable, shareReplay, switchMap } from "rxjs";
import { PageInfo } from "../../models/page-info";
import { Query, QueryProperty } from "../../models/query";
import { BackflowTestService } from "../backflow/backflow-test.service";
import { CsiInspectionService } from "../csi/csi-inspection.service";
import { ProfessionalFogInspectionService } from "../fog/professional-fog-inspection.service";
import { FogTripTicketService } from "../fog/fog-trip-ticket.service";
import { ProfesionalUserService } from "./professional-user.service";
import { AuthService } from "../auth/auth.service";
import { FeatureType } from "../../models/feature-type";
import { ROLE_DEFINITIONS } from "../../models/role-definitions";

const UNPAID_FILTER: QueryProperty[] = [
    { columnName: 'transactionId', isValueNull: true },
    { columnName: 'amount', comparisonOperator: 'Gt', value: '0' }
];

const COUNT_PAGE_INFO: PageInfo = { pageNumber: 1, pageSize: 1 };

@Injectable({
    providedIn: 'root'
})
export class CheckoutService {
    private readonly _refresh$ = new BehaviorSubject<void>(undefined);

    public readonly cartCount$: Observable<number> = this._refresh$.pipe(
        switchMap(() => this.fetchCartCount()),
        shareReplay(1)
    );

    constructor(
        private readonly _backflowTestService: BackflowTestService,
        private readonly _csiInspectionService: CsiInspectionService,
        private readonly _fogInspectionService: ProfessionalFogInspectionService,
        private readonly _fogTripTicketService: FogTripTicketService,
        private readonly _professionalUserService: ProfesionalUserService,
        private readonly _authService: AuthService
    ) {
    }

    public refresh(): void {
        this._refresh$.next();
    }

    private async fetchCartCount(): Promise<number> {
        const [isAdmin, currentUser, hasBackflowTesting, hasCsiInspection, hasFogInspection, hasFogTransportation] = await Promise.all([
            this._authService.hasAnyRoles(ROLE_DEFINITIONS.PROFESSIONALS.ADMIN),
            this._professionalUserService.getMyData(),
            this._authService.hasAnyFeatures(FeatureType.BackflowTesting),
            this._authService.hasAnyFeatures(FeatureType.CsiInspection),
            this._authService.hasAnyFeatures(FeatureType.FogInspection),
            this._authService.hasAnyFeatures(FeatureType.FogTransportation)
        ]);

        const buildQuery = (ownerColumnName: string): Query => ({
            sort: {},
            filter: isAdmin
                ? [...UNPAID_FILTER]
                : [...UNPAID_FILTER, { columnName: ownerColumnName, comparisonOperator: 'Eq', value: String(currentUser.id) }]
        });

        const counts = await Promise.all([
            hasBackflowTesting
                ? this._backflowTestService.getAllForProfessional(COUNT_PAGE_INFO, buildQuery('bpat.id'))
                : Promise.resolve(null),
            hasCsiInspection
                ? this._csiInspectionService.getProfessionalInspections(COUNT_PAGE_INFO, buildQuery('inspectorUser.id'), false)
                : Promise.resolve(null),
            hasFogInspection
                ? this._fogInspectionService.getAll(COUNT_PAGE_INFO, buildQuery('inspector.id'), false)
                : Promise.resolve(null),
            hasFogTransportation
                ? this._fogTripTicketService.searchForProfessional(COUNT_PAGE_INFO, buildQuery('transporter.id'))
                : Promise.resolve(null)
        ]);

        return counts.reduce((total, result) => total + (result?.pageInfo.totalItems || 0), 0);
    }
}
