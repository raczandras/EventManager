import { useEffect, useState } from "react";
import { Box, Snackbar, Alert, CircularProgress } from "@mui/material";
import type { Event } from "../types/eventTypes";
import * as api from "../api/eventApi";
import EventTable from "../components/EventTable";
import EventFormDialog from "../components/EventFormDialog";
import ConfirmDialog from "../components/ConfirmDialog";

export default function EventsPage() {
    const [events, setEvents] = useState<Event[]>([]);
    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(10);
    const [totalCount, setTotalCount] = useState(0);
    const [sortBy, setSortBy] = useState<string | undefined>();
    const [descending, setDescending] = useState(false);

    const [loading, setLoading] = useState(false);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);

    const [openForm, setOpenForm] = useState(false);
    const [openDelete, setOpenDelete] = useState(false);
    const [selectedEvent, setSelectedEvent] = useState<Event | null>(null);


    useEffect(() => { loadEvents(); }, [page, pageSize, sortBy, descending]);

    async function loadEvents() {
        setLoading(true);
        try {
            const response = await api.getEvents(page + 1, pageSize, sortBy, descending);
            setEvents(response.items);
            setTotalCount(response.totalCount);
        } catch (err: any) {
            setErrorMessage(err.message || "Unknown error while loading events");
        } finally {
            setLoading(false);
        }
    }

    const handleSort = (column: string) => {
        if (sortBy === column) {
            setDescending(!descending);
        } else {
            setSortBy(column);
            setDescending(false);
        }
        setPage(0);
    };

    async function handleSave(event: Event) {
        setSaving(true);
        try {
            if (event.eventId) {
                const updatedEvent = await api.updateEvent(event);
                setEvents(currentEvents => 
                    currentEvents.map(e => 
                        e.eventId === updatedEvent.eventId ? updatedEvent : e
                    )
                );
                setSuccessMessage("Event updated successfully!");
            } else {
                await api.createEvent(event);
                setSuccessMessage("Event created successfully!");
                await loadEvents();
            }
            setOpenForm(false);
        } catch (err: any) {
            setErrorMessage(err.message || "Unknown error while saving event");
        } finally {
            setSaving(false);
        }
    }

    async function handleDelete() {
        if (!selectedEvent) return;
        setDeleting(true);
        try {
            await api.deleteEvent(selectedEvent.eventId);
            setSuccessMessage("Event deleted successfully!");
            await loadEvents();
            setOpenDelete(false);
        } catch (err: any) {
            setErrorMessage(err.message || "Unknown error while deleting event");
        } finally {
            setDeleting(false);
        }
    }

    return (
        <Box sx={{ p: 3 }}>
            {loading ?
                (<Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", height: "calc(100vh - 130px)", }}>
                    <CircularProgress />
                </Box>) :
                (<EventTable
                    events={events}
                    page={page}
                    pageSize={pageSize}
                    totalCount={totalCount}
                    onEdit={(event) => { setSelectedEvent(event); setOpenForm(true); }}
                    onDelete={(event) => { setSelectedEvent(event); setOpenDelete(true); }}
                    onCreate={() => { setSelectedEvent(null); setOpenForm(true); }}
                    onPageChange={(newPage, newSize) => { setPage(newPage); setPageSize(newSize); }}
                    sortBy={sortBy}
                    descending={descending}
                    onSort={handleSort}
                />
                )}

            <EventFormDialog
                open={openForm}
                saving={saving}
                initialValues={selectedEvent || { eventId: 0, name: "", location: "", country: null, capacity: null }}
                onClose={() => setOpenForm(false)}
                onSave={handleSave}
            />

            <ConfirmDialog
                open={openDelete}
                deleting={deleting}
                title="Delete Event"
                message={`Are you sure you want to delete ${selectedEvent?.name}?`}
                onClose={() => setOpenDelete(false)}
                onConfirm={handleDelete}
            />

            <Snackbar open={!!successMessage} autoHideDuration={3000} onClose={() => setSuccessMessage(null)} anchorOrigin={{ vertical: "bottom", horizontal: "center" }}>
                <Alert severity="success" onClose={() => setSuccessMessage(null)}>
                    {successMessage}
                </Alert>
            </Snackbar>

            <Snackbar open={!!errorMessage} autoHideDuration={3000} onClose={() => setErrorMessage(null)} anchorOrigin={{ vertical: "bottom", horizontal: "center" }}>
                <Alert severity="error" onClose={() => setErrorMessage(null)}>
                    {errorMessage}
                </Alert>
            </Snackbar>
        </Box>
    );
}