import { Injectable, SecurityContext } from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";
import { GisArea, GisAreaCoordinate } from "../../models/gis-areas/gis-area";
import { MapPolygon } from "../../components/map/map.component";

@Injectable({
    providedIn: 'root'
})
export class GisMapService {
    constructor(private readonly _sanitizer: DomSanitizer) {}

    public buildMapPolygons(areas: GisArea[], coordinates: GisAreaCoordinate[]): MapPolygon<GisArea>[] {
        return areas
            .map((area): MapPolygon<GisArea> | null => {
                const coords = coordinates
                    .filter(c => c.area?.id === area.id)
                    .map(c => ({ lat: c.latitude!, lng: c.longitude! }));
                if (coords.length === 0) {
                    return null;
                }
                return { name: area.name, color: area.color ?? '#000000', coordinates: coords, data: area };
            })
            .filter((p): p is MapPolygon<GisArea> => p !== null);
    }

    public buildSitePopupHtml(label: string, siteUrl: string): string {
        const safeLabel = this._sanitizer.sanitize(SecurityContext.HTML, label) ?? '';
        const safeUrl = this._sanitizer.sanitize(SecurityContext.URL, siteUrl) ?? '';
        return `<div class="px-2 py-1" style="min-width:160px">` +
            `<div class="mb-2">${safeLabel}</div>` +
            `<button onclick="window.open('${safeUrl}','_blank')" ` +
            `class="btn btn-primary btn-sm">` +
            `View Site</button></div>`;
    }
}
