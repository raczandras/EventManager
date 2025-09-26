import type { Event, EventResponse } from "../types/eventTypes";
import { authorizedFetch } from "./http";

const API_URL = "/api/event";

export async function getEvents(page: number, pageSize: number, sortBy?: string, descending: boolean = false): Promise<EventResponse> {
    const params = new URLSearchParams({
        page: String(page),
        pageSize: String(pageSize),
    });
    if (sortBy) {
        params.set("sortBy", sortBy);
        params.set("descending", String(descending));
    }

    const response = await authorizedFetch(`${API_URL}?${params.toString()}`);
    if (!response.ok) {
        throw new Error(`Failed to load events (${response.status})`);
    }
    return response.json();
}

export async function createEvent(event: Event): Promise<Event> {
    const response = await authorizedFetch(API_URL, {
        method: "POST",
        body: JSON.stringify(event),
    });

    if (!response.ok) {
        throw new Error(`Failed to create event (${response.status})`);
    }
    return response.json();
}

export async function updateEvent(event: Event): Promise<Event> {
    const response = await authorizedFetch(API_URL, {
        method: "PUT",
        body: JSON.stringify(event),
    });

    if (!response.ok) {
        throw new Error(`Failed to update event (${response.status})`);
    }
    return response.json();
}

export async function deleteEvent(id: number): Promise<void> {
    const response = await authorizedFetch(`${API_URL}/${id}`, { method: "DELETE" });

    if (!response.ok) {
        throw new Error(`Failed to delete event (${response.status})`);
    }
}
