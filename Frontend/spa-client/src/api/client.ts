const API_URL = import.meta.env.VITE_API_URL;

export class ApiError extends Error {
	public readonly status: number;

	constructor(status: number, message: string) {
		super(message);

		this.name = "ApiError";
		this.status = status;
	}
}

export async function apiRequest<T>(
	endpoint: string,
	options: RequestInit = {},
): Promise<T> {
	const headers = new Headers(options.headers);

	const response = await fetch(`${API_URL}${endpoint}`, {
		...options,
		headers,
	});

	if (!response.ok) {
		const error = await response.text();

		throw new ApiError(
			response.status,
			error || `HTTP error: ${response.status}`,
		);
	}

	if (response.status === 204) {
		return undefined as T;
	}

	return (await response.json()) as T;
}
