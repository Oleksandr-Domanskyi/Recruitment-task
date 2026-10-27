import { createContext, useContext } from "react";

import type { CommentRatingChangedRealtimeEvent } from "../hooks/useCommentRealtime";

export const CommentRatingRealtimeContext =
	createContext<CommentRatingChangedRealtimeEvent | null>(null);

export function useCommentRatingEvent() {
	return useContext(CommentRatingRealtimeContext);
}
