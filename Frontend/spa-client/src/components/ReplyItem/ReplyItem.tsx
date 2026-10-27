import { useEffect, useMemo, useState } from "react";

import { commentsApi, type VoteValue } from "../../api/commentsApi";
import { sanitizeComment } from "../../utils/sanitizeComment";

import type { Comment } from "../../types/comment";

import styles from "./ReplyItem.module.css";

interface ReplyItemProps {
	comment: Comment;
	parent: Comment | null;
	selected: boolean;
	accessToken: string | null;
	onSelect: (commentId: string) => void;
}

const dateFormatter = new Intl.DateTimeFormat(undefined, {
	day: "numeric",
	month: "short",
	hour: "2-digit",
	minute: "2-digit",
});

export default function ReplyItem({
	comment,
	parent,
	selected,
	accessToken,
	onSelect,
}: ReplyItemProps) {
	const [rating, setRating] = useState(comment.rating);
	const [userVote, setUserVote] = useState<VoteValue>(comment.userVote ?? 0);
	const [ratingLoading, setRatingLoading] = useState(false);
	const [ratingError, setRatingError] = useState<string | null>(null);

	const [failedAvatarUrl, setFailedAvatarUrl] = useState<string | null>(null);

	const displayName = comment.userName?.trim() || "Anonymous";
	const parentName = parent?.userName?.trim() || "Anonymous";

	const avatarLetter = displayName.charAt(0).toUpperCase();
	const avatarUrl = comment.avatarUrl;

	const showAvatarImage = Boolean(avatarUrl && avatarUrl !== failedAvatarUrl);

	const attachments = comment.attachments ?? [];

	const sanitizedContent = useMemo(
		() => sanitizeComment(comment.text),
		[comment.text],
	);

	const sanitizedParentContent = useMemo(
		() => (parent ? sanitizeComment(parent.text) : ""),
		[parent?.text],
	);

	useEffect(() => {
		setRating(comment.rating);
		setUserVote(comment.userVote ?? 0);
		setRatingError(null);
	}, [comment.id, comment.rating, comment.userVote, accessToken]);

	async function handleRatingChange(delta: 1 | -1) {
		if (ratingLoading) return;

		setRatingError(null);

		if (!accessToken) {
			setRatingError("You must be logged in to vote.");
			return;
		}

		const newVote: VoteValue = userVote === delta ? 0 : delta;

		setRatingLoading(true);

		try {
			const updatedRating = await commentsApi.updateRating(
				comment.id,
				newVote,
				accessToken,
			);

			setRating(updatedRating);
			setUserVote(newVote);
		} catch (error) {
			setRatingError(
				error instanceof Error ? error.message : "Failed to update rating.",
			);
		} finally {
			setRatingLoading(false);
		}
	}

	return (
		<article className={`${styles.reply} ${selected ? styles.selected : ""}`}>
			<div className={styles.header}>
				<button
					type="button"
					className={styles.authorInfo}
					onClick={() => onSelect(comment.id)}
					aria-pressed={selected}
					aria-label={`Reply to ${displayName}`}>
					<span className={styles.avatar}>
						{showAvatarImage ?
							<img
								src={avatarUrl!}
								alt=""
								className={styles.avatarImage}
								loading="lazy"
								onError={() => setFailedAvatarUrl(avatarUrl ?? null)}
							/>
						:	<span className={styles.avatarLetter}>{avatarLetter}</span>}
					</span>

					<span className={styles.author}>
						<strong>{displayName}</strong>

						<time dateTime={comment.createdAt}>
							{dateFormatter.format(new Date(comment.createdAt))}
						</time>
					</span>
				</button>

				<div className={styles.ratingControls}>
					<button
						type="button"
						disabled={ratingLoading}
						aria-label="Increase rating"
						aria-pressed={userVote === 1}
						className={userVote === 1 ? styles.activeVote : ""}
						onClick={() => void handleRatingChange(1)}>
						↑
					</button>

					<span>{rating}</span>

					<button
						type="button"
						disabled={ratingLoading}
						aria-label="Decrease rating"
						aria-pressed={userVote === -1}
						className={userVote === -1 ? styles.activeVote : ""}
						onClick={() => void handleRatingChange(-1)}>
						↓
					</button>
				</div>
			</div>

			<button
				type="button"
				className={styles.body}
				onClick={() => onSelect(comment.id)}
				aria-pressed={selected}>
				{parent && (
					<span className={styles.quote}>
						<span className={styles.quoteAuthor}>Reply to {parentName}</span>

						<span
							className={styles.quoteText}
							dangerouslySetInnerHTML={{
								__html: sanitizedParentContent,
							}}
						/>
					</span>
				)}

				<span
					className={styles.content}
					dangerouslySetInnerHTML={{
						__html: sanitizedContent,
					}}
				/>
			</button>

			{attachments.length > 0 && (
				<div className={styles.attachments}>
					{attachments.map((attachment) =>
						attachment.contentType.startsWith("image/") ?
							<img
								key={attachment.id}
								src={attachment.url}
								alt={attachment.originalFileName}
								loading="lazy"
							/>
						:	<a
								key={attachment.id}
								href={attachment.url}
								target="_blank"
								rel="noopener noreferrer">
								{attachment.originalFileName}
							</a>,
					)}
				</div>
			)}

			{ratingError && (
				<p className={styles.ratingError} role="alert">
					{ratingError}
				</p>
			)}
		</article>
	);
}
