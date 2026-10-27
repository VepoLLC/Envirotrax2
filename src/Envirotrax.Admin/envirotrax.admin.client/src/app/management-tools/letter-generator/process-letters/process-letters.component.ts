import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { InputOption, ModalHelperService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
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
export class ProcessLettersComponent implements OnInit {
    public waterSuppliers: WaterSupplierLetterSettingsRow[] = [];
    public visibleWaterSuppliers: WaterSupplierLetterSettingsRow[] = [];

    public selectAll: boolean = true;
    public showAllWaterSuppliers: boolean = false;

    public isLoading: boolean = false;

    public readonly showTypeOptions: InputOption[] = [
        { id: false, text: 'Show only letter generating water suppliers' },
        { id: true, text: 'Show all water suppliers' }
    ];

    constructor(
        private readonly _waterSupplierService: WaterSupplierService,
        private readonly _modalHelper: ModalHelperService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        await this.loadWaterSuppliers();
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

    private toRow(settings: WaterSupplierLetterSettings): WaterSupplierLetterSettingsRow {
        return {
            ...settings,
            isSelected: true,
            hasLettersForGeneration: this.hasLettersForGeneration(settings)
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
