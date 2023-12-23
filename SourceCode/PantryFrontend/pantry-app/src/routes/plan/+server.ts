import { env } from '$env/dynamic/private';
import { getSession } from '@auth/sveltekit';
import { redirect } from '@sveltejs/kit';

export async function POST({ request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}

	const { createMealRequest } = await request.json();
	const response = await fetch(env.PRIVATE_PLAN_API_URL + '/meals', {
		method: 'POST',
		body: JSON.stringify(createMealRequest),
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

	const { date } = await request.json();
	const response = await fetch(env.PRIVATE_RECIPE_API_URL + '/meals/date/' + date, {
		method: 'GET',
		headers: {
			UserEMail: session?.user?.email
		}
	});
	return response;
}
