import { AppBar, Toolbar, Typography, Avatar, Box } from "@mui/material";
import mxcLogo from "../assets/mxc_logo.png";
import { useAuth } from "../context/AuthContext";
import { NavLink } from "react-router-dom";

export default function AppHeader() {
    const { email, logout } = useAuth();

    return (
        <AppBar position="static" color="default">
            <Toolbar sx={{ display: "flex", justifyContent: "space-between" }}>
                <Box sx={{ display: "flex", alignItems: "center" }}>
                    <img src={mxcLogo} alt="MXC Logo" style={{ height: 40 }} />
                </Box>

                <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
                    <Box sx={{ display: "flex", flexDirection: "column" }}>
                        <Typography variant="body1">{email}</Typography>
                        <NavLink to="/login" onClick={logout} style={{ textDecoration: "none", color: "inherit", textAlign: "right" }}><Typography variant="body1">Logout</Typography></NavLink>
                    </Box>
                                         {/*This won't exist but oh well*/}
                    <Avatar alt="Profile" src="/logo192.png" sx={{ width: 40, height: 40 }} />                
                </Box>
            </Toolbar>
        </AppBar>
    );
}
