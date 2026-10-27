import { authApi } from "./authApi";
import type { AuthTokenResponse } from "../types/auth";

let pendingRefresh: Promise<AuthTokenResponse> | null = null;

export function refreshSession(): Promise<AuthTokenResponse> {
	if (pendingRefresh) {
		return pendingRefresh;
	}

	const request = authApi.refresh();
	pendingRefresh = request;

	void request
		.finally(() => {
			if (pendingRefresh === request) {
				pendingRefresh = null;
			}
		})
		.catch(() => {});

	return request;
}
