import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import fs from "node:fs";
import path from "node:path";

export default defineConfig({
	plugins: [react()],

	server: {
		host: "localhost",
		port: 5173,

		https: {
			key: fs.readFileSync(
				path.resolve(import.meta.dirname, "localhost-key.pem"),
			),
			cert: fs.readFileSync(path.resolve(import.meta.dirname, "localhost.pem")),
		},

		proxy: {
			"/api": {
				target: "https://localhost:7000",
				changeOrigin: true,
				secure: false,
			},

			"/hubs": {
				target: "https://localhost:7000",
				changeOrigin: true,
				secure: false,
				ws: true,
			},
		},
	},
});
