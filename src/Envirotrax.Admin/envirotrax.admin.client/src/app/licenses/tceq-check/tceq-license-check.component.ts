import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalReference } from '@developer-partners/ngx-modal-dialog';
import { SharedComponentsModule } from '../../shared/components/shared.components.module';
import { TceqLicense } from '../../shared/models/professionals/licenses/tceq-license';
import { WaterSupplierLicense } from '../../shared/models/professionals/licenses/water-supplier-license';

export interface TceqLicenseCheckModalData {
    license: WaterSupplierLicense;
    tceqLicense: TceqLicense;
}

@Component({
    templateUrl: './tceq-license-check.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule
    ],
})
export class TceqLicenseCheckComponent {
    public license: WaterSupplierLicense;
    public tceqLicense: TceqLicense;
    public isTceqDateNewer: boolean = false;
    public tceqExpirationBorderClass: string = 'border-danger';

    private readonly _originalExpirationDate?: string;

    constructor(private readonly _modalReference: ModalReference<TceqLicenseCheckModalData, void>) {
        this.license = this._modalReference.config.model!.license;
        this.tceqLicense = this._modalReference.config.model!.tceqLicense;
        this._originalExpirationDate = this.license.expirationDate;

        this.isTceqDateNewer = this.isNewerThanEnvirotraxDate(this.tceqLicense.expirationDate);

        if (this.isTceqDateNewer) {
            this.tceqExpirationBorderClass = 'border-success';
        }
    }

    public copyExpirationDate(): void {
        this.license.expirationDate = this.tceqLicense.expirationDate;
    }

    public updateAndSave(): void {
        this.license.expirationDate = this.tceqLicense.expirationDate;

        this._modalReference.closeSuccess();
    }

    public cancel(): void {
        this.license.expirationDate = this._originalExpirationDate;

        this._modalReference.cancel();
    }

    private isNewerThanEnvirotraxDate(tceqExpirationDate: string): boolean {
        if (!this.license.expirationDate) {
            return true;
        }

        return this.toDateKey(tceqExpirationDate) > this.toDateKey(this.license.expirationDate);
    }

    private toDateKey(date: string): string {
        return date.substring(0, 10);
    }
}
