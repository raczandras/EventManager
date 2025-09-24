import { Dialog, DialogTitle, DialogContent, DialogActions, TextField, Button, CircularProgress } from "@mui/material";
import type { Event } from "../types/eventTypes";
import React from "react";

interface Props {
    open: boolean;
    saving: boolean;
    initialValues: Event;
    onClose: () => void;
    onSave: (values: Event) => void;
}

export default function EventFormDialog({ open, saving, initialValues, onClose, onSave }: Props) {
    const [values, setValues] = React.useState(initialValues);

    React.useEffect(() => {
        setValues(initialValues);
    }, [initialValues]);

    const handleChange = (field: keyof Event, value: string) => {
        setValues({
            ...values,
            [field]: field === "capacity" ? Number(value) : value,
        });
    };

    return (
        <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
            <DialogTitle>{initialValues.eventId ? "Edit Event" : "Create Event"}</DialogTitle>
            <DialogContent sx={{ display: "flex", flexDirection: "column", gap: 2, mt: 1 }}>
                <TextField sx={{ mt: 1} } label="Name" value={values.name} onChange={(e) => handleChange("name", e.target.value)} disabled={saving} />
                <TextField label="Location" value={values.location} onChange={(e) => handleChange("location", e.target.value)} disabled={saving} />
                <TextField label="Country" value={values.country ?? ""} onChange={(e) => handleChange("country", e.target.value)} disabled={saving} />
                <TextField label="Capacity" type="number" value={values.capacity ?? ""} onChange={(e) => handleChange("capacity", e.target.value)} disabled={saving} />
            </DialogContent>
            <DialogActions>
                <Button onClick={onClose} disabled={saving}>Cancel</Button>
                <Button onClick={() => onSave(values)} variant="contained" color="primary" disabled={saving}>
                    {saving ? <CircularProgress size={20} /> : "Save"}
                </Button>
            </DialogActions>
        </Dialog>
    );
}