import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, ViewChild } from "@angular/core";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { SignaturePadComponent, NgSignaturePadOptions } from "@almothafar/angular-signature-pad";

export interface FogSignatureModel {
    existingSignature: string | null;
}

@Component({
    standalone: false,
    templateUrl: './fog-signature-pad-modal.component.html',
    styleUrl: './fog-signature-pad-modal.component.scss'
})
export class FogSignaturePadModalComponent implements AfterViewInit {
    @ViewChild('signaturePad') private signaturePad?: SignaturePadComponent;
    @ViewChild('canvasWrapper', { static: true }) private canvasWrapper!: ElementRef<HTMLElement>;

    public signaturePadOptions: NgSignaturePadOptions = {
        canvasWidth: 600,
        canvasHeight: 350,
        backgroundColor: 'rgb(255, 255, 255)',
        penColor: 'rgb(0, 0, 0)'
    };

    constructor(
        private readonly _modalReference: ModalReference<FogSignatureModel, string>,
        private readonly _changeDetector: ChangeDetectorRef
    ) { }

    public ngAfterViewInit(): void {
        // Defer so the modal has finished laying out before we measure its width.
        setTimeout(() => this.initializePad());
    }

    private initializePad(): void {
        const width = this.canvasWrapper.nativeElement.clientWidth;

        if (width > 0) {
            this.signaturePadOptions = { ...this.signaturePadOptions, canvasWidth: width };
            // Flush the new options into the signature-pad before resizing its canvas.
            this._changeDetector.detectChanges();
        }

        this.signaturePad?.redrawCanvas();

        // Reload any previously saved signature so it can be continued/edited.
        const existing = this._modalReference.config.model?.existingSignature;
        if (existing) {
            this.signaturePad?.fromDataURL(existing);
        }
    }

    public clear(): void {
        this.signaturePad?.clear();
    }

    public save(): void {
        const dataUrl = this.signaturePad && !this.signaturePad.isEmpty()
            ? this.signaturePad.toDataURL('image/png')
            : '';

        this._modalReference.closeSuccess(dataUrl);
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
