import { Injectable, Type } from "@angular/core";
import { ModalConfig, ModalReference, ModalService, ModalSize } from "@developer-partners/ngx-modal-dialog";
import { ConfirmModalComponent } from "../../components/modals/confirm-modal.component";
import { MessageModalComponent } from "../../components/modals/message-modal.component";

@Injectable({
    providedIn: 'root'
})
export class ModalHelperService {
    constructor(private readonly _modalService: ModalService) {

    }

    public show<TConfig, TResult = TConfig>(componentType: Type<any>, config: ModalConfig<TConfig>): ModalReference<TConfig, TResult> {
        return this._modalService.show(componentType, config);
    }

    public showMessage(config: MessageConfig): ModalReference<string[], void> {
        return this._modalService.show<string[], void>(MessageModalComponent, {
            title: config.title || 'Information',
            type: config.type,
            model: config.messages
        });
    }

    public confirm(config: MessageConfig): ModalReference<string[], void> {
        return this._modalService.show<string[], void>(ConfirmModalComponent, {
            title: config.title || 'Question',
            type: config.type,
            model: config.messages
        });
    }

    public showDeleteConfirmation(): ModalReference<string[], void> {
        return this.confirm({
            messages: ['Are you sure that you want to delete this record?']
        });
    }

    public showReactivateConfirmation(): ModalReference<string[], void> {
        return this.confirm({
            messages: ['Are you sure that you want to reactivate this record?']
        });
    }
}

export interface MessageConfig {
    title?: string;
    type?: 'default' | 'error' | 'warning' | 'success';
    messages: string[];
}