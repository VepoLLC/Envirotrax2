import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CsiInspectionService } from '../../../../shared/services/csi/csi-inspection.service';
import { CsiInspection } from '../../../../shared/models/csi/csi-inspection';
import { CsiInspectionReason, csiInspectionReasonLabels } from '../../../../shared/enums/csi-inspection-reason.enum';

@Component({
    standalone: false,
    templateUrl: './csi-inspection-view.component.html',
    styleUrl: './csi-inspection-view.component.scss'
})
export class CsiInspectionViewComponent implements OnInit {
    public isLoading = true;
    public inspection?: CsiInspection;
    public activeTab: 'main' | 'assemblies' | 'additional' | 'images' = 'main';

    constructor(
        private readonly _route: ActivatedRoute,
        private readonly _inspectionService: CsiInspectionService
    ) {}

    public async ngOnInit(): Promise<void> {
        await this.loadInspection();
    }

    public getReasonLabel(reason?: number): string {
        if (reason == null) {
            return '';
        }
        return csiInspectionReasonLabels[reason as CsiInspectionReason] ?? '';
    }

    private async loadInspection(): Promise<void> {
        const idParam = this._route.snapshot.paramMap.get('id');

        if (!idParam) {
            this.isLoading = false;
            return;
        }

        try {
            this.isLoading = true;
            this.inspection = await this._inspectionService.getProfessionalInspection(Number(idParam));
        } finally {
            this.isLoading = false;
        }
    }
}
