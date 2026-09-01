import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { CsiInspectionImage } from '../../../../shared/models/csi/csi-inspection';
import { CsiInspectionService } from '../../../../shared/services/csi/csi-inspection.service';

@Component({
    selector: 'vp-csi-inspection-images',
    templateUrl: './csi-inspection-images.component.html',
    imports: [CommonModule, SharedComponentsModule]
})
export class CsiInspectionImagesComponent implements OnInit {
    @Input() public inspectionId: number = 0;
    @Input() public waterSupplierId: number = 0;

    public isLoading: boolean = false;

    public images: CsiInspectionImage[] = [];

    constructor(private readonly _inspectionService: CsiInspectionService) {

    }

    public async ngOnInit(): Promise<void> {
        await this.load();
    }

    public async onFileChange(file: File | null): Promise<void> {
        if (file == null) {
            return;
        }

        try {
            this.isLoading = true;

            const image = await this._inspectionService.addImage(this.inspectionId, this.waterSupplierId, file, '');

            this.images = [...this.images, image];
        } finally {
            this.isLoading = false;
        }
    }

    public async deleteImage(image: CsiInspectionImage): Promise<void> {
        if (image.id == null) {
            return;
        }

        try {
            this.isLoading = true;

            await this._inspectionService.deleteImage(this.inspectionId, image.id);

            this.images = this.images.filter(existing => existing.id !== image.id);
        } finally {
            this.isLoading = false;
        }
    }

    private async load(): Promise<void> {
        try {
            this.isLoading = true;
            this.images = await this._inspectionService.getImages(this.inspectionId);
        } finally {
            this.isLoading = false;
        }
    }
}
