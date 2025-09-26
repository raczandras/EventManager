import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider, useAuth } from "./auth/AuthContext";
import AppHeader from "./components/AppHeader";
import EventsPage from "./pages/EventsPage";
import LoginPage from "./pages/LoginPage";

function AppContent() {
    const { isAuthenticated } = useAuth();

    return (
        <>
            {isAuthenticated && <AppHeader />}
            <Routes>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/events" element={isAuthenticated ? <EventsPage /> : <Navigate to="/login" replace />} />
                <Route path="*" element={<Navigate to={isAuthenticated ? "/events" : "/login"} replace />} />
            </Routes>
        </>
    );
}

export default function App() {
    return (
        <BrowserRouter>
            <AuthProvider>
                <AppContent />
            </AuthProvider>
        </BrowserRouter>
    );
}
