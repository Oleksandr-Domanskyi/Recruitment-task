import type { Comment } from "../../types/comment";

import CommentCard from "../CommentCard/CommentCard";

import styles from "./CommentList.module.css";

interface CommentListProps {
	comments: Comment[];
	accessToken: string | null;
}

export default function CommentList({
	comments,
	accessToken,
}: CommentListProps) {
	if (comments.length === 0) {
		return <div className={styles.empty}>No comments yet.</div>;
	}

	return (
		<section className={styles.list}>
			{comments.map((comment) => (
				<CommentCard
					key={comment.id}
					comment={comment}
					accessToken={accessToken}
				/>
			))}
		</section>
	);
}
