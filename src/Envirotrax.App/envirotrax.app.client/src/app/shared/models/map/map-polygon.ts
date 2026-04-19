export interface MapPolygon {
    id?: number;
    name?: string;
    color: string;
    coordinates: { lat: number; lng: number }[];
}
