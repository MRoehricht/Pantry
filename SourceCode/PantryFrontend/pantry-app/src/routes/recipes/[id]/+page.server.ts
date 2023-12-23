import { env } from '$env/dynamic/private';
import type { Recipe } from '$lib/modules/recipe/types/Recipe.js';
import { getSession } from '@auth/sveltekit';
import { redirect } from '@sveltejs/kit';

export async function load({ fetch, params, request }) {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}

	const res = await fetch(env.PRIVATE_RECIPE_API_URL + '/recipes/' + params.id, {
		method: 'GET',
		headers: { UserEMail: session?.user?.email }
	});
	const recipe: Recipe = await res.json();

	return { recipe };
}
