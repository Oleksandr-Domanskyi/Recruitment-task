import { useState } from "react";

import Modal from "../../components/Modal/Modal";

import Login from "./Login";
import Register from "./Register";

interface AuthModalProps {
	onClose: () => void;
}

type AuthMode = "login" | "register";

export default function AuthModal({ onClose }: AuthModalProps) {
	const [mode, setMode] = useState<AuthMode>("login");
	const [registered, setRegistered] = useState(false);

	const switchToLogin = () => {
		setMode("login");
	};

	const switchToRegister = () => {
		setRegistered(false);
		setMode("register");
	};

	const handleRegisterSuccess = () => {
		setRegistered(true);
		setMode("login");
	};

	return (
		<Modal
			title={mode === "login" ? "Sign in" : "Create account"}
			onClose={onClose}>
			{mode === "login" ?
				<>
					{registered && (
						<p role="status">
							Account created successfully. You can now sign in.
						</p>
					)}

					<Login onSuccess={onClose} onRegister={switchToRegister} />
				</>
			:	<Register onSuccess={handleRegisterSuccess} onLogin={switchToLogin} />}
		</Modal>
	);
}
