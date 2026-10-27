import styles from "./CommentSorting.module.css";

export type CommentSortField = "createdAt" | "userName" | "email";

export type CommentSortDirection = "asc" | "desc";

interface CommentSortingProps {
	sortBy: CommentSortField;
	direction: CommentSortDirection;
	onChange: (sortBy: CommentSortField, direction: CommentSortDirection) => void;
}

const sortOptions: {
	value: CommentSortField;
	label: string;
}[] = [
	{ value: "createdAt", label: "Date" },
	{ value: "userName", label: "Username" },
	{ value: "email", label: "Email" },
];

export default function CommentSorting({
	sortBy,
	direction,
	onChange,
}: CommentSortingProps) {
	function handleSort(field: CommentSortField) {
		if (sortBy === field) {
			onChange(field, direction === "asc" ? "desc" : "asc");

			return;
		}

		onChange(field, "desc");
	}

	const activeOption = sortOptions.find((option) => option.value === sortBy);

	return (
		<section className={styles.sorting} aria-label="Comment sorting">
			<div className={styles.topRow}>
				<div className={styles.heading}>
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
						<path d="M4 7h16M7 12h10M10 17h4" />
					</svg>

					<span>Comments feed</span>
				</div>

				<span className={styles.currentSort}>
					{activeOption?.label}
					{" · "}
					{direction === "desc" ? "Descending" : "Ascending"}
				</span>
			</div>

			<div
				className={styles.options}
				role="group"
				aria-label="Sort comments by">
				{sortOptions.map((option) => {
					const active = sortBy === option.value;

					return (
						<button
							key={option.value}
							type="button"
							className={`${styles.option} ${active ? styles.active : ""}`}
							aria-pressed={active}
							onClick={() => handleSort(option.value)}>
							<span>{option.label}</span>

							{active && (
								<svg
									className={styles.arrow}
									viewBox="0 0 24 24"
									width="15"
									height="15"
									fill="none"
									stroke="currentColor"
									strokeWidth="2"
									strokeLinecap="round"
									strokeLinejoin="round"
									aria-hidden="true">
									{direction === "desc" ?
										<path d="M12 4v16m-6-6 6 6 6-6" />
									:	<path d="M12 20V4m-6 6 6-6 6 6" />}
								</svg>
							)}
						</button>
					);
				})}
			</div>
		</section>
	);
}
