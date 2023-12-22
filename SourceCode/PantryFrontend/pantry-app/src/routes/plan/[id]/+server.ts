import { env } from '$env/dynamic/private';
export async function GET({ params }): Promise<Response> {
	const response = await fetch(env.PRIVATE_PLAN_API_URL + '/meals/' + params.id);
	return response;
}

export async function PUT({ request }): Promise<Response> {
	const { meal } = await request.json();
	console.log('PUT');
	console.log(meal);
	const response = await fetch(env.PRIVATE_PLAN_API_URL + '/meals/', {
		method: 'PUT',
		body: JSON.stringify(meal),
		headers: {
			'Content-Type': 'application/json'
		}
	});

	return response;
}
