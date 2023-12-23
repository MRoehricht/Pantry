import type { Good } from '$lib/modules/goods/types/Good.js';
import { env } from '$env/dynamic/private';
import { getSession } from '@auth/sveltekit';
import { redirect } from '@sveltejs/kit';

export async function load({ fetch, params, request }) {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}
	const res = await fetch(env.PRIVATE_PANTRY_API_URL + '/goods/' + params.id, {
		method: 'GET',
		headers: { UserEMail: session?.user?.email }
	});
	const good: Good = await res.json();

	return { good };
}
