import { apiRequest } from "./client";

export interface UserProfile {
	id: string;
	userName: string;
	email: string;
	avatarUrl: string | null;
	createdAt: string;
}

export interface UserDiscussion {
	rootCommentId: string;
	preview: string;
	lastActivityAt: string;
}

const USERS_BASE_URL = "/api/v1/users";
const COMMENTS_BASE_URL = "/api/v1/comments";

export const profileApi = {
	getMe(accessToken: string): Promise<UserProfile> {
		return apiRequest<UserProfile>(`${USERS_BASE_URL}/me`, {
			headers: {
				Authorization: `Bearer ${accessToken}`,
			},
		});
	},

	updateAvatar(file: File, accessToken: string): Promise<UserProfile> {
		const formData = new FormData();

		formData.append("File", file);

		return apiRequest<UserProfile>(`${USERS_BASE_URL}/me/avatar`, {
			method: "PUT",
			headers: {
				Authorization: `Bearer ${accessToken}`,
			},
			body: formData,
		});
	},

	getUserDiscussions(
		accessToken: string,
		limit = 5,
	): Promise<UserDiscussion[]> {
		return apiRequest<UserDiscussion[]>(
			`${COMMENTS_BASE_URL}/user-discussions?limit=${limit}`,
			{
				headers: {
					Authorization: `Bearer ${accessToken}`,
				},
			},
		);
	},
};
