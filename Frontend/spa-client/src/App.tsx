import Header from "./layout/Header/Header";
import Comments from "./features/comments/Comments";

import "./App.css";

export default function App() {
	return (
		<main className="app">
			<Header />
			<Comments />
		</main>
	);
}
