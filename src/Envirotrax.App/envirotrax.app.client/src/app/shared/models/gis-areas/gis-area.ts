export interface GisArea {
    id?: number;
    name?: string;
    color?: string;
}

export interface ReferencedGisArea {
    id?: number;
    name?: string;
    color?: string;
}

export interface GisAreaCoordinate {
    id?: number;
    area?: ReferencedGisArea;
    latitude?: number;
    longitde?: number;
}

export interface DefaultGisMapView {
    gisCenterLatitude?: number;
    gisCenterLongitude?: number;
    gisCenterZoom?: number;
}
