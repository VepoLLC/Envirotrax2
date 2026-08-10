import { Injectable } from "@angular/core";
import { asapScheduler, BehaviorSubject, Observable, observeOn } from "rxjs";
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
        // Pages call setContainerVisibility from ngOnInit, i.e. in the middle of a
        // change detection pass. Deliver emissions on a microtask so subscribers
        // (App calls detectChanges) never re-enter change detection mid-pass.
        return this._useContainer$.pipe(observeOn(asapScheduler));
    }

    public setContainerVisibility(isVisible: boolean): void {
        this._useContainer$.next(isVisible);
    }
}