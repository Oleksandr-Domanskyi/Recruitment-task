import { useEffect, useState } from "react";

import { captchaApi } from "../../api/captchaApi";
import type { CaptchaResponse } from "../../types/captcha";

import styles from "./Captcha.module.css";

interface CaptchaProps {
	onChange: (captchaId: string, captchaAnswer: string) => void;
	refreshKey?: number;
}

export default function Captcha({ onChange, refreshKey = 0 }: CaptchaProps) {
	const [captcha, setCaptcha] = useState<CaptchaResponse | null>(null);
	const [answer, setAnswer] = useState("");
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);
	const [reload, setReload] = useState(0);

	useEffect(() => {
		let active = true;

		async function loadCaptcha() {
			setLoading(true);
			setError(null);
			setCaptcha(null);
			setAnswer("");
			onChange("", "");

			try {
				const response = await captchaApi.get();

				if (active) {
					setCaptcha(response);
					onChange(response.captchaId, "");
				}
			} catch (error) {
				if (active) {
					setError(
						error instanceof Error ? error.message : "Failed to load CAPTCHA.",
					);
				}
			} finally {
				if (active) {
					setLoading(false);
				}
			}
		}

		void loadCaptcha();

		return () => {
			active = false;
		};
	}, [refreshKey, reload]);

	function handleAnswerChange(value: string) {
		setAnswer(value);
		onChange(captcha?.captchaId ?? "", value);
	}

	return (
		<div className={styles.container}>
			<label className={styles.label} htmlFor="captcha-answer">
				CAPTCHA
			</label>

			<div className={styles.captchaRow}>
				{loading ?
					<span className={styles.message}>Loading CAPTCHA...</span>
				: captcha ?
					<img
						className={styles.image}
						src={`data:image/png;base64,${captcha.imageBase64}`}
						alt="CAPTCHA verification code"
					/>
				:	<span className={styles.error}>{error ?? "CAPTCHA unavailable."}</span>
				}

				<button
					type="button"
					className={styles.refresh}
					onClick={() => setReload((value) => value + 1)}
					disabled={loading}>
					Refresh
				</button>
			</div>

			<input
				id="captcha-answer"
				className={styles.input}
				type="text"
				value={answer}
				onChange={(event) => handleAnswerChange(event.target.value)}
				placeholder="Enter the code"
				autoComplete="off"
				required
				disabled={!captcha || loading}
			/>
		</div>
	);
}
