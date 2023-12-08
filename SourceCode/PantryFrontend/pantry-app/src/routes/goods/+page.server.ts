import type { Good, Goods } from '$lib/modules/goods/types/Good.js';
import { error } from '@sveltejs/kit';

export async function load({ fetch }) {
	const res = await fetch('http://localhost:56158/goods');
	const goods: Good[] = await res.json();

	return { goods };
}
