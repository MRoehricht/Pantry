import { env } from '$env/dynamic/private';

export async function PUT({ request }): Promise<Response> {
	const { recipe } = await request.json();
	const response = await fetch(env.PRIVATE_RECIPE_API_URL + '/recipes/', {
		method: 'PUT',
		body: JSON.stringify(recipe),
		headers: {
			'Content-Type': 'application/json'
		}
	});

	return response;
}

export async function POST({ request }): Promise<Response> {
	const { createDto } = await request.json();
	const response = await fetch(env.PRIVATE_RECIPE_API_URL + '/recipes/', {
		method: 'POST',
		body: JSON.stringify(createDto),
		headers: {
			'Content-Type': 'application/json'
		}
	});

	return response;
}

export async function GET(): Promise<Response> {
	const response = await fetch(env.PRIVATE_RECIPE_API_URL + '/recipes/overview');
	return response;
}

export async function PATCH({ request }): Promise<Response> {
	const { id } = await request.json();
	const response = await fetch(env.PRIVATE_RECIPE_API_URL + '/recipes/' + id);
	return response;
}
