import type { Good } from '$lib/modules/goods/types/Good.js';
import { env } from '$env/dynamic/private';

export async function load({ fetch, params }) {
	const res = await fetch(env.PRIVATE_PANTRY_API_URL + '/goods/' + params.id);
	const good: Good = await res.json();

	return { good };
}
