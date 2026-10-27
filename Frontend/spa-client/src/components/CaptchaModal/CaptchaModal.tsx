import { useEffect, useId, useRef, useState } from "react";
import { createPortal } from "react-dom";

import Captcha from "../Captcha/Captcha";
import { parseApiError } from "../../utils/parseApiError";
import ValidationMessage from "../FormError/FormError";

import styles from "./CaptchaModal.module.css";

interface CaptchaModalProps {
	submitting: boolean;
	onConfirm: (captchaId: string, captchaAnswer: string) => Promise<void>;
	onClose: () => void;
}

export default function CaptchaModal({
	submitting,
	onConfirm,
	onClose,
}: CaptchaModalProps) {
	const titleId = useId();

	const dialogRef = useRef<HTMLDialogElement>(null);
	const closeRef = useRef<HTMLButtonElement>(null);
	const inFlightRef = useRef(false);

	const [captchaId, setCaptchaId] = useState("");
	const [captchaAnswer, setCaptchaAnswer] = useState("");
	const [refreshKey, setRefreshKey] = useState(0);
	const [localSubmitting, setLocalSubmitting] = useState(false);
	const [error, setError] = useState<string | null>(null);

	const busy = submitting || localSubmitting;

	useEffect(() => {
		const dialog = dialogRef.current;

		if (!dialog) return;

		if (!dialog.open) {
			dialog.showModal();
		}

		closeRef.current?.focus();

		return () => {
			if (dialog.open) {
				dialog.close();
			}
		};
	}, []);

	function handleCancel(event: React.SyntheticEvent<HTMLDialogElement>) {
		event.preventDefault();

		if (!busy) {
			onClose();
		}
	}

	async function handleConfirm() {
		if (inFlightRef.current || busy || !captchaId || !captchaAnswer.trim()) {
			return;
		}

		inFlightRef.current = true;
		setLocalSubmitting(true);
		setError(null);

		try {
			await onConfirm(captchaId, captchaAnswer.trim());
		} catch (exception) {
			setError(parseApiError(exception).message);

			setCaptchaId("");
			setCaptchaAnswer("");

			setRefreshKey((value) => value + 1);
		} finally {
			inFlightRef.current = false;
			setLocalSubmitting(false);
		}
	}

	return createPortal(
		<dialog
			ref={dialogRef}
			className={styles.dialogLayer}
			aria-labelledby={titleId}
			onCancel={handleCancel}>
			<div
				className={styles.overlay}
				onMouseDown={(event) => {
					if (event.target === event.currentTarget && !busy) {
						onClose();
					}
				}}>
				<section
					className={styles.modal}
					role="document"
					aria-labelledby={titleId}>
					<header className={styles.header}>
						<div className={styles.heading}>
							<div className={styles.iconBox} aria-hidden="true">
								<svg
									viewBox="0 0 24 24"
									width="20"
									height="20"
									fill="none"
									stroke="currentColor"
									strokeWidth="1.8"
									strokeLinecap="round"
									strokeLinejoin="round">
									<rect x="5" y="10" width="14" height="11" rx="2" />
									<path d="M8 10V7a4 4 0 0 1 8 0v3" />
								</svg>
							</div>

							<div>
								<h2 id={titleId}>Security verification</h2>
								<p>Complete verification to publish your comment</p>
							</div>
						</div>

						<button
							ref={closeRef}
							type="button"
							className={styles.close}
							onClick={onClose}
							disabled={busy}
							aria-label="Close verification">
							×
						</button>
					</header>

					<div className={styles.body}>
						<p className={styles.instructions}>
							Enter the characters shown in the image below.
						</p>

						<div className={styles.captchaSurface}>
							<Captcha
								refreshKey={refreshKey}
								onChange={(id, answer) => {
									setCaptchaId(id);
									setCaptchaAnswer(answer);
								}}
							/>
						</div>

						<ValidationMessage
							variant="banner"
							title="Verification failed"
							message={error}
						/>
					</div>

					<footer className={styles.footer}>
						<button
							type="button"
							className={styles.cancel}
							onClick={onClose}
							disabled={busy}>
							Back to editing
						</button>

						<button
							type="button"
							className={styles.confirm}
							onClick={() => void handleConfirm()}
							disabled={busy || !captchaId || !captchaAnswer.trim()}>
							{busy ? "Publishing..." : "Confirm & post"}
						</button>
					</footer>
				</section>
			</div>
		</dialog>,
		document.body,
	);
}
