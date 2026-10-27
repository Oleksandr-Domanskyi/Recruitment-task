export interface CreateCommentRequest {
	text: string;
	parentCommentId: string | null;
	file: File | null;
	captchaId: string;
	captchaAnswer: string;
}

export interface UpdateCommentRequest {
	text: string;
	file: File | null;
	removeAttachment: boolean;
}

export interface UpdateCommentRatingRequest {
	rating: number;
}
