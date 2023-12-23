import { env } from '$env/dynamic/private';
import { getSession } from '@auth/sveltekit';
import { redirect } from '@sveltejs/kit';

export async function GET({ params, request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}

	const response = await fetch(env.PRIVATE_PLAN_API_URL + '/meals/date/' + params.date, {
		method: 'GET',
		headers: {
			UserEMail: session?.user?.email
		}
	});
	return response;
}
