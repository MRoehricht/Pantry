import { env } from '$env/dynamic/private';
import type { Recipe } from '$lib/modules/recipe/types/Recipe.js';

export async function load({ fetch, params }) {
	const res = await fetch(env.PRIVATE_RECIPE_API_URL + '/recipes/' + params.id);
	const recipe: Recipe = await res.json();

	return { recipe };
}
