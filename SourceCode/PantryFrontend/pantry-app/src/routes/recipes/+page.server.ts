import { env } from '$env/dynamic/private';
import type { Recipe } from '$lib/modules/recipe/types/Recipe.ts';

export async function load({ fetch }) {
	const res = await fetch(env.PRIVATE_RECIPE_API_URL + '/recipes');
	const recipes: Recipe[] = await res.json();

	return { recipes };
}
