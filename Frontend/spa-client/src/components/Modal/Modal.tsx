import { useEffect, useId, useRef } from "react";
import { createPortal } from "react-dom";
import type { ReactNode, MouseEvent } from "react";

import styles from "./Modal.module.css";

interface ModalProps {
	children: ReactNode;
	onClose: () => void;
	title: string;
}

export default function Modal({ children, onClose, title }: ModalProps) {
	const dialogRef = useRef<HTMLDialogElement>(null);
	const titleId = useId();

	useEffect(() => {
		const dialog = dialogRef.current;

		if (!dialog) return;

		if (!dialog.open) {
			dialog.showModal();
		}

		return () => {
			if (dialog.open) {
				dialog.close();
			}
		};
	}, []);

	function handleBackdropClick(event: MouseEvent<HTMLDialogElement>) {
		if (event.target === event.currentTarget) {
			onClose();
		}
	}

	function handleCancel(event: React.SyntheticEvent<HTMLDialogElement>) {
		event.preventDefault();
		onClose();
	}

	return createPortal(
		<dialog
			ref={dialogRef}
			className={styles.modal}
			aria-labelledby={titleId}
			onCancel={handleCancel}
			onClick={handleBackdropClick}>
			<div className={styles.content}>
				<div className={styles.header}>
					<h2 id={titleId}>{title}</h2>

					<button
						type="button"
						className={styles.close}
						onClick={onClose}
						aria-label="Close dialog">
						×
					</button>
				</div>

				{children}
			</div>
		</dialog>,
		document.body,
	);
}
