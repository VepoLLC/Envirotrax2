import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { CsiInspection } from "../../../shared/models/csi/csi-inspection";
import { CsiInspectionService } from "../../../shared/services/csi/csi-inspection.service";
import { ModalHelperService } from "../../../shared/services/helpers/modal-helper.service";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { DisapproveCsiInspectionComponent } from "./disapprove/disapprove-csi-inspection.component";

@Component({
    selector: 'app-csi-inspection-details',
    standalone: false,
    templateUrl: './csi-inspection-details.component.html',
    styleUrl: './csi-inspection-details.component.scss'
})
export class CsiInspectionDetailsComponent implements OnInit {
    public id: number = 0;
    public inspection: CsiInspection | null = null;
    public isLoading: boolean = false;
    public selectedTab: string = 'main';

    private readonly _reasonForInspectionLabels: Record<number, string> = {
        0: 'New construction',
        1: 'Existing service where contaminant hazards are suspected',
        2: 'Major renovation or expansion of distribution facilities'
    };

    private readonly _propertyTypeLabels: Record<number, string> = {
        0: 'Residential',
        1: 'Commercial'
    };

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _inspectionService: CsiInspectionService,
        private readonly _modalHelper: ModalHelperService
    ) {}

    public async ngOnInit(): Promise<void> {
        this.id = Number(this._activatedRoute.snapshot.paramMap.get('id'));
        await this.loadInspection();
    }

    private async loadInspection(): Promise<void> {
        try {
            this.isLoading = true;
            this.inspection = await this._inspectionService.get(this.id);
        } finally {
            this.isLoading = false;
        }
    }

    public toggleApproval(): void {
        if (this.inspection == null) return;

        if (this.inspection.disapproved) {
            this.approve();
        } else {
            this._modalHelper.show<CsiInspection>(DisapproveCsiInspectionComponent, {
                title: 'Disapprove CSI Certificate',
                size: ModalSize.large,
                model: this.inspection
            }).result().subscribe(updated => {
                this.inspection = updated;
            });
        }
    }

    private async approve(): Promise<void> {
        try {
            this.isLoading = true;
            this.inspection = await this._inspectionService.updateApproval(this.inspection!.id!, { disapproved: false });
        } finally {
            this.isLoading = false;
        }
    }

    public getReasonForInspectionLabel(): string {
        if (this.inspection?.reasonForInspection == null) return '';
        return this._reasonForInspectionLabels[this.inspection.reasonForInspection] ?? '';
    }

    public getPropertyTypeLabel(): string {
        if (this.inspection?.propertyType == null) return '';
        return this._propertyTypeLabels[this.inspection.propertyType] ?? '';
    }

    public exportPdf(): void {
        //generate PDf logic
    }
}
