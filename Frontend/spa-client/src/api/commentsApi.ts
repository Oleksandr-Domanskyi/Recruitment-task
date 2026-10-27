import { apiRequest } from "./client";
import { FORM_FIELDS } from "./forms/formFields";
import { QUERY_FIELDS } from "./forms/queryFields";

import type { Comment } from "../types/comment";
import type { CommentFilters } from "../types/commentFilters";
import type { PagedResponse } from "../types/pagination";
import type {
	CreateCommentRequest,
	UpdateCommentRequest,
} from "../types/commentRequests";

const BASE_URL = "/api/v1/comments";

type ParameterValue = string | number | boolean | null | undefined;

type FormValue = string | boolean | File | null | undefined;

export type VoteValue = -1 | 0 | 1;

const createQueryParams = (
	values: Record<string, ParameterValue>,
): URLSearchParams => {
	const entries = Object.entries(values)
		.filter(
			([, value]) => value !== null && value !== undefined && value !== "",
		)
		.map(([key, value]): [string, string] => [key, String(value)]);

	return new URLSearchParams(entries);
};

const createFormData = (values: Record<string, FormValue>): FormData => {
	const form = new FormData();

	Object.entries(values).forEach(([key, value]) => {
		if (value === null || value === undefined) {
			return;
		}

		form.append(key, value instanceof File ? value : String(value));
	});

	return form;
};

export const commentsApi = {
	getAll(filters: CommentFilters = { page: 1 }) {
		const params = createQueryParams({
			[QUERY_FIELDS.page]: filters.page,
			[QUERY_FIELDS.sortBy]: filters.sortBy ?? 0,
			[QUERY_FIELDS.direction]: filters.direction ?? 1,
			[QUERY_FIELDS.username]: filters.username,
			[QUERY_FIELDS.email]: filters.email,
		});

		return apiRequest<PagedResponse<Comment>>(
			`${BASE_URL}?${params.toString()}`,
		);
	},

	getById(id: string) {
		return apiRequest<Comment>(`${BASE_URL}/${encodeURIComponent(id)}`);
	},

	getReplies(parentCommentId: string, accessToken: string | null) {
		return apiRequest<Comment[]>(
			`${BASE_URL}/${encodeURIComponent(parentCommentId)}/replies`,
			{
				headers: accessToken ? { Authorization: `Bearer ${accessToken}` } : {},
			},
		);
	},

	create(request: CreateCommentRequest, accessToken: string | null) {
		const form = createFormData({
			[FORM_FIELDS.text]: request.text,
			[FORM_FIELDS.parentCommentId]: request.parentCommentId,
			[FORM_FIELDS.file]: request.file,
			[FORM_FIELDS.captchaId]: request.captchaId,
			[FORM_FIELDS.captchaAnswer]: request.captchaAnswer,
		});

		return apiRequest<Comment>(BASE_URL, {
			method: "POST",
			headers:
				accessToken ?
					{
						Authorization: `Bearer ${accessToken}`,
					}
				:	{},
			body: form,
		});
	},

	update(id: string, request: UpdateCommentRequest) {
		const form = createFormData({
			[FORM_FIELDS.text]: request.text,
			[FORM_FIELDS.file]: request.file,
			[FORM_FIELDS.removeAttachment]: request.removeAttachment,
		});

		return apiRequest<Comment>(`${BASE_URL}/${encodeURIComponent(id)}`, {
			method: "PUT",
			body: form,
		});
	},

	updateRating(
		commentId: string,
		rating: VoteValue,
		accessToken: string,
	): Promise<number> {
		return apiRequest<number>(
			`${BASE_URL}/${encodeURIComponent(commentId)}/rating`,
			{
				method: "PATCH",
				headers: {
					"Content-Type": "application/json",
					Authorization: `Bearer ${accessToken}`,
				},
				body: JSON.stringify({ rating }),
			},
		);
	},

	remove(id: string) {
		return apiRequest<void>(`${BASE_URL}/${encodeURIComponent(id)}`, {
			method: "DELETE",
		});
	},
};
