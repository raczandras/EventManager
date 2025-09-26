import { clearTokens, getRefreshToken, setTokens } from "../auth/tokenStore";

export class RefreshError extends Error {
    constructor(message: string) {
        super(message);
        this.name = "RefreshError";
    }
}

const AUTH_URL = "/api/authorization";

export async function login(email: string, password: string) {
    const res = await fetch(`${AUTH_URL}/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
    });

    if (!res.ok) {
        throw new Error(`Login failed (${res.status})`);
    }

    const data = (await res.json()) as { token: string; refreshToken: string };
    //Store tokens into local storage
    setTokens({ accessToken: data.token, refreshToken: data.refreshToken, email: email });
    return data;
}

export async function refresh() {
    const rt = getRefreshToken();
    if (!rt) {
        throw new RefreshError("No refresh token available");
    }

    const res = await fetch(`${AUTH_URL}/refresh`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ refreshToken: rt }),
    });

    if (!res.ok) {
        clearTokens();
        throw new RefreshError("Refresh failed");
    }

    const data = (await res.json()) as { token: string; refreshToken: string }; //convert to proper type
    //Store tokens into local storage
    setTokens({ accessToken: data.token, refreshToken: data.refreshToken, email: localStorage.getItem("em.userEmail") ?? "" });
    return data;
}

export async function logout() {
    const rt = getRefreshToken();
    try {
        if (rt) {
            await fetch(`${AUTH_URL}/logout`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ refreshToken: rt }),
            });
        }
    } finally {
        clearTokens();
    }
}
