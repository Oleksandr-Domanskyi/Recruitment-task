export interface ParsedApiError {
	title: string;
	message: string;
	status?: number;
	fields: Record<string, string[]>;
}

type UnknownRecord = Record<string, unknown>;

function isRecord(value: unknown): value is UnknownRecord {
	return typeof value === "object" && value !== null && !Array.isArray(value);
}

function parseJson(value: unknown): unknown {
	if (typeof value !== "string") {
		return value;
	}

	try {
		return JSON.parse(value);
	} catch {
		return value;
	}
}
export function isCaptchaApiError(problem: ParsedApiError): boolean {
	const containsCaptcha = (text: string): boolean =>
		/captcha|verification code|security code/i.test(text);

	return (
		Object.keys(problem.fields).some(containsCaptcha) ||
		containsCaptcha(problem.message)
	);
}

function extractPayload(error: unknown): unknown {
	if (isRecord(error) && isRecord(error.response) && "data" in error.response) {
		return parseJson(error.response.data);
	}

	if (error instanceof Error) {
		return parseJson(error.message);
	}

	return parseJson(error);
}

export function parseApiError(error: unknown): ParsedApiError {
	const payload = extractPayload(error);

	const fallback: ParsedApiError = {
		title: "Unable to publish comment",
		message: "Something went wrong. Please try again.",
		fields: {},
	};

	if (typeof payload === "string") {
		return {
			...fallback,
			message: payload.trim() || fallback.message,
		};
	}

	if (!isRecord(payload)) {
		return fallback;
	}

	const status =
		typeof payload.status === "number" ? payload.status : undefined;

	const fields: Record<string, string[]> = {};

	if (isRecord(payload.errors)) {
		for (const [field, value] of Object.entries(payload.errors)) {
			if (!Array.isArray(value)) continue;

			const messages = value.filter(
				(item): item is string =>
					typeof item === "string" && item.trim().length > 0,
			);

			if (messages.length > 0) {
				fields[field] = messages;
			}
		}
	}

	const validationMessages = [...new Set(Object.values(fields).flat())];

	const detail =
		typeof payload.detail === "string" ? payload.detail.trim() : "";

	const genericDetails = [
		"One or more validation errors occurred.",
		"The request is invalid.",
	];

	let message = fallback.message;

	if (validationMessages.length > 0) {
		message = validationMessages.join("\n");
	} else if (detail && !genericDetails.includes(detail)) {
		message = detail;
	} else if (status === 413) {
		message = "The selected file is too large.";
	} else if (status === 415) {
		message = "The selected file format is not supported.";
	} else if (status === 400) {
		message = "Please check your comment and attachment.";
	} else if (status === 403) {
		message = "You do not have permission to perform this action.";
	} else if (status === 500) {
		message = "A server error occurred. Please try again later.";
	}

	return {
		title: status === 500 ? "Server error" : "Unable to publish comment",
		message,
		status,
		fields,
	};
}
