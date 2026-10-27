import { createContext, useContext } from "react";

import type { CommentCreatedRealtimeEvent } from "../hooks/useCommentRealtime";

export const CommentRealtimeContext =
	createContext<CommentCreatedRealtimeEvent | null>(null);

export function useCommentCreatedEvent() {
	return useContext(CommentRealtimeContext);
}
