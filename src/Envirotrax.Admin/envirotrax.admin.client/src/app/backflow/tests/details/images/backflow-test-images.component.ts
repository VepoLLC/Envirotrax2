import { CommonModule } from '@angular/common';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { BackflowTestDetails } from '../../../../shared/models/backflow/backflow-test';
import { BackflowTestService } from '../../../../shared/services/backflow/backflow-test.service';
import { BackflowAssemblyVisibility, buildAssemblyVisibility } from '../backflow-test-visibility';

type BackflowImageUrlKey = 'assemblyImageUrl'
    | 'serialNumberImageUrl'
    | 'bypassAssemblyImageUrl'
    | 'bypassSerialNumberImageUrl'
    | 'airGapImageUrl';

interface BackflowImageSlot {
    type: string;
    urlKey: BackflowImageUrlKey;
    label: string;
    alt: string;
    url: string | null;
    stagedFile: File | null;
    stagedPreview: string | null;
}

interface BackflowImageGroup {
    header: string;
    slots: BackflowImageSlot[];
    isSaving: boolean;
    hasStagedImages: boolean;
}

const ImageAccept = '.jpg,.jpeg,.png,.gif,.bmp,.tiff';

@Component({
    selector: 'vp-backflow-test-images',
    templateUrl: './backflow-test-images.component.html',
    imports: [CommonModule, SharedComponentsModule]
})
export class BackflowTestImagesComponent implements OnInit, OnDestroy {
    @Input() public test: BackflowTestDetails = {};
    @Input() public visibility: BackflowAssemblyVisibility = buildAssemblyVisibility(undefined);

    public readonly accept: string = ImageAccept;

    public groups: BackflowImageGroup[] = [];

    constructor(
        private readonly _testService: BackflowTestService,
        private readonly _toastService: ToastService
    ) {

    }

    public ngOnInit(): void {
        this.groups = this.buildGroups();
    }

    public ngOnDestroy(): void {
        for (const group of this.groups) {
            for (const slot of group.slots) {
                this.clearStagedPreview(slot);
            }
        }
    }

    public onImageFileChange(file: File | null, group: BackflowImageGroup, slot: BackflowImageSlot): void {
        this.clearStagedPreview(slot);

        slot.stagedFile = file;
        slot.stagedPreview = file ? URL.createObjectURL(file) : null;

        this.refreshStagedState(group);
    }

    private removeStagedImage(group: BackflowImageGroup, slot: BackflowImageSlot): void {
        this.clearStagedPreview(slot);

        slot.stagedFile = null;

        this.refreshStagedState(group);
    }

    public async saveImages(group: BackflowImageGroup): Promise<void> {
        if (this.test.id == null || group.isSaving) {
            return;
        }

        const staged = group.slots.filter(slot => slot.stagedFile != null);

        if (staged.length === 0) {
            return;
        }

        try {
            group.isSaving = true;

            for (const slot of staged) {
                const file = slot.stagedFile!;
                const updated = await this._testService.uploadImage(this.test.id, slot.type, file);

                this.applyUploadedUrls(updated);
                this.removeStagedImage(group, slot);
            }
        } finally {
            group.isSaving = false;
        }

        this._toastService.successfullySaved('Images');
    }

    private applyUploadedUrls(saved: BackflowTestDetails): void {
        this.test.assemblyImageUrl = saved.assemblyImageUrl;
        this.test.serialNumberImageUrl = saved.serialNumberImageUrl;
        this.test.bypassAssemblyImageUrl = saved.bypassAssemblyImageUrl;
        this.test.bypassSerialNumberImageUrl = saved.bypassSerialNumberImageUrl;
        this.test.airGapImageUrl = saved.airGapImageUrl;

        for (const group of this.groups) {
            for (const slot of group.slots) {
                slot.url = this.test[slot.urlKey] ?? null;
            }
        }
    }

    private refreshStagedState(group: BackflowImageGroup): void {
        group.hasStagedImages = group.slots.some(slot => slot.stagedFile != null);
    }

    private clearStagedPreview(slot: BackflowImageSlot): void {
        if (slot.stagedPreview) {
            URL.revokeObjectURL(slot.stagedPreview);
        }

        slot.stagedPreview = null;
    }

    private buildGroups(): BackflowImageGroup[] {
        if (this.visibility.showAirGap) {
            return [
                this.buildGroup('Air Gap', [
                    this.buildSlot('air-gap', 'airGapImageUrl', 'Air Gap', 'Air gap')
                ])
            ];
        }

        const groups: BackflowImageGroup[] = [
            this.buildGroup('Main Assembly', [
                this.buildSlot('assembly', 'assemblyImageUrl', 'Exterior of Assembly', 'Assembly'),
                this.buildSlot('serial-number', 'serialNumberImageUrl', 'Serial Number', 'Serial number')
            ])
        ];

        if (this.visibility.hasBypassAssembly) {
            groups.push(this.buildGroup('Bypass Assembly', [
                this.buildSlot('bypass-assembly', 'bypassAssemblyImageUrl', 'Exterior of Assembly', 'Bypass assembly'),
                this.buildSlot('bypass-serial-number', 'bypassSerialNumberImageUrl', 'Serial Number', 'Bypass serial number')
            ]));
        }

        return groups;
    }

    private buildGroup(header: string, slots: BackflowImageSlot[]): BackflowImageGroup {
        return {
            header: header,
            slots: slots,
            isSaving: false,
            hasStagedImages: false
        };
    }

    private buildSlot(type: string, urlKey: BackflowImageUrlKey, label: string, alt: string): BackflowImageSlot {
        return {
            type: type,
            urlKey: urlKey,
            label: label,
            alt: alt,
            url: this.test[urlKey] ?? null,
            stagedFile: null,
            stagedPreview: null
        };
    }
}
