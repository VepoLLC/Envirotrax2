import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class ToastService {
    private readonly _toast$: Subject<Toast> = new Subject();

    public show(toast: Toast): void {
        this._toast$.next(toast);
    }

    public onShown(): Observable<Toast> {
        return this._toast$.asObservable();
    }

    public successfullySaved(entityName?: string): void {
        const message = entityName
            ? `Successfully saved ${entityName}!`
            : 'Successfully saved!';

        this.show({
            text: message,
            type: ToastType.Success
        });
    }

    public successFullyDeleted(entityName?: string): void {
        const message = entityName
            ? `Successfully deleted ${entityName}!`
            : 'Successfully deleted!';

        this.show({
            text: message,
            type: ToastType.Success
        });
    }

    public failedToSave(entityName?: string): void {
        const message = entityName
            ? `Failed to save ${entityName}`
            : 'Failed to save';

        this.show({
            text: message,
            type: ToastType.Error
        });
    }

    public successfullyReactivated(entityName?: string): void {
        const message = entityName
            ? `Successfully reactivated ${entityName}!`
            : 'Successfully reactivated';

        this.show({
            text: message,
            type: ToastType.Success
        });
    }
}

export interface Toast {
    text: string;
    seconds?: number;
    type?: ToastType
}

export enum ToastType {
    Default,
    Success,
    Warning,
    Error
}