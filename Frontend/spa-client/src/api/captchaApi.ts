import { apiRequest } from "./client";
import type { CaptchaResponse } from "../types/captcha";

const BASE_URL = "/api/v1/captcha";

export const captchaApi = {
	get() {
		return apiRequest<CaptchaResponse>(BASE_URL);
	},
};
