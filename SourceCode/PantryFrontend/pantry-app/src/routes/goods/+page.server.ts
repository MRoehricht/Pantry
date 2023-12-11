import type { GoodOverview } from '$lib/modules/goods/types/Good.ts';
import { env } from '$env/dynamic/private';

export async function load({ fetch }) {
	const res = await fetch(env.PRIVATE_PANTRY_API_URL + '/goods/overview');
	const goods: GoodOverview[] = await res.json();

	return { goods };
}
