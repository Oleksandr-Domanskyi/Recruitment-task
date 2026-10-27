import { useEffect, useMemo } from "react";
import { createPortal } from "react-dom";

import { sanitizeComment } from "../../utils/sanitizeComment";

import styles from "./CommentPreview.module.css";

interface CommentPreviewProps {
	text: string;
	userName: string;
	imageUrl?: string | null;
	fileName?: string | null;
	isReply?: boolean;
	onClose: () => void;
}

const dateFormatter = new Intl.DateTimeFormat(undefined, {
	year: "numeric",
	month: "short",
	day: "numeric",
	hour: "2-digit",
	minute: "2-digit",
});

export default function CommentPreview({
	text,
	userName,
	imageUrl,
	fileName,
	isReply = false,
	onClose,
}: CommentPreviewProps) {
	const sanitizedContent = useMemo(() => sanitizeComment(text), [text]);

	const displayName = userName.trim() || "Anonymous";
	const avatarLetter = displayName.charAt(0).toUpperCase();

	useEffect(() => {
		const previousOverflow = document.body.style.overflow;

		document.body.style.overflow = "hidden";

		function handleKeyDown(event: KeyboardEvent) {
			if (event.key === "Escape") {
				onClose();
			}
		}

		document.addEventListener("keydown", handleKeyDown);

		return () => {
			document.body.style.overflow = previousOverflow;
			document.removeEventListener("keydown", handleKeyDown);
		};
	}, [onClose]);

	return createPortal(
		<div
			className={styles.overlay}
			onMouseDown={(event) => {
				if (event.target === event.currentTarget) {
					onClose();
				}
			}}>
			<section
				className={styles.modal}
				role="dialog"
				aria-modal="true"
				aria-label={isReply ? "Reply preview" : "Comment preview"}>
				<div className={styles.modalHeader}>
					<div className={styles.modalHeading}>
						<span className={styles.previewIndicator} />

						<div>
							<h2>{isReply ? "Reply preview" : "Comment preview"}</h2>

							<p>Preview how your message will appear</p>
						</div>
					</div>

					<button
						type="button"
						className={styles.closeButton}
						onClick={onClose}
						aria-label="Close preview"
						autoFocus>
						<svg
							viewBox="0 0 24 24"
							width="18"
							height="18"
							fill="none"
							stroke="currentColor"
							strokeWidth="1.8"
							strokeLinecap="round"
							aria-hidden="true">
							<path d="M18 6 6 18M6 6l12 12" />
						</svg>
					</button>
				</div>

				<div className={styles.modalBody}>
					<div className={styles.comment}>
						<div className={styles.header}>
							<div className={styles.avatar}>{avatarLetter}</div>

							<div className={styles.author}>
								<strong>{displayName}</strong>
							</div>

							<time className={styles.date}>
								{dateFormatter.format(new Date())}
							</time>
						</div>

						<div
							className={styles.content}
							dangerouslySetInnerHTML={{
								__html: sanitizedContent,
							}}
						/>

						{imageUrl && (
							<div className={styles.attachments}>
								<img src={imageUrl} alt={fileName || "Attachment preview"} />
							</div>
						)}

						{fileName && !imageUrl && (
							<div className={styles.fileAttachment}>
								<svg
									viewBox="0 0 24 24"
									width="17"
									height="17"
									fill="none"
									stroke="currentColor"
									strokeWidth="1.8"
									strokeLinecap="round"
									aria-hidden="true">
									<path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8Z" />
									<path d="M14 2v6h6" />
								</svg>

								<span>{fileName}</span>
							</div>
						)}

						<div className={styles.commentFooter}>
							<span>{isReply ? "Reply preview" : "Comment preview"}</span>
						</div>
					</div>
				</div>

				<div className={styles.modalFooter}>
					<span className={styles.footerHint}>
						This is a preview. Your message hasn't been posted.
					</span>

					<button type="button" className={styles.backButton} onClick={onClose}>
						Back to editing
					</button>
				</div>
			</section>
		</div>,
		document.body,
	);
}
