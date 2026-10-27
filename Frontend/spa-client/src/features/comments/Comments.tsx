import { useContext, useEffect, useState } from "react";

import { commentsApi } from "../../api/commentsApi";
import { AuthContext } from "../../context/AuthContext";

import CommentForm from "../../components/CommentForm/CommentForm";
import CommentList from "../../components/CommentList/CommentList";
import CommentSorting from "../../components/CommentSorting/CommentSorting";
import CommentSearch, {
	type CommentSearchFilter,
} from "../../components/CommentSearch/CommentSearch";
import Modal from "../../components/Modal/Modal";

import { CommentRealtimeContext } from "../../context/CommentRealtimeContext";
import { CommentRatingRealtimeContext } from "../../context/CommentRatingRealtimeContext";

import {
	useCommentRealtime,
	type CommentCreatedRealtimeEvent,
	type CommentRatingChangedRealtimeEvent,
} from "../../hooks/useCommentRealtime";

import type { Comment } from "../../types/comment";
import type {
	CommentSortField,
	CommentSortDirection,
} from "../../components/CommentSorting/CommentSorting";

import styles from "./Comments.module.css";

const SORT_FIELDS: Record<CommentSortField, number> = {
	createdAt: 0,
	userName: 1,
	email: 2,
};

const SORT_DIRECTIONS: Record<CommentSortDirection, number> = {
	asc: 0,
	desc: 1,
};

function getDiscussionId(): string | null {
	return new URLSearchParams(window.location.search).get("discussion");
}

const initialSearchFilter: CommentSearchFilter = {
	field: "username",
	query: "",
};

