import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CsiInspectionService } from '../../../../shared/services/csi/csi-inspection.service';
import { ProfessionalSupplierService } from '../../../../shared/services/professionals/professional-supplier.service';
import { CsiInspection } from '../../../../shared/models/csi/csi-inspection';
import { ProfessionalWaterSupplier } from '../../../../shared/models/professionals/professional-water-supplier';
import { CsiInspectionReason, csiInspectionReasonLabels } from '../../../../shared/enums/csi-inspection-reason.enum';

@Component({
    standalone: false,
    templateUrl: './csi-inspection-view.component.html',
    styleUrl: './csi-inspection-view.component.scss'
})
export class CsiInspectionViewComponent implements OnInit {
    public isLoading = true;
    public inspection?: CsiInspection;
    public selectedWaterSupplier?: ProfessionalWaterSupplier;
    public activeTab: 'main' | 'assemblies' | 'additional' | 'images' = 'main';

    constructor(
        private readonly _route: ActivatedRoute,
        private readonly _inspectionService: CsiInspectionService,
        private readonly _professionalSupplierService: ProfessionalSupplierService
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

            const [inspection, waterSuppliersPage] = await Promise.all([
                this._inspectionService.getProfessionalInspection(Number(idParam)),
                this._professionalSupplierService.getAllMy(true)
            ]);

            this.inspection = inspection;

            const waterSuppliers = waterSuppliersPage.data ?? [];
            this.selectedWaterSupplier = waterSuppliers.find(ws => ws.waterSupplier?.id === inspection.waterSupplier?.id);
        } finally {
            this.isLoading = false;
        }
    }
}
