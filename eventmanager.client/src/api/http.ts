import { getAccessToken, isTokenExpired } from "./tokenStore";
import { refresh, RefreshError } from "./authApi";

export async function authorizedFetch(input: RequestInfo, init: RequestInit = {}): Promise<Response> {
    let token = getAccessToken();
    if (!token) {
        throw new RefreshError("Not authenticated");
    }

    //Refresh token if expired or about to expire
    if (isTokenExpired(token)) {
        const data = await refresh();
        token = data.token;
    }

    let res = await fetch(input, withAuth(init, token));
    if (res.status !== 401) { // If not unauthorized, return the response
        return res;
    }

    try {                     // Try refreshing the token and retrying once if unauthorized
        const refreshed = await refresh();
        res = await fetch(input, withAuth(init, refreshed.token));
    } catch (err) {
        if (err instanceof RefreshError) {
            throw err;
        }
        throw new Error("Unexpected error during refresh");
    }

    return res;
}

//Set Authorization header and Content-Type if needed
function withAuth(init: RequestInit, token: string): RequestInit {
    const headers = new Headers(init.headers || {});
    headers.set("Authorization", `Bearer ${token}`);

    if (!headers.has("Content-Type") && init.body) {
        headers.set("Content-Type", "application/json");
    }
    return { ...init, headers };
}
