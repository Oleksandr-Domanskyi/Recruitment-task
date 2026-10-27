import DOMPurify from "dompurify";

export function sanitizeComment(html: string): string {
	return DOMPurify.sanitize(html, {
		ALLOWED_TAGS: ["strong", "i", "code", "a"],
		ALLOWED_ATTR: ["href", "title"],
		ALLOW_DATA_ATTR: false,
	});
}
