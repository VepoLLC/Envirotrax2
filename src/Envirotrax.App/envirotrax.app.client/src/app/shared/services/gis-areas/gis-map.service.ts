import { Injectable } from "@angular/core";
import { GisArea, GisAreaCoordinate } from "../../models/gis-areas/gis-area";
import { MapPolygon } from "../../components/map/map.component";

@Injectable({
    providedIn: 'root'
})
export class GisMapService {
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
        return `<div class="px-2 py-1" style="min-width:160px">` +
            `<div class="mb-2">${label}</div>` +
            `<button onclick="window.open('${siteUrl}','_blank')" ` +
            `class="btn btn-primary btn-sm">` +
            `View Site</button></div>`;
    }

    public escapeHtml(text?: string | null): string {
        return text
            ?.replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;') ?? '';
    }
}
