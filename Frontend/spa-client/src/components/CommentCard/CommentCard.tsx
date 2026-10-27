import {
	useCallback,
	useEffect,
	useLayoutEffect,
	useMemo,
	useRef,
	useState,
} from "react";

import { commentsApi } from "../../api/commentsApi";

import CommentReplies from "../CommentReplies/CommentReplies";

import { useCommentCreatedEvent } from "../../context/CommentRealtimeContext";
import { sanitizeComment } from "../../utils/sanitizeComment";

import type { Comment } from "../../types/comment";

import styles from "./CommentCard.module.css";

interface CommentCardProps {
	comment: Comment;
	accessToken: string | null;
	initiallyExpanded?: boolean;
}

const dateFormatter = new Intl.DateTimeFormat(undefined, {
	year: "numeric",
	month: "short",
	day: "numeric",
	hour: "2-digit",
	minute: "2-digit",
});

export default function CommentCard({
	comment,
	accessToken,
	initiallyExpanded = false,
}: CommentCardProps) {
	const [showReplies, setShowReplies] = useState(initiallyExpanded);
	const [repliesCount, setRepliesCount] = useState(comment.repliesCount);
	const [refreshKey, setRefreshKey] = useState(0);

	const [selectedReplyId, setSelectedReplyId] = useState<string | null>(null);

	const [isExpanded, setIsExpanded] = useState(false);
	const [isOverflowing, setIsOverflowing] = useState(false);

	const [failedAvatarUrl, setFailedAvatarUrl] = useState<string | null>(null);

	const contentRef = useRef<HTMLDivElement>(null);

	const latestCreatedEvent = useCommentCreatedEvent();

	const displayName = comment.userName?.trim() || "Anonymous";
	const avatarLetter = displayName.charAt(0).toUpperCase();

	const avatarUrl = comment.avatarUrl;
	const showAvatarImage = Boolean(avatarUrl && avatarUrl !== failedAvatarUrl);

	const attachments = comment.attachments ?? [];
	const hasReplies = repliesCount > 0;

	const sanitizedContent = useMemo(
		() => sanitizeComment(comment.text),
		[comment.text],
	);

	useEffect(() => {
		setRepliesCount(comment.repliesCount);
	}, [comment.id, comment.repliesCount]);

	useEffect(() => {
		setShowReplies(initiallyExpanded);
		setSelectedReplyId(null);
	}, [comment.id, initiallyExpanded]);

	useEffect(() => {
		setIsExpanded(false);
	}, [comment.id, comment.text]);

	useEffect(() => {
		if (!latestCreatedEvent) return;

		if (latestCreatedEvent.parentCommentId !== comment.id) {
			return;
		}

		if (showReplies) {
			return;
		}

		let active = true;

		async function updateRepliesCount() {
			try {
				const updatedComment = await commentsApi.getById(comment.id);

				if (!active) return;

				setRepliesCount((current) =>
					Math.max(current, updatedComment.repliesCount),
				);
			} catch (error) {
				if (!active) return;

				console.error("Failed to update replies count:", error);
			}
		}

		void updateRepliesCount();

		return () => {
			active = false;
		};
	}, [comment.id, showReplies, latestCreatedEvent]);

	useLayoutEffect(() => {
		const element = contentRef.current;

		if (!element) return;

		const checkOverflow = (target: HTMLDivElement) => {
			const wasExpanded = target.classList.contains(styles.contentExpanded);

			if (wasExpanded) {
				target.classList.remove(styles.contentExpanded);
				target.classList.add(styles.contentCollapsed);
			}

			const overflowing = target.scrollHeight > target.clientHeight + 1;

			if (wasExpanded) {
				target.classList.remove(styles.contentCollapsed);
				target.classList.add(styles.contentExpanded);
			}

			setIsOverflowing((previous) =>
				previous === overflowing ? previous : overflowing,
			);
		};

		checkOverflow(element);

		const observer = new ResizeObserver(() => {
			checkOverflow(element);
		});

		observer.observe(element);

		return () => observer.disconnect();
	}, [sanitizedContent]);

	const handleCountChange = useCallback((count: number) => {
		setRepliesCount(count);
	}, []);

	function toggleReplies() {
		if (!hasReplies) return;

		setShowReplies((previous) => !previous);
	}

	function handleMainReply() {
		if (selectedReplyId === comment.id && showReplies) {
			setSelectedReplyId(null);
			return;
		}

		setSelectedReplyId(comment.id);
		setShowReplies(true);
	}

	function handleReplyCreated() {
		setSelectedReplyId(null);
		setShowReplies(true);
		setRefreshKey((previous) => previous + 1);
	}

	return (
		<article className={styles.card}>
			<div
				className={`${styles.main} ${hasReplies ? styles.expandable : ""}`}
				role={hasReplies ? "button" : undefined}
				tabIndex={hasReplies ? 0 : undefined}
				aria-expanded={hasReplies ? showReplies : undefined}
				onClick={(event) => {
					const target = event.target;

					if (
						target instanceof Element &&
						target.closest("a, button, input, textarea, select")
					) {
						return;
					}

					toggleReplies();
				}}
				onKeyDown={(event) => {
					if (
						hasReplies &&
						event.target === event.currentTarget &&
						(event.key === "Enter" || event.key === " ")
					) {
						event.preventDefault();
						toggleReplies();
					}
				}}>
				<div className={styles.header}>
					<div className={styles.avatar}>
						{showAvatarImage ?
							<img
								src={avatarUrl!}
								alt=""
								className={styles.avatarImage}
								loading="lazy"
								onError={() => {
									setFailedAvatarUrl(avatarUrl ?? null);
								}}
							/>
						:	<span className={styles.avatarLetter}>{avatarLetter}</span>}
					</div>

					<div className={styles.author}>
						<strong>{displayName}</strong>
					</div>

					<time className={styles.date} dateTime={comment.createdAt}>
						{dateFormatter.format(new Date(comment.createdAt))}
					</time>
				</div>

				<div
					ref={contentRef}
					className={`${styles.content} ${
						isExpanded ? styles.contentExpanded : styles.contentCollapsed
					}`}
					dangerouslySetInnerHTML={{
						__html: sanitizedContent,
					}}
				/>

				{isOverflowing && (
					<button
						type="button"
						className={styles.showMore}
						aria-expanded={isExpanded}
						onClick={(event) => {
							event.stopPropagation();

							setIsExpanded((previous) => !previous);
						}}>
						{isExpanded ? "Show less" : "Show more"}
					</button>
				)}

				{attachments.length > 0 && (
					<div
						className={styles.attachments}
						onClick={(event) => event.stopPropagation()}>
						{attachments.map((attachment) => (
							<div key={attachment.id} className={styles.attachment}>
								{attachment.contentType.startsWith("image/") ?
									<img
										src={attachment.url}
										alt={attachment.originalFileName}
										loading="lazy"
									/>
								:	<a
										href={attachment.url}
										target="_blank"
										rel="noopener noreferrer">
										{attachment.originalFileName}
									</a>
								}
							</div>
						))}
					</div>
				)}

				<div className={styles.actions}>
					<div className={styles.repliesCount}>
						{hasReplies && (
							<span>
								{showReplies ? "Hide replies" : `Replies (${repliesCount})`}
							</span>
						)}
					</div>

					<div
						className={styles.buttons}
						onClick={(event) => event.stopPropagation()}>
						<button
							type="button"
							aria-expanded={showReplies && selectedReplyId === comment.id}
							onClick={handleMainReply}>
							{showReplies && selectedReplyId === comment.id ?
								"Cancel"
							:	"Reply"}
						</button>
					</div>
				</div>
			</div>

			<CommentReplies
				parentCommentId={comment.id}
				visible={showReplies}
				refreshKey={refreshKey}
				accessToken={accessToken}
				onCountChange={handleCountChange}
				selectedReplyId={selectedReplyId}
				onSelectReply={setSelectedReplyId}
				onReplyCreated={handleReplyCreated}
			/>
		</article>
	);
}
