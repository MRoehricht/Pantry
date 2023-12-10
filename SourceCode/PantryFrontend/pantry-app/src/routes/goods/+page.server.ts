import type { Good, Goods } from '$lib/modules/goods/types/Good.js';
import { error } from '@sveltejs/kit';
import { env } from '$env/dynamic/private';

export async function load({ fetch }) {
	const res = await fetch(env.PRIVATE_PANTRY_API_URL + '/goods');
	const goods: Good[] = await res.json();

	return { goods };
}
