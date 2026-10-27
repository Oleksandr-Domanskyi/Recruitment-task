import { useEffect, useRef, useState, type ChangeEvent } from "react";

import {
	profileApi,
	type UserDiscussion,
	type UserProfile,
} from "../../api/profileApi";

import styles from "./UserMenu.module.css";

interface UserMenuProps {
	accessToken: string;
	userName: string;
}

const MAX_AVATAR_SIZE = 2 * 1024 * 1024;

const ALLOWED_TYPES = ["image/jpeg", "image/png"];

export default function UserMenu({ accessToken, userName }: UserMenuProps) {
	const [open, setOpen] = useState(false);

	const [profile, setProfile] = useState<UserProfile | null>(null);
	const [discussions, setDiscussions] = useState<UserDiscussion[]>([]);

	const [profileLoading, setProfileLoading] = useState(false);
	const [discussionsLoading, setDiscussionsLoading] = useState(false);
	const [uploading, setUploading] = useState(false);

	const [profileError, setProfileError] = useState<string | null>(null);
	const [discussionsError, setDiscussionsError] = useState<string | null>(null);

	const containerRef = useRef<HTMLDivElement>(null);
	const fileInputRef = useRef<HTMLInputElement>(null);
	const triggerRef = useRef<HTMLButtonElement>(null);

	const displayName = profile?.userName || userName;
	const avatarUrl = profile?.avatarUrl ?? null;

	useEffect(() => {
		let active = true;

		async function loadProfile() {
			setProfileLoading(true);
			setProfileError(null);
			setProfile(null);

			try {
				const result = await profileApi.getMe(accessToken);

				if (active) {
					setProfile(result);
				}
			} catch {
				if (active) {
					setProfileError("Failed to load profile.");
				}
			} finally {
				if (active) {
					setProfileLoading(false);
				}
			}
		}

		void loadProfile();

		return () => {
			active = false;
		};
	}, [accessToken]);

	useEffect(() => {
		if (!open) return;

		function handlePointerDown(event: PointerEvent) {
			const target = event.target as Node;

			if (containerRef.current && !containerRef.current.contains(target)) {
				setOpen(false);
			}
		}

		function handleKeyDown(event: KeyboardEvent) {
			if (event.key === "Escape") {
				setOpen(false);
				triggerRef.current?.focus();
			}
		}

		document.addEventListener("pointerdown", handlePointerDown);
		document.addEventListener("keydown", handleKeyDown);

		return () => {
			document.removeEventListener("pointerdown", handlePointerDown);
			document.removeEventListener("keydown", handleKeyDown);
		};
	}, [open]);

	useEffect(() => {
		if (!open) return;

		let active = true;

		async function loadDiscussions() {
			setDiscussionsLoading(true);
			setDiscussionsError(null);

			try {
				const result = await profileApi.getUserDiscussions(accessToken, 5);

				if (active) {
					setDiscussions(result);
				}
			} catch {
				if (active) {
					setDiscussionsError("Discussions are not available yet.");
				}
			} finally {
				if (active) {
					setDiscussionsLoading(false);
				}
			}
		}

		void loadDiscussions();

		return () => {
			active = false;
		};
	}, [open, accessToken]);

	function getDiscussionUrl(rootCommentId: string): string {
		const url = new URL(window.location.href);

		url.searchParams.set("discussion", rootCommentId);

		return `${url.pathname}${url.search}${url.hash}`;
	}

	async function handleAvatarChange(event: ChangeEvent<HTMLInputElement>) {
		const input = event.currentTarget;
		const file = input.files?.[0];

		if (!file || uploading) return;

		if (!ALLOWED_TYPES.includes(file.type)) {
			setProfileError("Only JPG and PNG images are supported.");
			input.value = "";
			return;
		}

		if (file.size > MAX_AVATAR_SIZE) {
			setProfileError("Maximum avatar size is 2 MB.");
			input.value = "";
			return;
		}

		setUploading(true);
		setProfileError(null);

		try {
			const updatedProfile = await profileApi.updateAvatar(file, accessToken);

			setProfile(updatedProfile);
		} catch (error) {
			setProfileError(
				error instanceof Error ? error.message : "Failed to update avatar.",
			);
		} finally {
			setUploading(false);
			input.value = "";
		}
	}

	function renderAvatar() {
		if (avatarUrl) {
			return <img src={avatarUrl} alt="" className={styles.avatarImage} />;
		}

		return (
			<svg
				viewBox="0 0 24 24"
				width="18"
				height="18"
				fill="none"
				stroke="currentColor"
				strokeWidth="1.8"
				strokeLinecap="round"
				strokeLinejoin="round"
				aria-hidden="true">
				<circle cx="12" cy="8" r="4" />
				<path d="M5 21a7 7 0 0 1 14 0" />
			</svg>
		);
	}

	return (
		<div className={styles.container} ref={containerRef}>
			<button
				ref={triggerRef}
				type="button"
				className={styles.trigger}
				aria-expanded={open}
				aria-haspopup="dialog"
				onClick={() => setOpen((previous) => !previous)}>
				<span className={styles.avatar}>{renderAvatar()}</span>

				<span className={styles.userName}>{displayName}</span>

				<svg
					className={styles.chevron}
					viewBox="0 0 24 24"
					width="14"
					height="14"
					fill="none"
					stroke="currentColor"
					strokeWidth="2"
					aria-hidden="true">
					<path d="m6 9 6 6 6-6" />
				</svg>
			</button>

			{open && (
				<div
					className={styles.dropdown}
					role="dialog"
					aria-label="User profile">
					<div className={styles.profile}>
						<span className={styles.largeAvatar}>{renderAvatar()}</span>

						<div className={styles.profileDetails}>
							<strong className={styles.profileName}>{displayName}</strong>

							{profile?.email && (
								<span className={styles.email}>{profile.email}</span>
							)}
						</div>
					</div>

					<input
						ref={fileInputRef}
						type="file"
						accept=".jpg,.jpeg,.png"
						className={styles.fileInput}
						onChange={(event) => void handleAvatarChange(event)}
						disabled={uploading}
						aria-label="Select avatar image"
					/>

					<button
						type="button"
						className={styles.changeAvatar}
						onClick={() => fileInputRef.current?.click()}
						disabled={uploading}>
						<span>{uploading ? "Uploading..." : "Change avatar"}</span>
					</button>

					{profileLoading && <p className={styles.muted}>Loading profile...</p>}

					{profileError && (
						<p className={styles.error} role="alert">
							{profileError}
						</p>
					)}

					<div className={styles.divider} />

					<div className={styles.sectionHeader}>
						<span>Recent discussions</span>
						<span className={styles.sectionCount}>Last 5</span>
					</div>

					{discussionsLoading ?
						<p className={styles.muted}>Loading discussions...</p>
					: discussionsError ?
						<p className={styles.muted}>{discussionsError}</p>
					: discussions.length === 0 ?
						<p className={styles.muted}>No discussions yet.</p>
					:	<div className={styles.discussionList}>
							{discussions.map((discussion) => (
								<a
									key={discussion.rootCommentId}
									className={styles.discussion}
									href={getDiscussionUrl(discussion.rootCommentId)}
									onClick={() => setOpen(false)}>
									<div className={styles.discussionContent}>
										<span className={styles.discussionText}>
											{discussion.preview}
										</span>

										<time
											className={styles.discussionDate}
											dateTime={discussion.lastActivityAt}>
											{new Date(discussion.lastActivityAt).toLocaleDateString()}
										</time>
									</div>

									<svg
										className={styles.discussionArrow}
										viewBox="0 0 24 24"
										width="16"
										height="16"
										fill="none"
										stroke="currentColor"
										strokeWidth="1.8"
										strokeLinecap="round"
										strokeLinejoin="round"
										aria-hidden="true">
										<path d="m9 18 6-6-6-6" />
									</svg>
								</a>
							))}
						</div>
					}
				</div>
			)}
		</div>
	);
}
