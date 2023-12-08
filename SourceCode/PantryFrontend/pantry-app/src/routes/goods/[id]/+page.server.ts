import type { Good } from '$lib/modules/goods/types/Good.js';

export async function load({ fetch, params }) {
	const res = await fetch('http://localhost:56158/goods/' + params.id);
	const good: Good = await res.json();

	return { good };
}
