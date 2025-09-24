import { Dialog, DialogTitle, DialogContent, DialogActions, Button, CircularProgress, Typography } from "@mui/material";

interface Props {
    open: boolean;
    deleting: boolean;
    title: string;
    message: string;
    onClose: () => void;
    onConfirm: () => void;
}

export default function ConfirmDialog({ open, deleting, title, message, onClose, onConfirm }: Props) {
    return (
        <Dialog open={open} onClose={onClose}>
            <DialogTitle>{title}</DialogTitle>
            <DialogContent>
                <Typography>{message}</Typography>
            </DialogContent>
            <DialogActions>
                <Button onClick={onClose} disabled={deleting}>Cancel</Button>
                <Button onClick={onConfirm} variant="contained" color="error" disabled={deleting}>
                    {deleting ? <CircularProgress size={20} /> : "Delete"}
                </Button>
            </DialogActions>
        </Dialog>
    );
}