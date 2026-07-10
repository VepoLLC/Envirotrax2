import { Component, ComponentRef, EventEmitter, InjectOptions, Injector, input, Input, OnDestroy, OnInit, Output, ProviderToken, Type, ViewChild, ViewContainerRef } from "@angular/core";
import { WindowConfig, WindowReference } from "./window-config";

@Component({
    standalone: false,
    selector: 'vp-window',
    templateUrl: './window.component.html'
})
export class WindowComponent implements OnInit, OnDestroy {
    private _componentRef!: ComponentRef<any>;

    @Input()
    public config!: WindowConfig<any>;

    @Input()
    public componentType!: Type<any>;

    @Output()
    public close: EventEmitter<void> = new EventEmitter();

    @Output()
    public minimize: EventEmitter<void> = new EventEmitter();

    @ViewChild('hostContainer', { read: ViewContainerRef, static: true })
    public hostContainer!: ViewContainerRef;

    constructor(
        private readonly _injector: Injector
    ) {

    }

    public ngOnInit(): void {
        const reference = new WindowReference<any>(this.config);
        const map = new WeakMap<any, any>();

        map.set(WindowReference, reference);

        const windowInjector = new WindowInjector(this._injector, map);

        this._componentRef = this.hostContainer.createComponent(this.componentType, {
            injector: windowInjector
        });
    }

    public ngOnDestroy(): void {
        this._componentRef?.destroy();
    }

    public onClose(): void {
        this.close.emit();
    }

    public onMinimize(): void {
        this.minimize.emit();
    }
}

class WindowInjector implements Injector {
    private readonly _injector: Injector;
    private readonly _extraDependecnies: WeakMap<any, any>;

    constructor(
        injector: Injector,
        extraDependencies: WeakMap<any, any>
    ) {
        this._injector = injector;
        this._extraDependecnies = extraDependencies;
    }

    public get<T>(token: ProviderToken<T>, notFoundValue?: any, options?: InjectOptions): any {
        const resolved = this._extraDependecnies.get(token);

        if (resolved) {
            return resolved;
        }

        return this._injector.get<T>(token, notFoundValue, options);
    }
}