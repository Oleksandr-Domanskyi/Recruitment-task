import { useEffect, useState, type FormEvent } from "react";

import styles from "./CommentSearch.module.css";

export type SearchField = "username" | "email";

export interface CommentSearchFilter {
	field: SearchField;
	query: string;
}

interface CommentSearchProps {
	value: CommentSearchFilter;
	onSearch: (filter: CommentSearchFilter) => void;
}

export default function CommentSearch({ value, onSearch }: CommentSearchProps) {
	const [field, setField] = useState<SearchField>(value.field);
	const [query, setQuery] = useState(value.query);

	useEffect(() => {
		setField(value.field);
		setQuery(value.query);
	}, [value.field, value.query]);

	function handleSubmit(event: FormEvent<HTMLFormElement>) {
		event.preventDefault();

		onSearch({
			field,
			query: query.trim(),
		});
	}

	function handleClear() {
		setQuery("");

		onSearch({
			field,
			query: "",
		});
	}

	return (
		<form className={styles.search} role="search" onSubmit={handleSubmit}>
			<div className={styles.inputContainer}>
				<svg
					className={styles.searchIcon}
					width="17"
					height="17"
					viewBox="0 0 24 24"
					fill="none"
					stroke="currentColor"
					strokeWidth="1.8"
					strokeLinecap="round">
					<circle cx="11" cy="11" r="7" />
					<path d="m16 16 5 5" />
				</svg>

				<input
					type="search"
					className={styles.input}
					value={query}
					onChange={(event) => setQuery(event.target.value)}
					placeholder={
						field === "username" ?
							"Search by username..."
						:	"Search by email..."
					}
					aria-label={
						field === "username" ?
							"Search comments by username"
						:	"Search comments by email"
					}
				/>

				{query && (
					<button
						type="button"
						className={styles.clear}
						onClick={handleClear}
						aria-label="Clear search">
						×
					</button>
				)}
			</div>

			<select
				className={styles.select}
				value={field}
				onChange={(event) => setField(event.target.value as SearchField)}
				aria-label="Search field">
				<option value="username">Username</option>
				<option value="email">Email</option>
			</select>

			<button type="submit" className={styles.submit}>
				<svg
					width="16"
					height="16"
					viewBox="0 0 24 24"
					fill="none"
					stroke="currentColor"
					strokeWidth="1.8"
					strokeLinecap="round">
					<circle cx="11" cy="11" r="7" />
					<path d="m16 16 5 5" />
				</svg>

				<span>Search</span>
			</button>
		</form>
	);
}
