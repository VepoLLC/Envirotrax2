import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { CsiInspectorDetails } from "../../../shared/models/csi/csi-inspector-details";
import { CsiInspectoreManagementService } from "../../../shared/services/csi/csi-inspector-management.service";
import { ExpirationType, ProfessionalUserLicense, professionalTypeLabels, ProfessionalType } from "../../../shared/models/professionals/licenses/professional-user-license";

@Component({
    selector: 'app-csi-inspector-details',
    standalone: false,
    templateUrl: './csi-inspector-details.component.html'
})
export class CsiInspectorDetailsComponent implements OnInit {
    public id: number | null = null;
    public details: CsiInspectorDetails | null = null;
    public isLoading: boolean = false;

    public expirationType = ExpirationType;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _csiInspectorService: CsiInspectoreManagementService
    ) {}

    public async ngOnInit(): Promise<void> {
        this.setIdFromRoute();
        if (this.id !== null) {
            await this.loadDetails();
        }
    }

    private setIdFromRoute(): void {
        const idParam = this._activatedRoute.snapshot.paramMap.get('id');
        this.id = idParam ? Number(idParam) : null;
    }

    private async loadDetails(): Promise<void> {
        try {
            this.isLoading = true;

            if(this.id === null){
                return;
            }
            
            this.details = await this._csiInspectorService.getDetails(this.id);
        } finally {
            this.isLoading = false;
        }
    }

    public goBack(): void {
        this._router.navigate(['/csi/inspectors']);
    }

    public getProfessionalTypeLabel(type?: number): string {
        if (type === undefined || type === null){
            return '';
        }
        return professionalTypeLabels[type as ProfessionalType] ?? '';
    }

    public getFormattedAddress(): string {
        if (!this.details){
            return '';
        }
        const parts = [
            this.details.address,
            this.details.city,
            this.details.state?.name,
            this.details.zipCode
        ].filter(p => p);
        return parts.join(', ');
    }

    public getLicenseExpirationClass(license: ProfessionalUserLicense): string {
        if (!license.expirationDate){
            return 'text-bg-primary';
        }
        if (license.expirationType === ExpirationType.Expired){
            return 'text-bg-danger';
        }
        if (license.expirationType === ExpirationType.AboutToExpire){
            return 'text-bg-warning';
        }
        return 'text-bg-success';
    }
}
