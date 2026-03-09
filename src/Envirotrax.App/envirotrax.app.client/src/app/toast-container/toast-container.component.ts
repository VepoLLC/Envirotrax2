import { Component, OnDestroy, OnInit } from "@angular/core";
import { Toast, ToastService, ToastType } from "../shared/services/toast.service";
import { Subscription } from "rxjs";

@Component({
    selector: 'vp-toast-container',
    templateUrl: './toast-container.component.html',
    styleUrl: './toast-container.component.css',
    standalone: false
})
export class ToastContainerComponent implements OnInit, OnDestroy {
    private _subscription!: Subscription;

    public toasts: ToastVm[] = [];
    public toastType = ToastType;

    constructor(
        private readonly _toastService: ToastService
    ) {

    }

    public ngOnInit(): void {
        this._subscription = this._toastService.onShown().subscribe(toast => {
            const toastVm: ToastVm = {
                entity: toast
            };

            this.toasts.push(toastVm);

            const timeout = toast.seconds
                ? toast.seconds * 1000
                : 3000;

            setTimeout(() => {
                toastVm.isVisible = true;
            }, 50);

            setTimeout(() => {
                const index = this.toasts.indexOf(toastVm);
                this.toasts.splice(index, 1);
            }, timeout);
        });
    }

    public ngOnDestroy(): void {
        this._subscription?.unsubscribe();
    }
}

interface ToastVm {
    entity: Toast;
    isVisible?: boolean;
}