import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CsiInspectionService } from '../../../../shared/services/csi/csi-inspection.service';
import { CsiInspection } from '../../../../shared/models/csi/csi-inspection';
import { CsiInspectionImage } from '../../../../shared/models/csi/csi-inspection-image';
import { CsiInspectionReason, csiInspectionReasonLabels } from '../../../../shared/enums/csi-inspection-reason.enum';
import { ToastService } from '../../../../shared/services/toast.service';
import { ModalHelperService } from '@envirotrax/common-ui';

@Component({
    standalone: false,
    templateUrl: './csi-inspection-view.component.html',
    styleUrl: './csi-inspection-view.component.scss'
})
export class CsiInspectionViewComponent implements OnInit {
    public isLoading = true;
    public inspection?: CsiInspection;
    public activeTab: 'main' | 'assemblies' | 'additional' | 'images' = 'main';
    public images: CsiInspectionImage[] = [];
    public isLoadingImages = false;
    public newImageDescription = '';
    public showAddImageModal = false;
    public modalPreviewUrl: string | null = null;
    public modalSelectedFile: File | null = null;

    private imagesLoaded = false;

    constructor(
        private readonly _route: ActivatedRoute,
        private readonly _inspectionService: CsiInspectionService,
        private readonly _toastService: ToastService,
        private readonly _modalHelper: ModalHelperService
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

    public onTabChange(tab: 'main' | 'assemblies' | 'additional' | 'images'): void {
        this.activeTab = tab;
        if (tab === 'images' && !this.imagesLoaded) {
            this.loadImages();
        }
    }

    private async loadImages(): Promise<void> {
        if (!this.inspection?.id) return;
        try {
            this.isLoadingImages = true;
            this.images = await this._inspectionService.getProfessionalImages(this.inspection.id);
        } finally {
            this.imagesLoaded = true;
            this.isLoadingImages = false;
        }
    }

    public openAddImageModal(): void {
        this.newImageDescription = '';
        this.modalPreviewUrl = null;
        this.modalSelectedFile = null;
        this.showAddImageModal = true;
    }

    public closeAddImageModal(): void {
        this.showAddImageModal = false;
        this.newImageDescription = '';
        this.modalPreviewUrl = null;
        this.modalSelectedFile = null;
    }

    public onModalFileChange(file: File | null): void {
        this.modalSelectedFile = file;
        if (!file) {
            this.modalPreviewUrl = null;
            return;
        }
        const reader = new FileReader();
        reader.onload = (e) => {
            this.modalPreviewUrl = e.target?.result as string;
        };
        reader.readAsDataURL(file);
    }

    public async onModalOk(): Promise<void> {
        if (!this.modalSelectedFile || !this.inspection?.id) return; // no file selected — no-op (matches V1 server-side guard)
        const file = this.modalSelectedFile;
        const description = this.newImageDescription || null;
        this.closeAddImageModal();
        try {
            this.isLoadingImages = true;
            const added = await this._inspectionService.addImage(this.inspection.id, description, file);
            this.images = [...this.images, added];
            this._toastService.successfullySaved('Image');
        } catch {
            this._toastService.failedToSave('Image');
        } finally {
            this.isLoadingImages = false;
        }
    }

    public deleteImage(imageId: number): void {
        const inspectionId = this.inspection?.id;
        if (!inspectionId) return;
        this._modalHelper.confirm({
            messages: ['Are you sure you want to delete this image?']
        }).result().subscribe(async () => {
            try {
                this.isLoadingImages = true;
                await this._inspectionService.deleteProfessionalImage(inspectionId, imageId);
                this.images = this.images.filter(i => i.id !== imageId);
                this._toastService.successfullySaved('Image');
            } catch {
                this._toastService.failedToSave('Image');
            } finally {
                this.isLoadingImages = false;
            }
        });
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
