import { useEffect, useId, useRef, useState } from "react";
import type { ChangeEvent, FormEvent } from "react";

import { commentsApi } from "../../api/commentsApi";
import { useAuth } from "../../hooks/useAuth";
import { isCaptchaApiError, parseApiError } from "../../utils/parseApiError";

import CommentPreview from "../CommentPreview/CommentPreview";
import CaptchaModal from "../CaptchaModal/CaptchaModal";
import ValidationMessage from "../FormError/FormError";

import validationStyles from "../FormError/FormError.module.css";
import styles from "./CommentForm.module.css";

interface CommentFormProps {
	parentCommentId?: string | null;
	onCreated?: () => void;
	userName?: string;
}

interface FormErrors {
	text?: string;
	file?: string;
	general?: string;
}

const ALLOWED_EXTENSIONS = new Set(["jpg", "jpeg", "png", "gif", "txt"]);
const MAX_TXT_BYTES = 100 * 1024;

function validateFile(file: File | null): string | undefined {
	if (!file) return undefined;

	const extension = file.name.split(".").pop()?.toLowerCase() ?? "";

	if (!ALLOWED_EXTENSIONS.has(extension)) {
		return "Unsupported attachment. Allowed formats: JPG, PNG, GIF, TXT.";
	}

	if (extension === "txt" && file.size > MAX_TXT_BYTES) {
		return "Text file must not exceed 100 KB.";
	}

	// Do not invent an image file-size limit here: the backend owns that rule.
	return undefined;
}

function mapFormErrors(error: unknown): FormErrors {
	const problem = parseApiError(error);
	const result: FormErrors = {};
	const generic: string[] = [];

	for (const [key, messages] of Object.entries(problem.fields)) {
		const normalized = key.toLowerCase().replace(/[^a-z]/g, "");

		for (const message of messages) {
			if (
				normalized === "text" ||
				normalized.endsWith("commenttext") ||
				normalized === "comment"
			) {
				result.text = [result.text, message].filter(Boolean).join("\n");
			} else if (
				normalized === "file" ||
				normalized.startsWith("attachment") ||
				normalized.startsWith("image")
			) {
				result.file = [result.file, message].filter(Boolean).join("\n");
			} else if (normalized === "general" || normalized === "") {
				// Best-effort placement for legacy ValidationException("...") messages.
				if (/\b(file|image|attachment)\b/i.test(message)) {
					result.file = [result.file, message].filter(Boolean).join("\n");
				} else if (/\b(xhtml|html|markup|comment text)\b/i.test(message)) {
					result.text = [result.text, message].filter(Boolean).join("\n");
				} else {
					generic.push(message);
				}
			} else {
				generic.push(message);
			}
		}
	}

	if (generic.length > 0) {
		result.general = [...new Set(generic)].join("\n");
	} else if (!result.text && !result.file) {
		result.general = problem.message;
	}

	return result;
}

