import { Component } from "@angular/core";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";

@Component({
    templateUrl: './confirm-modal.component.html',
    standalone: false,
})
export class ConfirmModalComponent {
    public readonly messages: string[];

    constructor(
        private readonly _modalReference: ModalReference<string[], void>
    ) {
        this.messages = _modalReference.config.model!;
    }

    public cancel(): void {
        this._modalReference.cancel();
    }

    public submit(): void {
        this._modalReference.closeSuccess();
    }
}