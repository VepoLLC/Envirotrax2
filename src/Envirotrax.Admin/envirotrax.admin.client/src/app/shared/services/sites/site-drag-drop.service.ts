import { Injectable } from "@angular/core";
import { SiteDetail } from "../../models/sites/site-detail";

@Injectable({
    providedIn: 'root'
})
export class SiteDragDropService {
    public draggedSite?: DraggedSite;
}

export interface DraggedSite {
    site: SiteDetail;
    closeWindow: () => void;
}
