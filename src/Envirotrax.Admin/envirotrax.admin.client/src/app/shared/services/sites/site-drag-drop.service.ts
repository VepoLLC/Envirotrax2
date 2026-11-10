import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";

// The Drag/Drop Merge button puts the site id in the drag data under this type. Drag data also reaches a
// site window in another browser window or tab, so sites can be merged across monitors.
export const SITE_DRAG_DATA_TYPE = 'application/x-envirotrax-site';

const SITE_MERGE_BROADCAST_CHANNEL_NAME = 'envirotrax-admin-site-merge';

export interface SiteMerged {
    sourceSiteId: number;
    destinationSiteId: number;
}

@Injectable({
    providedIn: 'root'
})
export class SiteDragDropService {
    private readonly _siteMerged$: Subject<SiteMerged> = new Subject();

    private readonly _channel: BroadcastChannel | null = typeof BroadcastChannel !== 'undefined'
        ? new BroadcastChannel(SITE_MERGE_BROADCAST_CHANNEL_NAME)
        : null;

    constructor() {
        this._channel?.addEventListener('message', (event: MessageEvent<SiteMerged>) => {
            this._siteMerged$.next(event.data);
        });
    }

    public onSiteMerged(): Observable<SiteMerged> {
        return this._siteMerged$.asObservable();
    }

    // Notifies the site windows in every browser window and tab. A BroadcastChannel doesn't receive its own
    // messages, so the windows in this tab are notified directly.
    public notifySiteMerged(siteMerged: SiteMerged): void {
        this._channel?.postMessage(siteMerged);
        this._siteMerged$.next(siteMerged);
    }
}
