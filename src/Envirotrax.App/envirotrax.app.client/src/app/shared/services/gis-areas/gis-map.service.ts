import { Injectable, SecurityContext } from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";
import { GisArea, GisAreaCoordinate } from "../../models/gis-areas/gis-area";
import { MapPolygon } from "@envirotrax/common-ui";

@Injectable({
    providedIn: 'root'
})
export class GisMapService {
    constructor(private readonly _sanitizer: DomSanitizer) { }

    public buildRings(coordinates: GisAreaCoordinate[]): { lat: number, lng: number }[][] {
        const ringsByIndex = new Map<number, { lat: number, lng: number }[]>();

        for (const coordinate of coordinates) {
            const index = coordinate.polygonIndex ?? 0;
            let ring = ringsByIndex.get(index);

            if (!ring) {
                ring = [];
                ringsByIndex.set(index, ring);
            }

            ring.push({ lat: coordinate.latitude!, lng: coordinate.longitude! });
        }

        const sortedIndexes = Array.from(ringsByIndex.keys()).sort((first, second) => first - second);
        const rings: { lat: number, lng: number }[][] = [];

        for (const index of sortedIndexes) {
            rings.push(ringsByIndex.get(index)!);
        }

        return rings;
    }

    public buildCoordinates(outer: { lat: number, lng: number }[], holes?: { lat: number, lng: number }[][]): GisAreaCoordinate[] {
        const coordinates: GisAreaCoordinate[] = [];

        for (const point of outer) {
            coordinates.push({ polygonIndex: 0, latitude: point.lat, longitude: point.lng });
        }

        if (holes) {
            for (let index = 0; index < holes.length; index++) {
                for (const point of holes[index]) {
                    coordinates.push({ polygonIndex: index + 1, latitude: point.lat, longitude: point.lng });
                }
            }
        }

        return coordinates;
    }

    public buildMapPolygons(areas: GisArea[], coordinates: GisAreaCoordinate[]): MapPolygon<GisArea>[] {
        return areas
            .map((area): MapPolygon<GisArea> | null => {
                const areaCoordinates = coordinates.filter(c => c.area?.id === area.id);
                const rings = this.buildRings(areaCoordinates);

                if (!rings.length || !rings[0].length) {
                    return null;
                }

                return { name: area.name, color: area.color || '#000000', coordinates: rings[0], holes: rings.slice(1), data: area };
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
