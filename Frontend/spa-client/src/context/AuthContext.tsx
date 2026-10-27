import {
	createContext,
	useCallback,
	useEffect,
	useMemo,
	useRef,
	useState,
	type ReactNode,
} from "react";

import { authApi } from "../api/authApi";
import { refreshSession } from "../api/authSession";

import type { LoginRequest } from "../types/auth";

interface AuthUser {
	userName: string;
}

export interface AuthContextValue {
	accessToken: string | null;
	user: AuthUser | null;
	isAuthenticated: boolean;
	isLoading: boolean;
	login: (data: LoginRequest) => Promise<void>;
	logout: () => Promise<void>;
	refresh: () => Promise<string | null>;
}

export const AuthContext = createContext<AuthContextValue | null>(null);

interface AuthProviderProps {
	children: ReactNode;
}

function getUserFromToken(accessToken: string | null): AuthUser | null {
	if (!accessToken) return null;

	try {
		const parts = accessToken.split(".");

		if (parts.length !== 3) return null;

		const base64 = parts[1].replace(/-/g, "+").replace(/_/g, "/");

		const padded = base64.padEnd(Math.ceil(base64.length / 4) * 4, "=");

		const bytes = Uint8Array.from(atob(padded), (character) =>
			character.charCodeAt(0),
		);

		const payload = JSON.parse(new TextDecoder().decode(bytes)) as Record<
			string,
			unknown
		>;

		const candidates = [
			payload.userName,
			payload.username,
			payload.preferred_username,
			payload.unique_name,
			payload.name,
			payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
		];

		const userName = candidates.find(
			(value): value is string =>
				typeof value === "string" && value.trim().length > 0,
		);

		return userName ? { userName } : null;
	} catch {
		return null;
	}
}

export function AuthProvider({ children }: AuthProviderProps) {
	const [accessToken, setAccessToken] = useState<string | null>(null);
	const [isLoading, setIsLoading] = useState(true);

	const sessionVersion = useRef(0);

	const user = useMemo(() => getUserFromToken(accessToken), [accessToken]);

	const refresh = useCallback(async (): Promise<string | null> => {
		const version = sessionVersion.current;

		try {
			const response = await refreshSession();

			if (version !== sessionVersion.current) {
				return null;
			}

			setAccessToken(response.accessToken);

			return response.accessToken;
		} catch {
			if (version === sessionVersion.current) {
				setAccessToken(null);
			}

			return null;
		}
	}, []);

	useEffect(() => {
		let active = true;
		const version = sessionVersion.current;

		async function restoreSession() {
			try {
				const response = await refreshSession();

				if (active && version === sessionVersion.current) {
					setAccessToken(response.accessToken);
				}
			} catch {
				if (active && version === sessionVersion.current) {
					setAccessToken(null);
				}
			} finally {
				if (active) {
					setIsLoading(false);
				}
			}
		}

		void restoreSession();

		return () => {
			active = false;
		};
	}, []);

	const login = useCallback(async (data: LoginRequest) => {
		const version = ++sessionVersion.current;

		const response = await authApi.login(data);

		if (version === sessionVersion.current) {
			setAccessToken(response.accessToken);
		}
	}, []);

	const logout = useCallback(async () => {
		if (!accessToken) return;

		await authApi.logout(accessToken);

		sessionVersion.current++;
		setAccessToken(null);
	}, [accessToken]);

	return (
		<AuthContext.Provider
			value={{
				accessToken,
				user,
				isAuthenticated: accessToken !== null,
				isLoading,
				login,
				logout,
				refresh,
			}}>
			{children}
		</AuthContext.Provider>
	);
}
