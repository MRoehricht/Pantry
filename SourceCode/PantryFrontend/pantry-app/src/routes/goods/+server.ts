import { json } from '@sveltejs/kit';

/** @type {import('./$types').RequestHandler} */
export async function GET() {
	let data;
	try {
		data = await fetchGoodsData();
	} catch (error) {
		console.error('Error:', error);
		return json('Fehler', { status: 500 });
	}

	return json(data);
}

async function fetchGoodsData() {
	const response = await fetch('http://localhost:56158/goods');

	if (!response.ok) {
		throw new Error(`HTTP error! status: ${response.status}`);
	}

	const data = await response.json();
	return data;
}
