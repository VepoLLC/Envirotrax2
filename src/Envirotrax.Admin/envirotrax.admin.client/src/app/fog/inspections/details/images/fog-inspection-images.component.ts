import { CommonModule } from '@angular/common';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { FogInspection } from '../../../../shared/models/fog/fog-inspection';
import { FogInspectionService } from '../../../../shared/services/fog/fog-inspection.service';

type FogImageUrlKey = 'exteriorImageUrl' | 'interiorImageUrl';

interface FogImageSlot {
    type: string;
    urlKey: FogImageUrlKey;
    label: string;
    alt: string;
    url: string | null;
    stagedFile: File | null;
    stagedPreview: string | null;
}

const ImageAccept = '.jpg,.jpeg,.png,.gif,.bmp,.tiff';

@Component({
    selector: 'vp-fog-inspection-images',
    templateUrl: './fog-inspection-images.component.html',
    imports: [CommonModule, SharedComponentsModule]
})
export class FogInspectionImagesComponent implements OnInit, OnDestroy {
    @Input() public inspection: FogInspection = {};

    public readonly accept: string = ImageAccept;

    public slots: FogImageSlot[] = [];

    public isSaving: boolean = false;
    public hasStagedImages: boolean = false;

    constructor(
        private readonly _inspectionService: FogInspectionService,
        private readonly _toastService: ToastService
    ) {

    }

    public ngOnInit(): void {
        this.slots = [
            this.buildSlot('exterior', 'exteriorImageUrl', 'Exterior Image', 'Exterior'),
            this.buildSlot('interior', 'interiorImageUrl', 'Interior Image', 'Interior')
        ];
    }

    public ngOnDestroy(): void {
        for (const slot of this.slots) {
            this.clearStagedPreview(slot);
        }
    }

    public onImageFileChange(file: File | null, slot: FogImageSlot): void {
        this.clearStagedPreview(slot);

        slot.stagedFile = file;
        slot.stagedPreview = file ? URL.createObjectURL(file) : null;

        this.refreshStagedState();
    }

    public async saveImages(): Promise<void> {
        const waterSupplierId = this.inspection.waterSupplier?.id;

        if (this.inspection.id == null || waterSupplierId == null || this.isSaving) {
            return;
        }

        const staged = this.slots.filter(slot => slot.stagedFile != null);

        if (staged.length === 0) {
            return;
        }

        try {
            this.isSaving = true;

            for (const slot of staged) {
                const file = slot.stagedFile!;
                const updated = await this._inspectionService.uploadImage(this.inspection.id, waterSupplierId, slot.type, file);

                this.applyUploadedUrls(updated);
                this.removeStagedImage(slot);
            }
        } finally {
            this.isSaving = false;
        }

        this._toastService.successfullySaved('Images');
    }

    private removeStagedImage(slot: FogImageSlot): void {
        this.clearStagedPreview(slot);

        slot.stagedFile = null;

        this.refreshStagedState();
    }

    private applyUploadedUrls(saved: FogInspection): void {
        this.inspection.exteriorImageUrl = saved.exteriorImageUrl;
        this.inspection.interiorImageUrl = saved.interiorImageUrl;

        for (const slot of this.slots) {
            slot.url = this.inspection[slot.urlKey] ?? null;
        }
    }

    private refreshStagedState(): void {
        this.hasStagedImages = this.slots.some(slot => slot.stagedFile != null);
    }

    private clearStagedPreview(slot: FogImageSlot): void {
        if (slot.stagedPreview) {
            URL.revokeObjectURL(slot.stagedPreview);
        }

        slot.stagedPreview = null;
    }

    private buildSlot(type: string, urlKey: FogImageUrlKey, label: string, alt: string): FogImageSlot {
        return {
            type: type,
            urlKey: urlKey,
            label: label,
            alt: alt,
            url: this.inspection[urlKey] ?? null,
            stagedFile: null,
            stagedPreview: null
        };
    }
}