export default function CommentForm({
	parentCommentId = null,
	onCreated,
	userName = "Anonymous",
}: CommentFormProps) {
	const { accessToken, isLoading } = useAuth();

	const id = useId();
	const textareaRef = useRef<HTMLTextAreaElement>(null);
	const fileInputRef = useRef<HTMLInputElement>(null);

	const [text, setText] = useState("");
	const [file, setFile] = useState<File | null>(null);
	const [filePreviewUrl, setFilePreviewUrl] = useState<string | null>(null);

	const [showPreview, setShowPreview] = useState(false);
	const [showCaptcha, setShowCaptcha] = useState(false);
	const [submitting, setSubmitting] = useState(false);
	const [formErrors, setFormErrors] = useState<FormErrors>({});

	const isReply = Boolean(parentCommentId);

	useEffect(() => {
		if (!file || !file.type.startsWith("image/")) {
			setFilePreviewUrl(null);
			return;
		}

		const url = URL.createObjectURL(file);
		setFilePreviewUrl(url);

		return () => URL.revokeObjectURL(url);
	}, [file]);

	function handleFileChange(event: ChangeEvent<HTMLInputElement>) {
		const selected = event.target.files?.[0] ?? null;
		setFile(selected);
		setFormErrors((current) => ({
			...current,
			file: validateFile(selected),
			general: undefined,
		}));
	}

	function removeFile() {
		setFile(null);
		setFormErrors((current) => ({
			...current,
			file: undefined,
			general: undefined,
		}));
		if (fileInputRef.current) fileInputRef.current.value = "";
	}

	function insertFormatting(before: string, after = before) {
		const textarea = textareaRef.current;
		if (!textarea || submitting) return;

		const start = textarea.selectionStart;
		const end = textarea.selectionEnd;
		const selectedText = text.slice(start, end);
		const nextText =
			text.slice(0, start) + before + selectedText + after + text.slice(end);

		setText(nextText);
		setFormErrors((current) => ({
			...current,
			text: undefined,
			general: undefined,
		}));

		requestAnimationFrame(() => {
			textarea.focus();
			textarea.setSelectionRange(start + before.length, end + before.length);
		});
	}

	function handlePostClick(event: FormEvent<HTMLFormElement>) {
		event.preventDefault();
		if (submitting || isLoading) return;

		if (!text.trim()) {
			setFormErrors({ text: "Comment text is required." });
			textareaRef.current?.focus();
			return;
		}

		const fileError = validateFile(file);
		if (fileError) {
			setFormErrors({ file: fileError });
			return;
		}

		setFormErrors({});
		setShowCaptcha(true);
	}

	async function handleCaptchaConfirm(
		captchaId: string,
		captchaAnswer: string,
	) {
		if (submitting || isLoading) return;

		setSubmitting(true);
		setFormErrors({});

		try {
			await commentsApi.create(
				{
					text: text.trim(),
					parentCommentId,
					file,
					captchaId,
					captchaAnswer: captchaAnswer.trim(),
				},
				accessToken,
			);

			setShowCaptcha(false);
			setShowPreview(false);
			setText("");
			removeFile();
			onCreated?.();
		} catch (error) {
			const parsed = parseApiError(error);

			if (isCaptchaApiError(parsed)) {
				// CAPTCHA-specific failure stays in CAPTCHA modal.
				throw error;
			}

			// Text, attachment, other validation and server errors belong to the form.
			setFormErrors(mapFormErrors(error));
			setShowCaptcha(false);
		} finally {
			setSubmitting(false);
		}
	}

	return (
		<form className={styles.form} onSubmit={handlePostClick}>
			<div className={styles.editor}>
				<label className={styles.editorLabel} htmlFor={`${id}-text`}>
					{isReply ? "Your reply" : "Your comment"}
				</label>

				<div
					className={`${styles.editorSurface} ${formErrors.text ? validationStyles.invalidSurface : ""}`}>
					<textarea
						ref={textareaRef}
						id={`${id}-text`}
						className={styles.textarea}
						value={text}
						onChange={(event) => {
							setText(event.target.value);
							setFormErrors((current) => ({
								...current,
								text: undefined,
								general: undefined,
							}));
						}}
						placeholder={
							isReply ? "Write your reply..." : "Write your comment..."
						}
						rows={4}
						required
						disabled={submitting}
						aria-invalid={Boolean(formErrors.text)}
						aria-describedby={formErrors.text ? `${id}-text-error` : undefined}
					/>

					<div className={styles.editorToolbar}>
						<div
							className={styles.formatButtons}
							role="group"
							aria-label="Text formatting">
							<button
								type="button"
								title="Bold"
								disabled={submitting}
								onClick={() => insertFormatting("<strong>", "</strong>")}>
								<strong>B</strong>
							</button>
							<button
								type="button"
								title="Italic"
								disabled={submitting}
								onClick={() => insertFormatting("<i>", "</i>")}>
								<em>I</em>
							</button>
							<button
								type="button"
								title="Insert link"
								disabled={submitting}
								onClick={() =>
									insertFormatting('<a href="https://example.com">', "</a>")
								}>
								<svg
									viewBox="0 0 24 24"
									width="17"
									height="17"
									fill="none"
									stroke="currentColor"
									strokeWidth="1.8"
									strokeLinecap="round"
									strokeLinejoin="round"
									aria-hidden="true">
									<path d="M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71" />
									<path d="M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71" />
								</svg>
							</button>
							<button
								type="button"
								title="Code"
								disabled={submitting}
								onClick={() => insertFormatting("<code>", "</code>")}>
								<span className={styles.codeIcon}>{"</>"}</span>
							</button>
						</div>
						<span className={styles.characterCount}>
							{text.length} characters
						</span>
					</div>
				</div>

				<ValidationMessage id={`${id}-text-error`} message={formErrors.text} />
			</div>

			<div
				className={`${styles.attachmentSection} ${formErrors.file ? validationStyles.invalidSurface : ""}`}>
				<input
					ref={fileInputRef}
					id={`${id}-file`}
					className={styles.fileInput}
					type="file"
					accept=".jpg,.jpeg,.png,.gif,.txt"
					onChange={handleFileChange}
					disabled={submitting}
					aria-invalid={Boolean(formErrors.file)}
					aria-describedby={formErrors.file ? `${id}-file-error` : undefined}
				/>

				<div className={styles.attachmentRow}>
					<label
						htmlFor={`${id}-file`}
						className={`${styles.attachmentButton} ${submitting ? styles.attachmentDisabled : ""}`}>
						<svg
							viewBox="0 0 24 24"
							width="17"
							height="17"
							fill="none"
							stroke="currentColor"
							strokeWidth="1.8"
							strokeLinecap="round"
							strokeLinejoin="round"
							aria-hidden="true">
							<path d="m21.44 11.05-9.19 9.19a6 6 0 0 1-8.49-8.49l9.19-9.19a4 4 0 0 1 5.66 5.66l-9.2 9.19a2 2 0 0 1-2.82-2.83l8.49-8.48" />
						</svg>
						<span>Add attachment</span>
					</label>
					<span className={styles.fileHint}>
						PNG, JPG, GIF, TXT (TXT max 100 KB)
					</span>
				</div>

				{file && (
					<div className={styles.selectedFile}>
						<span className={styles.fileName} title={file.name}>
							{file.name}
						</span>
						<span className={styles.fileSize}>
							{(file.size / 1024).toFixed(1)} KB
						</span>
						<button
							type="button"
							className={styles.removeFile}
							onClick={removeFile}
							disabled={submitting}
							aria-label="Remove attachment">
							×
						</button>
					</div>
				)}

				<ValidationMessage id={`${id}-file-error`} message={formErrors.file} />
			</div>

			<ValidationMessage
				variant="banner"
				title="Unable to publish comment"
				message={formErrors.general}
			/>

			<div className={styles.footer}>
				<span className={styles.footerHint}>
					{isReply ?
						"Your reply will appear in this thread"
					:	"Share your thoughts with the community"}
				</span>

				<div className={styles.footerActions}>
					<button
						type="button"
						className={styles.previewButton}
						onClick={() => setShowPreview(true)}
						disabled={!text.trim() || submitting}>
						<svg
							viewBox="0 0 24 24"
							width="17"
							height="17"
							fill="none"
							stroke="currentColor"
							strokeWidth="1.8"
							strokeLinecap="round"
							strokeLinejoin="round"
							aria-hidden="true">
							<path d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7S2 12 2 12Z" />
							<circle cx="12" cy="12" r="3" />
						</svg>
						Preview
					</button>
					<button
						type="submit"
						className={styles.submit}
						disabled={submitting || isLoading || !text.trim()}>
						{isReply ? "Post reply" : "Post comment"}
					</button>
				</div>
			</div>

			{showPreview && (
				<CommentPreview
					text={text}
					userName={userName}
					imageUrl={filePreviewUrl}
					fileName={file?.name ?? null}
					isReply={isReply}
					onClose={() => setShowPreview(false)}
				/>
			)}

			{showCaptcha && (
				<CaptchaModal
					submitting={submitting}
					onConfirm={handleCaptchaConfirm}
					onClose={() => {
						if (!submitting) setShowCaptcha(false);
					}}
				/>
			)}
		</form>
	);
}
