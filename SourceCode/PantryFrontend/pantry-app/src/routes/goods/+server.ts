import { env } from '$env/dynamic/private';

export async function DELETE({ request }): Promise<Response> {
	const { id } = await request.json();

	const response = await fetch(env.PRIVATE_PANTRY_API_URL + '/goods/' + id, {
		method: 'DELETE'
	});

	return response;
}

export async function PUT({ request }): Promise<Response> {
	const { good } = await request.json();
	const response = await fetch(env.PRIVATE_PANTRY_API_URL + '/goods/', {
		method: 'PUT',
		body: JSON.stringify(good),
		headers: {
			'Content-Type': 'application/json'
		}
	});

	return response;
}
