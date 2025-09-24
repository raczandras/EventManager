import type { Event, EventResponse } from "../types/eventTypes";

const API_URL = "api/event";

export async function getEvents(page: number, pageSize: number, sortBy?: string, descending: boolean = false): Promise<EventResponse> {
    const params = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString(),
    });

    if (sortBy) {
        params.append('sortBy', sortBy);
        params.append('descending', descending.toString());
    }

    const response = await fetch(`${API_URL}?${params}`);
    if (!response.ok) throw new Error(`Failed to load events (${response.status})`);
    return response.json();
}

export async function createEvent(event: Event): Promise<void> {
    const response = await fetch(API_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(event),
    });
    if (!response.ok) throw new Error(`Failed to create event (${response.status})`);
}

export async function updateEvent(event: Event): Promise<void> {
    const response = await fetch(API_URL, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(event),
    });
    if (!response.ok) throw new Error(`Failed to update event (${response.status})`);
}

export async function deleteEvent(id: number): Promise<void> {
    const response = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
    if (!response.ok) throw new Error(`Failed to delete event (${response.status})`);
}