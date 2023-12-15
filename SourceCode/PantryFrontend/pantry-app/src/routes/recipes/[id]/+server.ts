import { env } from '$env/dynamic/private';

export async function DELETE({ params }): Promise<Response> {
	const response = await fetch(env.PRIVATE_RECIPE_API_URL + '/recipes/' + params.id, {
		method: 'DELETE'
	});

	return response;
}

export async function POST({ request, params }): Promise<Response> {
	const { rating } = await request.json();

	const response = await fetch(
		env.PRIVATE_RECIPE_API_URL + '/recipedetails/review/' + params.id + '?review=' + rating,
		{
			method: 'POST',
			body: JSON.stringify(rating),
			headers: {
				'Content-Type': 'application/json'
			}
		}
	);

	return response;
}

export async function GET({ params }): Promise<Response> {
	const response = await fetch(env.PRIVATE_RECIPE_API_URL + '/recipes/overview/' + params.id);
	return response;
}
