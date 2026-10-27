import { useState, type FormEvent } from "react";

import { useAuth } from "../../hooks/useAuth";
import { ApiError } from "../../api/client";

import styles from "./AuthForm.module.css";

interface LoginProps {
	onSuccess?: () => void;
	onRegister?: () => void;
}

export default function Login({ onSuccess, onRegister }: LoginProps) {
	const { login } = useAuth();

	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");

	const [loading, setLoading] = useState(false);
	const [error, setError] = useState<string | null>(null);

	async function handleSubmit(event: FormEvent<HTMLFormElement>) {
		event.preventDefault();

		if (loading) return;

		setLoading(true);
		setError(null);

		try {
			await login({
				email: email.trim(),
				password,
			});

			onSuccess?.();
		} catch (error) {
			if (error instanceof ApiError && error.status === 401) {
				setError("Invalid email or password.");
			} else {
				setError(error instanceof Error ? error.message : "Failed to sign in.");
			}
		} finally {
			setLoading(false);
		}
	}

	return (
		<form className={styles.form} onSubmit={handleSubmit}>
			<div className={styles.heading}>
				<h2>Welcome back</h2>
				<p>Sign in to your account.</p>
			</div>

			<div className={styles.field}>
				<label htmlFor="login-email">Email</label>
				<input
					id="login-email"
					type="email"
					autoComplete="email"
					value={email}
					onChange={(event) => setEmail(event.target.value)}
					placeholder="Enter your email"
					required
				/>
			</div>

			<div className={styles.field}>
				<label htmlFor="login-password">Password</label>
				<input
					id="login-password"
					type="password"
					autoComplete="current-password"
					value={password}
					onChange={(event) => setPassword(event.target.value)}
					placeholder="Enter your password"
					required
				/>
			</div>

			{error && (
				<p className={styles.error} role="alert">
					{error}
				</p>
			)}

			<button className={styles.submitButton} type="submit" disabled={loading}>
				{loading ? "Signing in..." : "Sign in"}
			</button>

			{onRegister && (
				<p className={styles.switchText}>
					Don't have an account?{" "}
					<button type="button" onClick={onRegister}>
						Register
					</button>
				</p>
			)}
		</form>
	);
}
