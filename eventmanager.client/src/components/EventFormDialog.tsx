import { Dialog, DialogTitle, DialogContent, DialogActions, TextField, Button, CircularProgress,} from "@mui/material";
import type { Event } from "../types/eventTypes";
import React from "react";

interface Props {
    open: boolean;
    saving: boolean;
    initialValues: Event;
    onClose: () => void;
    onSave: (values: Event) => void;
}

export default function EventFormDialog({
    open,
    saving,
    initialValues,
    onClose,
    onSave,
}: Props) {
    const [values, setValues] = React.useState<Event>(initialValues);

    React.useEffect(() => { if (open) { setValues(initialValues); }}, [open]);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        onSave({
            ...values,
            country: values.country?.trim() === "" ? null : values.country,
        });
    };

    return (
        <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
            <DialogTitle>
                {initialValues.eventId ? "Edit Event" : "Create Event"}
            </DialogTitle>
            <form onSubmit={handleSubmit}>
                <DialogContent sx={{ display: "flex", flexDirection: "column", gap: 2, mt: 1 }}>
                    <TextField
                        sx={{ mt: 1 }}
                        label="Name"
                        value={values.name}
                        onChange={(e) => setValues({ ...values, name: e.target.value })}
                        disabled={saving}
                        required
                    />

                    <TextField
                        label={`Location (${values.location?.length || 0} / 100)`}
                        value={values.location}
                        onChange={(e) => setValues({ ...values, location: e.target.value })}
                        disabled={saving}
                        required
                        slotProps={{ htmlInput: { maxLength: 100 } }}
                    />

                    <TextField
                        label="Country"
                        value={values.country ?? ""}
                        onChange={(e) => setValues({ ...values, country: e.target.value })}
                        disabled={saving}
                    />

                    <TextField
                        label="Capacity"
                        type="number"
                        value={values.capacity ?? ""}
                        onChange={(e) => setValues({ ...values, capacity: e.target.value === "" ? null : Number(e.target.value), })}
                        slotProps={{ htmlInput: { min: 1 } }}
                        disabled={saving}
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={onClose} disabled={saving}>Cancel</Button>
                    <Button type="submit" variant="contained" color="primary" disabled={saving}>
                        {saving ? <CircularProgress size={20} /> : "Save"}
                    </Button>
                </DialogActions>
            </form>
        </Dialog>
    );
}
