import { Component, EventEmitter, Input, Output } from "@angular/core";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";
import { BackflowTestService } from "../../../../shared/services/backflow/backflow-test.service";
import { BackflowDeviceType } from "../../../../shared/models/backflow/backflow-test-enums";
import { ToastService } from '@envirotrax/common-ui';

interface ImageSlot {
    type: string;
    urlKey: keyof BackflowTest;
    label: string;
    alt: string;
}

export interface ImageUrlChange {
    urlKey: keyof BackflowTest;
    value: string | undefined;
}

@Component({
    selector: 'vp-backflow-test-images',
    standalone: false,
    templateUrl: './backflow-test-images.component.html'
})
export class BackflowTestImagesComponent {
    @Input() public test!: BackflowTest;
    @Input() public canModify: boolean = false;

    @Output() public imageUrlUpdated = new EventEmitter<ImageUrlChange>();

    public readonly assemblyImageSlots: ImageSlot[] = [
        { type: 'assembly', urlKey: 'assemblyImageUrl', label: 'Upload an image of the entire assembly', alt: 'Assembly' },
        { type: 'serial-number', urlKey: 'serialNumberImageUrl', label: 'Upload an image of the Serial Number of the assembly', alt: 'Serial number' }
    ];

    public readonly bypassImageSlots: ImageSlot[] = [
        { type: 'bypass-assembly', urlKey: 'bypassAssemblyImageUrl', label: 'Upload an image of the entire Bypass assembly', alt: 'Bypass assembly' },
        { type: 'bypass-serial-number', urlKey: 'bypassSerialNumberImageUrl', label: 'Upload an image of the Serial Number of the Bypass assembly', alt: 'Bypass serial number' }
    ];

    public readonly airGapImageSlots: ImageSlot[] = [
        { type: 'air-gap', urlKey: 'airGapImageUrl', label: 'Upload an image of the air gap', alt: 'Air gap' }
    ];

    public stagedImageFiles: Record<string, File | null> = {};
    public stagedImagePreviews: Record<string, string | null> = {};
    public savingImageTypes: string[] = [];

    private readonly _bypassDeviceTypes: string[] = [BackflowDeviceType.DCD, BackflowDeviceType.DCD2, BackflowDeviceType.RPPD, BackflowDeviceType.RPPD2];

    constructor(
        private readonly _testService: BackflowTestService,
        private readonly _toastService: ToastService
    ) { }

    public get showBypassImages(): boolean {
        return !!this.test?.deviceType && this._bypassDeviceTypes.includes(this.test.deviceType);
    }

    public get showAirGapImages(): boolean {
        return this.test?.deviceType === BackflowDeviceType.AG;
    }

    public imageUrl(slot: ImageSlot): string | null {
        const url = this.test?.[slot.urlKey];
        return typeof url === 'string' ? url : null;
    }

    public onImageFileChange(file: File | null, type: string): void {
        this.clearStagedPreview(type);

        if (!file) {
            this.stagedImageFiles[type] = null;
            return;
        }

        this.stagedImageFiles[type] = file;
        this.stagedImagePreviews[type] = URL.createObjectURL(file);
    }

    public onImageInputChange(event: Event, type: string): void {
        const input = event.target as HTMLInputElement;
        const file = input.files?.[0] ?? null;
        input.value = '';

        if (file) {
            this.onImageFileChange(file, type);
        }
    }

    public removeStagedImage(type: string): void {
        this.clearStagedPreview(type);
        this.stagedImageFiles[type] = null;
    }

    public hasStagedImages(slots: ImageSlot[]): boolean {
        return slots.some(slot => this.stagedImageFiles[slot.type] != null);
    }

    public isSavingImages(slots: ImageSlot[]): boolean {
        return slots.some(slot => this.savingImageTypes.includes(slot.type));
    }

    public async saveImages(slots: ImageSlot[]): Promise<void> {
        if (this.test == null || this.savingImageTypes.length > 0) {
            return;
        }

        const staged = slots.filter(slot => this.stagedImageFiles[slot.type] != null);
        if (staged.length === 0) {
            return;
        }

        try {
            this.savingImageTypes = staged.map(slot => slot.type);

            for (const slot of staged) {
                const file = this.stagedImageFiles[slot.type]!;
                const updated = await this._testService.uploadImage(this.test.id!, slot.type, file);
                this.imageUrlUpdated.emit({ urlKey: slot.urlKey, value: updated[slot.urlKey] as string | undefined });
                this.removeStagedImage(slot.type);
            }

            this._toastService.successfullySaved('Images');
        } catch (e) {
            this._toastService.failedToSave('Images');
            throw e;
        } finally {
            this.savingImageTypes = [];
        }
    }

    private clearStagedPreview(type: string): void {
        const preview = this.stagedImagePreviews[type];
        if (preview) {
            URL.revokeObjectURL(preview);
        }
        this.stagedImagePreviews[type] = null;
    }
}
