import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { DownloadService, InputOption, ModalHelperService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { LetterCounts } from '../../../shared/models/water-suppliers/generated-letters';
import { WaterSupplierLetterSettings, WaterSupplierLetterSettingsRow } from '../../../shared/models/water-suppliers/water-supplier-letter-settings';
import { WaterSupplierDetails } from '../../../shared/models/water-suppliers/water-supplier-details';
import { WaterSupplierService } from '../../../shared/services/water-suppliers/water-supplier.service';
import { LetterSettingsComponent, LetterSettingsModalData } from '../settings/letter-settings.component';

@Component({
    selector: 'vp-process-letters',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
    templateUrl: './process-letters.component.html'
})
export class ProcessLettersComponent implements OnInit, OnDestroy {
    public waterSuppliers: WaterSupplierLetterSettingsRow[] = [];
    public visibleWaterSuppliers: WaterSupplierLetterSettingsRow[] = [];

    public selectAll: boolean = true;
    public showAllWaterSuppliers: boolean = false;

    public isLoading: boolean = false;

    public isGenerating: boolean = false;
    public isCancelRequested: boolean = false;
    public currentWaterSupplierId?: number;
    public totalTime: string = '00:00';
    public status: string = '';
    public emailStatus: string = '';

    private _timerId?: number;

    public readonly showTypeOptions: InputOption[] = [
        { id: false, text: 'Show only letter generating water suppliers' },
        { id: true, text: 'Show all water suppliers' }
    ];

    constructor(
        private readonly _waterSupplierService: WaterSupplierService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _downloadService: DownloadService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        await this.loadWaterSuppliers();
    }

    public ngOnDestroy(): void {
        this.isCancelRequested = true;
        this.stopTimer();
    }

    public async loadWaterSuppliers(): Promise<void> {
        try {
            this.isLoading = true;

            const letterSettings = await this._waterSupplierService.getAllLetterSettings();

            this.waterSuppliers = letterSettings.map(settings => this.toRow(settings));
            this.selectAll = true;

            this.applyShowType();
        } finally {
            this.isLoading = false;
        }
    }

    public applyShowType(): void {
        this.visibleWaterSuppliers = this.waterSuppliers.filter(waterSupplier => this.showAllWaterSuppliers || waterSupplier.hasLettersForGeneration);
    }

    public onSelectAllChange(): void {
        for (const waterSupplier of this.visibleWaterSuppliers) {
            waterSupplier.isSelected = this.selectAll;
        }
    }

    public showSettings(waterSupplier: WaterSupplierLetterSettingsRow): void {
        this._modalHelper.show<LetterSettingsModalData, WaterSupplierDetails>(LetterSettingsComponent, {
            title: `${waterSupplier.name} Settings`,
            model: { id: waterSupplier.id! },
            size: ModalSize.extraLarge,
            mode: 'disableFullScreen'
        }).result().subscribe(() => this.loadWaterSuppliers());
    }

    public async savePrintSettings(waterSupplier: WaterSupplierLetterSettingsRow): Promise<void> {
        try {
            this.isLoading = true;

            const details = await this._waterSupplierService.getDetails(waterSupplier.id!);

            details.backflowSettings.nonCompliantPrint1 = !!waterSupplier.backflowNonCompliantPrint1;
            details.backflowSettings.nonCompliantPrint2 = !!waterSupplier.backflowNonCompliantPrint2;
            details.csiSettings.nonCompliantPrint1 = !!waterSupplier.csiNonCompliantPrint1;
            details.csiSettings.nonCompliantPrint2 = !!waterSupplier.csiNonCompliantPrint2;

            await this._waterSupplierService.updateDetails(waterSupplier.id!, details);
        } catch (error) {
            await this.loadWaterSuppliers();

            throw error;
        } finally {
            this.isLoading = false;
        }
    }

    public async generateLetters(): Promise<void> {
        const waterSuppliersToProcess: WaterSupplierLetterSettingsRow[] = [];

        for (const waterSupplier of this.visibleWaterSuppliers) {
            waterSupplier.letterCounts = new LetterCounts();

            if (waterSupplier.isSelected && waterSupplier.hasLettersForGeneration) {
                waterSuppliersToProcess.push(waterSupplier);
            }
        }

        let letterCount = 0;
        let emailCount = 0;

        try {
            this.isGenerating = true;
            this.isCancelRequested = false;
            this.emailStatus = '';
            this.startTimer();

            for (let index = 0; index < waterSuppliersToProcess.length; index++) {
                if (this.isCancelRequested) {
                    break;
                }

                const waterSupplier = waterSuppliersToProcess[index];

                this.currentWaterSupplierId = waterSupplier.id;
                this.status = `Generating water supplier letters (${index + 1}/${waterSuppliersToProcess.length})`;

                const letters = await this._waterSupplierService.generateLetters(waterSupplier.id!);

                waterSupplier.letterCounts = letters.counts;
                letterCount += this.getTotalLetterCount(letters.counts);
                emailCount += letters.emailCount;

                if (letters.fileName && letters.fileContent) {
                    this._downloadService.downloadFileFromBase64(letters.fileContent, 'application/zip', letters.fileName);
                }
            }
        } finally {
            this.status = `Total Letters: ${letterCount}`;
            this.emailStatus = `Total Sent by Email: ${emailCount}`;

            this.isGenerating = false;
            this.currentWaterSupplierId = undefined;
            this.stopTimer();
        }
    }

    public cancelGeneration(): void {
        this.isCancelRequested = true;
    }

    private startTimer(): void {
        const startTime = Date.now();

        this.totalTime = '00:00';

        this._timerId = window.setInterval(() => {
            const elapsedSeconds = Math.floor((Date.now() - startTime) / 1000);
            const minutes = Math.floor(elapsedSeconds / 60);
            const seconds = elapsedSeconds % 60;

            this.totalTime = `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
        }, 1000);
    }

    private stopTimer(): void {
        if (this._timerId !== undefined) {
            window.clearInterval(this._timerId);
            this._timerId = undefined;
        }
    }

    private getTotalLetterCount(counts: LetterCounts): number {
        return counts.expiringNotice1
            + counts.expiringNotice2
            + counts.expiredNotice1
            + counts.expiredNotice2
            + counts.backflowNonCompliant1
            + counts.backflowNonCompliant2
            + counts.unknownAssembliesNotice
            + counts.impendingNotice1
            + counts.impendingNotice2
            + counts.pastDueNotice1
            + counts.pastDueNotice2
            + counts.csiNonCompliant1
            + counts.csiNonCompliant2;
    }

    private toRow(settings: WaterSupplierLetterSettings): WaterSupplierLetterSettingsRow {
        return {
            ...settings,
            isSelected: true,
            hasLettersForGeneration: this.hasLettersForGeneration(settings),
            letterCounts: new LetterCounts()
        };
    }

    private hasLettersForGeneration(settings: WaterSupplierLetterSettings): boolean {
        return !!settings.expiringNotice1
            || !!settings.expiringNotice2
            || !!settings.expiredNotice1
            || !!settings.expiredNotice2
            || !!settings.backflowNonCompliant1
            || !!settings.backflowNonCompliant2
            || !!settings.unknownAssembliesNotice
            || !!settings.impendingNotice1
            || !!settings.impendingNotice2
            || !!settings.pastDueNotice1
            || !!settings.pastDueNotice2
            || !!settings.csiNonCompliant1
            || !!settings.csiNonCompliant2;
    }
}
