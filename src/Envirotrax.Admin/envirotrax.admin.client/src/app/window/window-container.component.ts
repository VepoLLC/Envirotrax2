import { Component, ElementRef, OnDestroy, OnInit, Type, ViewChild } from "@angular/core";
import { WindowConfig } from "./window-config";
import { WindowService } from "../shared/services/window.service";
import { Subscription } from "rxjs";

@Component({
    standalone: false,
    selector: 'vp-window-container',
    templateUrl: './window-container.component.html',
    styles: `
    .vp-window-wrapper {
        min-width: calc(50% - 0.5rem);
    }
    `
})
export class WindowContainerComponent implements OnInit, OnDestroy {
    private _subsription$!: Subscription;

    public windows: WindowVm<any, any>[] = [];

    @ViewChild('container', { static: true })
    public container!: ElementRef<HTMLElement>;

    constructor(
        private readonly _windowService: WindowService
    ) {
    }

    public ngOnInit(): void {
        this._subsription$ = this._windowService.onWindowAdded().subscribe(window => {
            this.addWindow(window.component, window.config);
        })
    }

    public ngOnDestroy(): void {
        this._subsription$?.unsubscribe();
    }

    public addWindow<TComponent, TData>(component: Type<TComponent>, config: WindowConfig<TData>): void {
        this.windows.push({
            componentType: component,
            config: config
        });

        this.windows = [...this.windows];

        setTimeout(() => {
            this.container.nativeElement.scrollLeft = this.container.nativeElement.scrollWidth;
        }, 50);
    }

    public removeWindow(window: WindowVm<any, any>): void {
        const index = this.windows.indexOf(window);
        this.windows.splice(index, 1);
    }

    public maximizeWindow(window: WindowVm<any, any>): void {
        window.isMinimized = false;
    }

    public minimizeWindow(window: WindowVm<any, any>): void {
        window.isMinimized = true;
    }
}

export interface WindowVm<TComponent, TData> {
    componentType: Type<TComponent>;
    config: WindowConfig<TData>;
    isMinimized?: boolean;
}