import AuthControls from "./AuthControls";

import styles from "./Header.module.css";

export default function Header() {
	return (
		<header className={styles.header}>
			<div className={styles.container}>
				<div className={styles.brand}>
					<div className={styles.logo}>
						<svg
							viewBox="0 0 24 24"
							width="22"
							height="22"
							fill="none"
							stroke="currentColor"
							strokeWidth="1.8"
							strokeLinecap="round"
							strokeLinejoin="round"
							aria-hidden="true">
							<path d="M21 11.5a8.5 8.5 0 0 1-8.5 8.5 9 9 0 0 1-4-.9L3 21l1.9-5.5a9 9 0 0 1-.9-4A8.5 8.5 0 0 1 12.5 3H13a8 8 0 0 1 8 8v.5Z" />
							<path d="M8 10h8M8 14h5" />
						</svg>
					</div>

					<div className={styles.brandText}>
						<span className={styles.title}>Comments</span>

						<span className={styles.subtitle}>Community platform</span>
					</div>
				</div>

				<AuthControls className={styles.authControls} />
			</div>
		</header>
	);
}