export default function Comments() {
	const auth = useContext(AuthContext);
	const accessToken = auth?.accessToken ?? null;

	const [comments, setComments] = useState<Comment[]>([]);

	const [discussionId, setDiscussionId] = useState<string | null>(
		getDiscussionId,
	);

	const [page, setPage] = useState(1);
	const [totalPages, setTotalPages] = useState(0);
	const [refreshKey, setRefreshKey] = useState(0);

	const [showForm, setShowForm] = useState(false);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);

	const [sortBy, setSortBy] = useState<CommentSortField>("createdAt");

	const [direction, setDirection] = useState<CommentSortDirection>("desc");

	const [searchFilter, setSearchFilter] =
		useState<CommentSearchFilter>(initialSearchFilter);

	const [latestCreatedEvent, setLatestCreatedEvent] =
		useState<CommentCreatedRealtimeEvent | null>(null);

	const [latestRatingEvent, setLatestRatingEvent] =
		useState<CommentRatingChangedRealtimeEvent | null>(null);

	const isFocused = discussionId !== null;

	const hasActiveSearch = searchFilter.query.trim().length > 0;

	useCommentRealtime(
		(event: CommentCreatedRealtimeEvent) => {
			if (event.parentCommentId === null) {
				setRefreshKey((value) => value + 1);
				return;
			}

			setLatestCreatedEvent(event);

			void commentsApi
				.getById(event.parentCommentId)
				.then((updatedParent) => {
					if (updatedParent.parentCommentId !== null) {
						return;
					}

					setComments((current) =>
						current.map((comment) =>
							comment.id === updatedParent.id ?
								{
									...comment,
									repliesCount: updatedParent.repliesCount,
								}
							:	comment,
						),
					);
				})
				.catch((error) => {
					console.error("Failed to update reply count:", error);
				});
		},

		(event: CommentRatingChangedRealtimeEvent) => {
			setLatestRatingEvent(event);

			setComments((current) =>
				current.map((comment) =>
					comment.id === event.commentId ?
						{
							...comment,
							rating: event.rating,
						}
					:	comment,
				),
			);
		},
	);

	useEffect(() => {
		function handlePopState() {
			setDiscussionId(getDiscussionId());
		}

		window.addEventListener("popstate", handlePopState);

		return () => {
			window.removeEventListener("popstate", handlePopState);
		};
	}, []);

	useEffect(() => {
		let active = true;

		async function loadComments() {
			setLoading(true);
			setError(null);
			setComments([]);

			try {
				if (discussionId) {
					const comment = await commentsApi.getById(discussionId);

					if (!active) return;

					if (comment.parentCommentId !== null) {
						throw new Error("Selected comment is not a root discussion.");
					}

					setComments([comment]);
					setTotalPages(0);

					return;
				}

				const username =
					searchFilter.field === "username" ? searchFilter.query.trim() : "";

				const email =
					searchFilter.field === "email" ? searchFilter.query.trim() : "";

				const response = await commentsApi.getAll({
					page,
					sortBy: String(SORT_FIELDS[sortBy]),
					direction: String(SORT_DIRECTIONS[direction]),
					username: username || undefined,
					email: email || undefined,
				});

				if (!active) return;

				setComments(response.items);
				setTotalPages(response.totalPages);
			} catch (err) {
				if (!active) return;

				setError(
					err instanceof Error ? err.message : "Failed to load comments.",
				);
			} finally {
				if (active) {
					setLoading(false);
				}
			}
		}

		void loadComments();

		return () => {
			active = false;
		};
	}, [
		discussionId,
		page,
		sortBy,
		direction,
		searchFilter.field,
		searchFilter.query,
		refreshKey,
		accessToken,
	]);

	function handleSortChange(
		field: CommentSortField,
		order: CommentSortDirection,
	) {
		setSortBy(field);
		setDirection(order);
		setPage(1);
	}

	function handleSearch(filter: CommentSearchFilter) {
		const nextFilter: CommentSearchFilter = {
			field: filter.field,
			query: filter.query.trim(),
		};

		setSearchFilter(nextFilter);
		setPage(1);
	}

	function handleCreated() {
		setShowForm(false);

		if (!isFocused) {
			setPage(1);
		}

		setRefreshKey((value) => value + 1);
	}

	function handleBackToFeed() {
		const url = new URL(window.location.href);

		url.searchParams.delete("discussion");

		window.history.pushState({}, "", `${url.pathname}${url.search}${url.hash}`);

		setDiscussionId(null);
	}

	function handleRetry() {
		setRefreshKey((value) => value + 1);
	}

	return (
		<main className={styles.container}>
			<header className={styles.header}>
				<div className={styles.heading}>
					<div className={styles.eyebrow}>
						<span className={styles.statusDot} />
						COMMUNITY SPACE
					</div>

					<h1>{isFocused ? "Selected discussion" : "Discussion"}</h1>

					<p>
						{isFocused ?
							"Follow the conversation and explore its replies."
						:	"Share ideas, ask questions and join the conversation."}
					</p>
				</div>

				<button
					type="button"
					className={styles.addButton}
					onClick={() => setShowForm(true)}>
					<svg
						viewBox="0 0 24 24"
						width="18"
						height="18"
						fill="none"
						stroke="currentColor"
						strokeWidth="2"
						strokeLinecap="round"
						strokeLinejoin="round"
						aria-hidden="true">
						<path d="M12 5v14M5 12h14" />
					</svg>

					<span>New comment</span>
				</button>
			</header>

			<section
				className={styles.feed}
				aria-label={isFocused ? "Selected discussion" : "Comments feed"}>
				{isFocused ?
					<div className={styles.focusToolbar}>
						<button
							type="button"
							className={styles.backButton}
							onClick={handleBackToFeed}>
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
								<path d="m15 18-6-6 6-6" />
								<path d="M20 12H9" />
							</svg>
							Back to discussions
						</button>

						<span className={styles.focusLabel}>Viewing one discussion</span>
					</div>
				:	<div
						style={{
							display: "flex",
							flexDirection: "column",
							gap: "14px",
						}}>
						<CommentSearch value={searchFilter} onSearch={handleSearch} />

						<CommentSorting
							sortBy={sortBy}
							direction={direction}
							onChange={handleSortChange}
						/>
					</div>
				}

				<div className={styles.feedContent} aria-busy={loading}>
					{loading && (
						<div className={styles.loadingState} role="status">
							<span className={styles.spinner} />
							Loading comments...
						</div>
					)}

					{error && (
						<div className={styles.errorState} role="alert">
							<span>{error}</span>

							<button type="button" onClick={handleRetry}>
								Try again
							</button>
						</div>
					)}

					{!loading && !error && (
						<>
							{comments.length > 0 ?
								<CommentRatingRealtimeContext.Provider
									value={latestRatingEvent}>
									<CommentRealtimeContext.Provider value={latestCreatedEvent}>
										<CommentList
											comments={comments}
											accessToken={accessToken}
										/>
									</CommentRealtimeContext.Provider>
								</CommentRatingRealtimeContext.Provider>
							:	<div className={styles.loadingState}>
									{isFocused ?
										"Discussion not found."
									: hasActiveSearch ?
										"No comments match your search."
									:	"No comments yet. Start the conversation."}
								</div>
							}

							{!isFocused && totalPages > 1 && (
								<nav
									className={styles.pagination}
									aria-label="Comments pagination">
									<button
										type="button"
										disabled={page <= 1}
										onClick={() => setPage((value) => value - 1)}>
										<svg
											viewBox="0 0 24 24"
											width="16"
											height="16"
											fill="none"
											stroke="currentColor"
											strokeWidth="2"
											strokeLinecap="round"
											strokeLinejoin="round"
											aria-hidden="true">
											<path d="m15 18-6-6 6-6" />
										</svg>
										Previous
									</button>

									<span className={styles.pageInfo}>
										<span>Page</span>
										<strong>{page}</strong>
										<span>of {totalPages}</span>
									</span>

									<button
										type="button"
										disabled={page >= totalPages}
										onClick={() => setPage((value) => value + 1)}>
										Next
										<svg
											viewBox="0 0 24 24"
											width="16"
											height="16"
											fill="none"
											stroke="currentColor"
											strokeWidth="2"
											strokeLinecap="round"
											strokeLinejoin="round"
											aria-hidden="true">
											<path d="m9 18 6-6-6-6" />
										</svg>
									</button>
								</nav>
							)}
						</>
					)}
				</div>
			</section>

			{showForm && (
				<Modal title="New comment" onClose={() => setShowForm(false)}>
					<CommentForm onCreated={handleCreated} />
				</Modal>
			)}
		</main>
	);
}
