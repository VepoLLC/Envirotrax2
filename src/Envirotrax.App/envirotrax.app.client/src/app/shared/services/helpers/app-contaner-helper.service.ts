import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { NavigationStart, Router } from "@angular/router";

@Injectable({
    providedIn: 'root'
})
export class AppContainerHelperService {
    private _useContainer$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);

    constructor(router: Router) {
        router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this._useContainer$.next(true);
            }
        });
    }

    public usContainer(): Observable<boolean> {
        return this._useContainer$.asObservable();
    }

    public setContainerVisibility(isVisible: boolean): void {
        this._useContainer$.next(isVisible);
    }
}