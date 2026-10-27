import {
	useCallback,
	useEffect,
	useRef,
	useState,
	type CSSProperties,
} from "react";

import { commentsApi } from "../../api/commentsApi";

import CommentForm from "../CommentForm/CommentForm";
import ReplyItem from "../ReplyItem/ReplyItem";

import { useCommentCreatedEvent } from "../../context/CommentRealtimeContext";
import { useCommentRatingEvent } from "../../context/CommentRatingRealtimeContext";

import type { Comment } from "../../types/comment";

import styles from "./CommentReplies.module.css";

interface CommentRepliesProps {
	parentCommentId: string;
	visible: boolean;
	refreshKey?: number;
	accessToken: string | null;
	onCountChange: (count: number) => void;
	selectedReplyId: string | null;
	onSelectReply: (id: string | null) => void;
	onReplyCreated: () => void;
}

interface NestedReply {
	comment: Comment;
	parent: Comment | null;
	depth: number;
}

const MAX_INDENT_LEVEL = 4;

function sortReplies(comments: Comment[]): Comment[] {
	return [...comments].sort(
		(a, b) =>
			b.rating - a.rating ||
			new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime() ||
			a.id.localeCompare(b.id),
	);
}

export default function CommentReplies({
	parentCommentId,
	visible,
	refreshKey = 0,
	accessToken,
	onCountChange,
	selectedReplyId,
	onSelectReply,
	onReplyCreated,
}: CommentRepliesProps) {
	const [replies, setReplies] = useState<NestedReply[]>([]);
	const [loading, setLoading] = useState(false);
	const [error, setError] = useState<string | null>(null);
	const [localRefreshKey, setLocalRefreshKey] = useState(0);

	const latestCreatedEvent = useCommentCreatedEvent();
	const latestRatingEvent = useCommentRatingEvent();

	const onCountChangeRef = useRef(onCountChange);
	const realtimeRatingsRef = useRef<Map<string, number>>(new Map());

	useEffect(() => {
		onCountChangeRef.current = onCountChange;
	}, [onCountChange]);

	useEffect(() => {
		realtimeRatingsRef.current.clear();
	}, [parentCommentId]);

	const eventParentId = latestCreatedEvent?.parentCommentId;

	const isEventInThread =
		visible &&
		eventParentId != null &&
		(eventParentId === parentCommentId ||
			replies.some(({ comment }) => comment.id === eventParentId));

	const realtimeRefreshId =
		isEventInThread ? latestCreatedEvent?.commentId : null;

	useEffect(() => {
		if (!latestRatingEvent) return;

		const { commentId, rating } = latestRatingEvent;

		realtimeRatingsRef.current.set(commentId, rating);

		setReplies((current) => {
			const exists = current.some(({ comment }) => comment.id === commentId);

			if (!exists) return current;

			return current.map((reply) =>
				reply.comment.id === commentId ?
					{
						...reply,
						comment: {
							...reply.comment,
							rating,
						},
					}
				:	reply,
			);
		});
	}, [latestRatingEvent]);

	useEffect(() => {
		if (!visible) return;

		let active = true;

		async function loadReplies() {
			setLoading(true);
			setError(null);

			try {
				const result: NestedReply[] = [];

				const visited = new Set<string>([parentCommentId]);

				async function loadChildren(
					commentId: string,
					parent: Comment | null,
					depth: number,
				): Promise<void> {
					if (!active) return;

					const children = await commentsApi.getReplies(commentId, accessToken);

					if (!active) return;

					for (const child of sortReplies(children)) {
						if (!active) return;

						if (visited.has(child.id)) continue;

						visited.add(child.id);

						const updatedRating = realtimeRatingsRef.current.get(child.id);

						const updatedChild: Comment =
							updatedRating !== undefined ?
								{
									...child,
									rating: updatedRating,
								}
							:	child;

						result.push({
							comment: updatedChild,
							parent,
							depth,
						});

						if (child.repliesCount > 0) {
							await loadChildren(child.id, updatedChild, depth + 1);
						}
					}
				}

				await loadChildren(parentCommentId, null, 0);

				if (!active) return;

				setReplies(
					result.map((reply) => {
						const latestRating = realtimeRatingsRef.current.get(
							reply.comment.id,
						);

						return latestRating === undefined ? reply : (
								{
									...reply,
									comment: {
										...reply.comment,
										rating: latestRating,
									},
								}
							);
					}),
				);

				onCountChangeRef.current(result.length);
			} catch (err) {
				if (!active) return;

				setError(
					err instanceof Error ? err.message : "Failed to load replies.",
				);
			} finally {
				if (active) {
					setLoading(false);
				}
			}
		}

		void loadReplies();

		return () => {
			active = false;
		};
	}, [
		parentCommentId,
		visible,
		refreshKey,
		localRefreshKey,
		accessToken,
		realtimeRefreshId,
	]);

	const handleRetry = useCallback(() => {
		setLocalRefreshKey((value) => value + 1);
	}, []);

	if (!visible) return null;

	return (
		<section className={styles.replies} aria-label="Comment replies">
			{loading && (
				<div className={styles.loading} role="status">
					Loading replies...
				</div>
			)}

			{error && (
				<div className={styles.error} role="alert">
					<span>{error}</span>

					<button type="button" onClick={handleRetry}>
						Retry
					</button>
				</div>
			)}

			{!loading && !error && (
				<>
					{replies.map(({ comment, parent, depth }) => {
						const indentLevel = Math.min(depth, MAX_INDENT_LEVEL);

						const nodeStyle = {
							"--reply-indent-level": indentLevel,
						} as CSSProperties;

						return (
							<div
								key={comment.id}
								className={styles.replyNode}
								style={nodeStyle}
								data-depth={depth}>
								<ReplyItem
									key={`${comment.id}-${comment.rating}`}
									comment={comment}
									parent={parent}
									selected={selectedReplyId === comment.id}
									accessToken={accessToken}
									onSelect={onSelectReply}
								/>
							</div>
						);
					})}

					{replies.length === 0 && (
						<p className={styles.empty}>
							No replies yet. Start the conversation.
						</p>
					)}
				</>
			)}

			<div className={styles.replyFormContainer}>
				{selectedReplyId ?
					<div className={styles.replyForm}>
						<div className={styles.formHeader}>
							<span>Write a reply</span>

							<button
								type="button"
								className={styles.cancelButton}
								onClick={() => onSelectReply(null)}>
								Cancel
							</button>
						</div>

						<CommentForm
							parentCommentId={selectedReplyId}
							onCreated={onReplyCreated}
						/>
					</div>
				:	<button
						type="button"
						className={styles.replyPlaceholder}
						onClick={() => onSelectReply(parentCommentId)}>
						Write a reply...
					</button>
				}
			</div>
		</section>
	);
}
