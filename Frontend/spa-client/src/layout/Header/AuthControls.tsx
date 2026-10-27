import { useState } from "react";

import { useAuth } from "../../hooks/useAuth";
import AuthModal from "../../features/auth/AuthModal";
import UserMenu from "./UserMenu";

import styles from "./AuthControls.module.css";

interface AuthControlsProps {
	className?: string;
}

export default function AuthControls({ className }: AuthControlsProps) {
	const { accessToken, user, isAuthenticated, isLoading, logout } = useAuth();

	const [isModalOpen, setIsModalOpen] = useState(false);
	const [isLoggingOut, setIsLoggingOut] = useState(false);
	const [error, setError] = useState<string | null>(null);

	async function handleLogout() {
		if (isLoggingOut) return;

		setIsLoggingOut(true);
		setError(null);

		try {
			await logout();
		} catch {
			setError("Failed to sign out.");
		} finally {
			setIsLoggingOut(false);
		}
	}

	return (
		<div className={`${styles.controls} ${className ?? ""}`}>
			{isLoading ?
				<span className={styles.loading}>Loading...</span>
			: isAuthenticated && accessToken ?
				<div className={styles.authenticated}>
					<UserMenu
						accessToken={accessToken}
						userName={user?.userName ?? "User"}
					/>

					<button
						type="button"
						className={styles.logout}
						onClick={() => void handleLogout()}
						disabled={isLoggingOut}
						title="Sign out">
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
							<path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
							<path d="m16 17 5-5-5-5" />
							<path d="M21 12H9" />
						</svg>

						<span>{isLoggingOut ? "Signing out..." : "Logout"}</span>
					</button>
				</div>
			:	<button
					type="button"
					className={styles.login}
					onClick={() => setIsModalOpen(true)}>
					Sign in / Register
				</button>
			}

			{error && (
				<p className={styles.error} role="alert">
					{error}
				</p>
			)}

			{isModalOpen && <AuthModal onClose={() => setIsModalOpen(false)} />}
		</div>
	);
}
