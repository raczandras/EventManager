import { createContext, useContext, useState, useEffect } from "react";
import { login as apiLogin, logout as apiLogout, RefreshError } from "../api/authApi";
import { clearTokens, getAccessToken, getUserEmail, setTokens } from "../api/tokenStore";
import { useNavigate } from "react-router-dom";

type AuthContextType = {
    email: string | null;
    isAuthenticated: boolean;
    login: (email: string, password: string) => Promise<void>;
    logout: () => Promise<void>;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
    const navigate = useNavigate();
    const [email, setEmail] = useState<string | null>(getUserEmail());

    useEffect(() => {
        if (!getAccessToken()) {
            clearTokens();
            setEmail(null);
        }
    }, []);

    //Global refresh failure listener
    useEffect(() => {
        const handler = (e: any) => {
            if (e.reason instanceof RefreshError) {
                clearTokens();
                setEmail(null);
                navigate("/login", { replace: true });
            }
        };
        window.addEventListener("unhandledrejection", handler);
        return () => window.removeEventListener("unhandledrejection", handler);
    }, [navigate]);

    const login = async (email: string, password: string) => {
        const { token, refreshToken } = await apiLogin(email, password);
        setTokens({ accessToken: token, refreshToken, email: email });
        setEmail(email);
    };

    const logout = async () => {
        await apiLogout();
        clearTokens();
        setEmail(null);
        navigate("/login", { replace: true });
    };

    return (
        <AuthContext.Provider value={{ email, isAuthenticated: !!email, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const ctx = useContext(AuthContext);
    if (!ctx) {
        throw new Error("useAuth must be used inside AuthProvider");
    }
    return ctx;
}
