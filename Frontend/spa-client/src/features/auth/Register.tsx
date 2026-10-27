import { useState, type FormEvent } from "react";

import { authApi } from "../../api/authApi";

import styles from "./AuthForm.module.css";

interface RegisterProps {
	onSuccess?: () => void;
	onLogin?: () => void;
}

export default function Register({ onSuccess, onLogin }: RegisterProps) {
	const [userName, setUserName] = useState("");
	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const [confirmPassword, setConfirmPassword] = useState("");

	const [loading, setLoading] = useState(false);
	const [error, setError] = useState<string | null>(null);

	async function handleSubmit(event: FormEvent<HTMLFormElement>) {
		event.preventDefault();

		if (loading) return;

		if (password !== confirmPassword) {
			setError("Passwords do not match.");
			return;
		}

		setLoading(true);
		setError(null);

		try {
			await authApi.register({
				userName: userName.trim(),
				email: email.trim(),
				password,
			});

			onSuccess?.();
		} catch (error) {
			setError(error instanceof Error ? error.message : "Registration failed.");
		} finally {
			setLoading(false);
		}
	}

	return (
		<form className={styles.form} onSubmit={handleSubmit}>
			<div className={styles.heading}>
				<h2>Create account</h2>
				<p>Join the discussion.</p>
			</div>

			<div className={styles.field}>
				<label htmlFor="register-username">Username</label>
				<input
					id="register-username"
					type="text"
					autoComplete="username"
					value={userName}
					onChange={(event) => setUserName(event.target.value)}
					placeholder="Enter your username"
					required
					minLength={3}
					maxLength={100}
				/>
			</div>

			<div className={styles.field}>
				<label htmlFor="register-email">Email</label>
				<input
					id="register-email"
					type="email"
					autoComplete="email"
					value={email}
					onChange={(event) => setEmail(event.target.value)}
					placeholder="Enter your email"
					required
				/>
			</div>

			<div className={styles.field}>
				<label htmlFor="register-password">Password</label>
				<input
					id="register-password"
					type="password"
					autoComplete="new-password"
					value={password}
					onChange={(event) => setPassword(event.target.value)}
					placeholder="Create a password"
					required
				/>
			</div>

			<div className={styles.field}>
				<label htmlFor="register-confirm-password">Confirm password</label>
				<input
					id="register-confirm-password"
					type="password"
					autoComplete="new-password"
					value={confirmPassword}
					onChange={(event) => setConfirmPassword(event.target.value)}
					placeholder="Repeat your password"
					required
				/>
			</div>

			{error && (
				<p className={styles.error} role="alert">
					{error}
				</p>
			)}

			<button className={styles.submitButton} type="submit" disabled={loading}>
				{loading ? "Creating account..." : "Register"}
			</button>

			{onLogin && (
				<p className={styles.switchText}>
					Already have an account?{" "}
					<button type="button" onClick={onLogin}>
						Sign in
					</button>
				</p>
			)}
		</form>
	);
}
