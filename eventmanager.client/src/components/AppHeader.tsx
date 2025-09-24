import { AppBar, Toolbar, Typography, Avatar, Box } from "@mui/material";
import mxcLogo from "../assets/mxc_logo.png";

export default function AppHeader() {
    return (
        <AppBar position="static" color="default">
            <Toolbar sx={{ display: "flex", justifyContent: "space-between" }}>
                <Box sx={{ display: "flex", alignItems: "center" }}>
                    <img src={mxcLogo} alt="MXC Logo" style={{ height: 40 }} />
                </Box>

                <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
                    <Typography variant="body1">user@example.com</Typography>
                    <Avatar alt="Profile" src="/logo192.png" sx={{ width: 40, height: 40 }} />
                </Box>
            </Toolbar>
        </AppBar>
    );
}