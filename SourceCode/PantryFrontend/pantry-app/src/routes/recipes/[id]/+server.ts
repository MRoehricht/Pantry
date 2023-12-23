import { env } from '$env/dynamic/private';
import { getSession } from '@auth/sveltekit';
import { redirect } from '@sveltejs/kit';

export async function DELETE({ params, request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}

	const response = await fetch(env.PRIVATE_RECIPE_API_URL + '/recipes/' + params.id, {
		method: 'DELETE',
		headers: {
			UserEMail: session?.user?.email
		}
	});

	return response;
}

export async function POST({ request, params }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}

	const { rating } = await request.json();

	const response = await fetch(
		env.PRIVATE_RECIPE_API_URL + '/recipedetails/review/' + params.id + '?review=' + rating,
		{
			method: 'POST',
			body: JSON.stringify(rating),
			headers: {
				'Content-Type': 'application/json',
				UserEMail: session?.user?.email
			}
		}
	);

	return response;
}

export async function GET({ params, request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}

	const response = await fetch(env.PRIVATE_RECIPE_API_URL + '/recipes/overview/' + params.id, {
		method: 'GET',
		headers: {
			UserEMail: session?.user?.email
		}
	});
	return response;
}
