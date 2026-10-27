export interface RegisterRequest {
	userName: string;
	email: string;
	password: string;
}

export interface LoginRequest {
	email: string;
	password: string;
}

export interface AuthTokenResponse {
	accessToken: string;
	expiresAt: string;
}

export interface UserResponse {
	id: string;
	userName: string;
	email: string;
	avatarUrl: string | null;
	createdAt: string;
}
