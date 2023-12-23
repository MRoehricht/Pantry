import { env } from '$env/dynamic/private';
import { getSession } from '@auth/sveltekit';
import { redirect } from '@sveltejs/kit';

export async function GET({ params, request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}

	const response = await fetch(env.PRIVATE_PLAN_API_URL + '/meals/' + params.id, {
		method: 'GET',
		headers: {
			UserEMail: session?.user?.email
		}
	});
	return response;
}

export async function PUT({ request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}

	const { meal } = await request.json();
	const response = await fetch(env.PRIVATE_PLAN_API_URL + '/meals/', {
		method: 'PUT',
		body: JSON.stringify(meal),
		headers: {
			'Content-Type': 'application/json',
			UserEMail: session?.user?.email
		}
	});

	return response;
}
