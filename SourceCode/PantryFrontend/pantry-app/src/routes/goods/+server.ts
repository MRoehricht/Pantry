import { env } from '$env/dynamic/private';
import { getSession } from '@auth/sveltekit';
import { redirect } from '@sveltejs/kit';

export async function PUT({ request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}
	console.log('PUT');
	console.log(session);

	const { good } = await request.json();
	console.log(good);
	const response = await fetch(env.PRIVATE_PANTRY_API_URL + '/goods/', {
		method: 'PUT',
		body: JSON.stringify(good),
		headers: {
			'Content-Type': 'application/json',
			UserEMail: session?.user?.email
		}
	});
	console.log(response);
	return response;
}

export async function POST({ request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}
	console.log('POST');
	console.log(session);
	const { createDto } = await request.json();
	const response = await fetch(env.PRIVATE_PANTRY_API_URL + '/goods/', {
		method: 'POST',
		body: JSON.stringify(createDto),
		headers: {
			'Content-Type': 'application/json',
			UserEMail: session?.user?.email
		}
	});

	return response;
}

export async function GET({ request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}
	const response = await fetch(env.PRIVATE_PANTRY_API_URL + '/goods/overview', {
		method: 'GET',
		headers: {
			UserEMail: session?.user?.email
		}
	});
	return response;
}
