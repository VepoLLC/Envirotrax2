import { Injectable, SecurityContext } from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";
import { GisArea, GisAreaCoordinate } from "../../models/gis-areas/gis-area";
import { MapPoint, MapPolygon } from "@envirotrax/common-ui";

// The database keeps one flat, ordered list of vertices per area, with polygonIndex saying which
export interface PolygonRings {
    outer: MapPoint[];
    holes: MapPoint[][];
}

@Injectable({
    providedIn: 'root'
})
export class GisMapService {
    constructor(private readonly _sanitizer: DomSanitizer) { }

    // Flat list of vertices from the database -> rings the map can draw.
    public buildPolygonRings(coordinates: GisAreaCoordinate[]): PolygonRings {
        const pointsByPolygonIndex = new Map<number, MapPoint[]>();

        for (const coordinate of coordinates) {
            const polygonIndex = coordinate.polygonIndex ?? 0;
            let points = pointsByPolygonIndex.get(polygonIndex);

            if (!points) {
                points = [];
                pointsByPolygonIndex.set(polygonIndex, points);
            }

            points.push({ lat: coordinate.latitude!, lng: coordinate.longitude! });
        }

        const rings: PolygonRings = { outer: [], holes: [] };
        const polygonIndexes = Array.from(pointsByPolygonIndex.keys()).sort((first, second) => first - second);

        for (const polygonIndex of polygonIndexes) {
            const points = pointsByPolygonIndex.get(polygonIndex)!;

            if (polygonIndex === 0) {
                rings.outer = points;
            } else {
                rings.holes.push(points);
            }
        }

        return rings;
    }

    // Rings edited on the map -> flat list of vertices for the database.
    public buildFlatCoordinates<TData>(polygon: MapPolygon<TData>): GisAreaCoordinate[] {
        const coordinates: GisAreaCoordinate[] = [];

        for (const point of polygon.coordinates) {
            coordinates.push({ polygonIndex: 0, latitude: point.lat, longitude: point.lng });
        }

        const holes = polygon.holes ?? [];

        for (let holeIndex = 0; holeIndex < holes.length; holeIndex++) {
            for (const point of holes[holeIndex]) {
                coordinates.push({ polygonIndex: holeIndex + 1, latitude: point.lat, longitude: point.lng });
            }
        }

        return coordinates;
    }

    public buildMapPolygons(areas: GisArea[], coordinates: GisAreaCoordinate[]): MapPolygon<GisArea>[] {
        return areas
            .map((area): MapPolygon<GisArea> | null => {
                const areaCoordinates = coordinates.filter(c => c.area?.id === area.id);
                const rings = this.buildPolygonRings(areaCoordinates);

                if (!rings.outer.length) {
                    return null;
                }

                return { name: area.name, color: area.color || '#000000', coordinates: rings.outer, holes: rings.holes, data: area };
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
