import { Injectable, Type } from "@angular/core";
import { WindowConfig } from "../../window/window-config";
import { WindowContainerComponent } from "../../window/window-container.component";
import { Observable, Subject } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class WindowService {
    private readonly _windows$: Subject<AppWindow<any, any>> = new Subject();

    public onWindowAdded(): Observable<AppWindow<any, any>> {
        return this._windows$.asObservable();
    }

    public addWindow<TComponent, TData>(component: Type<TComponent>, config: WindowConfig<TData>): void {
        this._windows$.next({
            component,
            config
        });
    }
}

export interface AppWindow<TComponent, TData> {
    component: Type<TComponent>,
    config: WindowConfig<TData>
}
