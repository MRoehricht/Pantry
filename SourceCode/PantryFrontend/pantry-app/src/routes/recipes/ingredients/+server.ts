import { env } from '$env/dynamic/private';
import { getSession } from '@auth/sveltekit';
import { redirect } from '@sveltejs/kit';

export async function PUT({ request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}

	const { ingredientCreateDto } = await request.json();
	const response = await fetch(
		env.PRIVATE_RECIPE_API_URL +
			'/ingredients/' +
			ingredientCreateDto.recipeId +
			'/' +
			ingredientCreateDto.name,
		{
			method: 'PUT',
			body: JSON.stringify(ingredientCreateDto.ingredient),
			headers: {
				'Content-Type': 'application/json',
				UserEMail: session?.user?.email
			}
		}
	);

	return response;
}

export async function POST({ request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}

	const { ingredientCreateDto } = await request.json();
	const response = await fetch(
		env.PRIVATE_RECIPE_API_URL + '/ingredients/' + ingredientCreateDto.recipeId,
		{
			method: 'POST',
			body: JSON.stringify(ingredientCreateDto.ingredient),
			headers: {
				'Content-Type': 'application/json',
				UserEMail: session?.user?.email
			}
		}
	);

	return response;
}

export async function DELETE({ request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}

	const { ingredientCreateDto } = await request.json();
	const response = await fetch(
		env.PRIVATE_RECIPE_API_URL +
			'/ingredients/' +
			ingredientCreateDto.recipeId +
			'/' +
			ingredientCreateDto.name,
		{
			method: 'DELETE',
			headers: {
				UserEMail: session?.user?.email
			}
		}
	);

	return response;
}
