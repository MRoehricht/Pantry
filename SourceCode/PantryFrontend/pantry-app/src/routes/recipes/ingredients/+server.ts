import { env } from '$env/dynamic/private';

export async function PUT({ request }): Promise<Response> {
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
				'Content-Type': 'application/json'
			}
		}
	);

	return response;
}

export async function POST({ request }): Promise<Response> {
	const { ingredientCreateDto } = await request.json();
	console.log(ingredientCreateDto);
	const response = await fetch(
		env.PRIVATE_RECIPE_API_URL + '/ingredients/' + ingredientCreateDto.recipeId,
		{
			method: 'POST',
			body: JSON.stringify(ingredientCreateDto.ingredient),
			headers: {
				'Content-Type': 'application/json'
			}
		}
	);

	return response;
}

export async function DELETE({ request }): Promise<Response> {
	const { ingredientCreateDto } = await request.json();
	const response = await fetch(
		env.PRIVATE_RECIPE_API_URL +
			'/ingredients/' +
			ingredientCreateDto.recipeId +
			'/' +
			ingredientCreateDto.name,
		{
			method: 'DELETE'
		}
	);

	return response;
}
