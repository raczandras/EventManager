import {
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    TableSortLabel,
    Toolbar,
    Typography,
    Button,
    TablePagination,
} from "@mui/material";
import { Edit, Delete } from "@mui/icons-material";
import type { Event } from "../types/eventTypes";

interface Props {
    events: Event[];
    page: number;
    pageSize: number;
    totalCount: number;
    sortBy?: string;
    descending: boolean;
    onEdit: (event: Event) => void;
    onDelete: (event: Event) => void;
    onPageChange: (page: number, pageSize: number) => void;
    onCreate: () => void;
    onSort: (column: string) => void;
}

export default function EventTable({
    events,
    page,
    pageSize,
    totalCount,
    onEdit,
    onDelete,
    onPageChange,
    onCreate,
    sortBy,
    descending,
    onSort,
}: Props) {
    const handleChangePage = (_: unknown, newPage: number) => {
        onPageChange(newPage, pageSize);
    };

    const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
        onPageChange(0, parseInt(event.target.value, 10));
    };

    const createSortHandler = (column: string) => () => {
        onSort(column);
    };

    return (
        <Paper sx={{ display: "flex", flexDirection: "column", height: "calc(100vh - 130px)" }}>
            <Toolbar sx={{ display: "flex", justifyContent: "space-between" }}>
                <Typography variant="h6">Events</Typography>
                <Button variant="contained" color="primary" onClick={onCreate}>
                    Add New Event
                </Button>
            </Toolbar>

            <TableContainer sx={{ flex: 1 }}>
                <Table stickyHeader aria-label="event table">
                    <TableHead>
                        <TableRow>
                            <TableCell>
                                <TableSortLabel active={sortBy === "Name"} direction={sortBy === "Name" ? (descending ? "desc" : "asc") : "asc"} onClick={createSortHandler("Name")}>
                                    Name
                                </TableSortLabel>
                            </TableCell>
                            <TableCell>
                                <TableSortLabel active={sortBy === "Location"} direction={sortBy === "Location" ? (descending ? "desc" : "asc") : "asc"}onClick={createSortHandler("Location")}>
                                    Location
                                </TableSortLabel>
                            </TableCell>
                            <TableCell>
                                <TableSortLabel active={sortBy === "Country"} direction={sortBy === "Country" ? (descending ? "desc" : "asc") : "asc"} onClick={createSortHandler("Country")}>
                                    Country
                                </TableSortLabel>
                            </TableCell>
                            <TableCell>
                                <TableSortLabel active={sortBy === "Capacity"} direction={sortBy === "Capacity" ? (descending ? "desc" : "asc") : "asc"} onClick={createSortHandler("Capacity")}>
                                    Capacity
                                </TableSortLabel>
                            </TableCell>
                            <TableCell align="right">Actions</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {events.length > 0 ? (
                            events.map((event) => (
                                <TableRow key={event.eventId}>
                                    <TableCell>{event.name}</TableCell>
                                    <TableCell>{event.location}</TableCell>
                                    <TableCell>{event.country ?? "-"}</TableCell>
                                    <TableCell>{event.capacity ?? "-"}</TableCell>
                                    <TableCell align="right">
                                        <Button variant="outlined" color="primary" size="small" startIcon={<Edit />} onClick={() => onEdit(event)}>
                                            Edit
                                        </Button>
                                        <Button variant="outlined" color="error" size="small" startIcon={<Delete />} onClick={() => onDelete(event)} sx={{ ml: 1 }}>
                                            Delete
                                        </Button>
                                    </TableCell>
                                </TableRow>
                            ))
                        ) : (
                            <TableRow>
                                <TableCell colSpan={5} align="center">
                                    No events available
                                </TableCell>
                            </TableRow>
                        )}
                    </TableBody>
                </Table>
            </TableContainer>

            <TablePagination component="div" count={totalCount} page={page} onPageChange={handleChangePage} rowsPerPage={pageSize} onRowsPerPageChange={handleChangeRowsPerPage}/>
        </Paper>
    );
}
