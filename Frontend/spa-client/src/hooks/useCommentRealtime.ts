import { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";

export interface CommentCreatedRealtimeEvent {
	commentId: string;
	parentCommentId: string | null;
	createdAt: string;
}

export interface CommentRatingChangedRealtimeEvent {
	commentId: string;
	rating: number;
}

export function useCommentRealtime(
	onCommentCreated: (event: CommentCreatedRealtimeEvent) => void,
	onRatingChanged?: (event: CommentRatingChangedRealtimeEvent) => void,
): void {
	const createdCallbackRef = useRef(onCommentCreated);
	const ratingCallbackRef = useRef(onRatingChanged);

	useEffect(() => {
		createdCallbackRef.current = onCommentCreated;
		ratingCallbackRef.current = onRatingChanged;
	}, [onCommentCreated, onRatingChanged]);

	useEffect(() => {
		let disposed = false;

		const connection = new signalR.HubConnectionBuilder()
			.withUrl("/hubs/comments")
			.withAutomaticReconnect()
			.configureLogging(signalR.LogLevel.Information)
			.build();

		function handleCommentCreated(event: CommentCreatedRealtimeEvent) {
			createdCallbackRef.current(event);
		}

		function handleRatingChanged(event: CommentRatingChangedRealtimeEvent) {
			ratingCallbackRef.current?.(event);
		}

		connection.on("CommentCreated", handleCommentCreated);
		connection.on("CommentRatingChanged", handleRatingChanged);

		connection.onreconnected(() => {
			if (!disposed) {
				console.info("SignalR reconnected");
			}
		});

		connection.onclose((error) => {
			if (!disposed) {
				console.warn("SignalR connection closed", error);
			}
		});

		async function connect() {
			try {
				await connection.start();

				if (disposed) {
					await connection.stop();
					return;
				}

				console.info("SignalR connected");
			} catch (error) {
				if (!disposed) {
					console.error("SignalR connection failed:", error);
				}
			}
		}

		void connect();

		return () => {
			disposed = true;

			connection.off("CommentCreated", handleCommentCreated);
			connection.off("CommentRatingChanged", handleRatingChanged);

			void connection.stop();
		};
	}, []);
}
