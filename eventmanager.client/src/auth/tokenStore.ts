import type { JwtPayload } from "../types/authTypes";

const ACCESS_TOKEN_KEY = "em.accessToken";
const REFRESH_TOKEN_KEY = "em.refreshToken";
const USER_EMAIL_KEY = "em.userEmail";

export function setTokens(tokens: { accessToken: string; refreshToken: string; email: string }) {
    localStorage.setItem(ACCESS_TOKEN_KEY, tokens.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, tokens.refreshToken);
    localStorage.setItem(USER_EMAIL_KEY, tokens.email);
}

export function getAccessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
}

export function getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
}

export function getUserEmail(): string | null {
    return localStorage.getItem(USER_EMAIL_KEY);
}

export function clearTokens() {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_EMAIL_KEY);
}

export function isTokenExpired(token: string, skewSeconds = 30): boolean {
    try {
        const [, payloadB64] = token.split(".");
        if (!payloadB64) {
            return true;
        }

        const json = atob(payloadB64.replace(/-/g, "+").replace(/_/g, "/")); //Replace so Base64URL -> Base64
        const payload = JSON.parse(json) as JwtPayload;

        if (!payload.exp) { //In case there is no exp claim, we treat it as expired
            return true;
        }

        const now = Math.floor(Date.now() / 1000); //Seconds since epoch

        return now >= payload.exp - skewSeconds;
    } catch {
        return true;
    }
}
