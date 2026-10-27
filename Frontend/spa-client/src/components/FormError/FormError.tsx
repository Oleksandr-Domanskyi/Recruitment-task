import styles from "./FormError.module.css";

interface ValidationMessageProps {
	message?: string | null;
	title?: string;
	variant?: "field" | "banner";
	id?: string;
}

export default function ValidationMessage({
	message,
	title,
	variant = "field",
	id,
}: ValidationMessageProps) {
	if (!message) return null;

	return (
		<div
			id={id}
			className={`${styles.message} ${variant === "banner" ? styles.banner : styles.field}`}
			role="alert">
			<svg
				className={styles.icon}
				viewBox="0 0 24 24"
				width="18"
				height="18"
				fill="none"
				stroke="currentColor"
				strokeWidth="2"
				strokeLinecap="round"
				strokeLinejoin="round"
				aria-hidden="true">
				<circle cx="12" cy="12" r="10" />
				<path d="M12 8v4M12 16h.01" />
			</svg>
			<div className={styles.content}>
				{title && <strong>{title}</strong>}
				<p>{message}</p>
			</div>
		</div>
	);
}
