import { apiRequest } from "./client";

import type {
	RegisterRequest,
	LoginRequest,
	AuthTokenResponse,
	UserResponse,
} from "../types/auth";

const BASE_URL = "/api/v1/users";

export const authApi = {
	register(data: RegisterRequest): Promise<UserResponse> {
		return apiRequest<UserResponse>(`${BASE_URL}/register`, {
			method: "POST",
			headers: {
				"Content-Type": "application/json",
			},
			body: JSON.stringify(data),
		});
	},

	login(data: LoginRequest): Promise<AuthTokenResponse> {
		return apiRequest<AuthTokenResponse>(`${BASE_URL}/login`, {
			method: "POST",
			credentials: "include",
			headers: {
				"Content-Type": "application/json",
			},
			body: JSON.stringify(data),
		});
	},

	refresh(): Promise<AuthTokenResponse> {
		return apiRequest<AuthTokenResponse>(`${BASE_URL}/refresh`, {
			method: "POST",
			credentials: "include",
		});
	},

	logout(accessToken: string): Promise<void> {
		return apiRequest<void>(`${BASE_URL}/logout`, {
			method: "POST",
			credentials: "include",
			headers: {
				Authorization: `Bearer ${accessToken}`,
			},
		});
	},

	updateAvatar(file: File, accessToken: string): Promise<unknown> {
		const formData = new FormData();

		formData.append("file", file);

		return apiRequest<unknown>(`${BASE_URL}/me/avatar`, {
			method: "PUT",
			credentials: "include",
			headers: {
				Authorization: `Bearer ${accessToken}`,
			},
			body: formData,
		});
	},
};
