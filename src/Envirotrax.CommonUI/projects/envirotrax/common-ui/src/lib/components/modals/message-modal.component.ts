import { Component } from "@angular/core";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";

@Component({
    templateUrl: 'message-modal.component.html',
    standalone: false,
})
export class MessageModalComponent {
    public messages: string[] = [];

    constructor(private readonly _modalReference: ModalReference<string[], void>) {
        this.messages = _modalReference.config.model || [];
    }

    public close(): void {
        this._modalReference.closeSuccess();
    }
}