import type { CommentAttachment } from "./CommentAttachment";

export interface Comment {
	id: string;
	userId: string | null;
	userName: string | null;
	avatarUrl: string | null;

	text: string;
	attachments: CommentAttachment[];
	rating: number;
	userVote?: -1 | 0 | 1;

	createdAt: string;
	parentCommentId: string | null;
	repliesCount: number;
}
