export interface Event {
    eventId: number;
    name: string;
    location: string;
    country: string | null;
    capacity: number | null;
}

export interface EventResponse {
    items: Event[];
    totalCount: number;
    page: number;
    pageSize: number;
}