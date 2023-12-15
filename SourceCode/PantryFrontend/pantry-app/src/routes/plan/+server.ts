import { env } from '$env/dynamic/private';

export async function POST({ request }): Promise<Response> {
	const { createMealRequest } = await request.json();
	const response = await fetch(env.PRIVATE_PLAN_API_URL + '/meals', {
		method: 'POST',
		body: JSON.stringify(createMealRequest),
		headers: {
			'Content-Type': 'application/json'
		}
	});

	return response;
}

export async function GET({ request }): Promise<Response> {
	const { date } = await request.json();
	const response = await fetch(env.PRIVATE_RECIPE_API_URL + '/meals/date/' + date);
	return response;
}
