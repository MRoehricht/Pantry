import { env } from '$env/dynamic/private';

export async function PUT({ request }): Promise<Response> {
	const { good } = await request.json();
	console.log(good);
	const response = await fetch(env.PRIVATE_PANTRY_API_URL + '/goods/', {
		method: 'PUT',
		body: JSON.stringify(good),
		headers: {
			'Content-Type': 'application/json'
		}
	});

	return response;
}

export async function POST({ request }): Promise<Response> {
	const { createDto } = await request.json();
	const response = await fetch(env.PRIVATE_PANTRY_API_URL + '/goods/', {
		method: 'POST',
		body: JSON.stringify(createDto),
		headers: {
			'Content-Type': 'application/json'
		}
	});

	return response;
}

export async function GET(): Promise<Response> {
	const response = await fetch(env.PRIVATE_PANTRY_API_URL + '/goods/overview');
	return response;
}
