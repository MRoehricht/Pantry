import { env } from '$env/dynamic/private';
import { getSession } from '@auth/sveltekit';
import { redirect } from '@sveltejs/kit';

export async function PUT({ request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}

	const { recipe } = await request.json();
	const response = await fetch(env.PRIVATE_RECIPE_API_URL + '/recipes/', {
		method: 'PUT',
		body: JSON.stringify(recipe),
		headers: {
			'Content-Type': 'application/json',
			UserEMail: session?.user?.email
		}
	});

	return response;
}

export async function POST({ request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}

	const { createDto } = await request.json();
	const response = await fetch(env.PRIVATE_RECIPE_API_URL + '/recipes/', {
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

	const response = await fetch(env.PRIVATE_RECIPE_API_URL + '/recipes/overview', {
		method: 'GET',
		headers: {
			UserEMail: session?.user?.email
		}
	});
	return response;
}

export async function PATCH({ request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}

	const { id } = await request.json();
	const response = await fetch(env.PRIVATE_RECIPE_API_URL + '/recipes/' + id, {
		method: 'PATCH',
		headers: {
			UserEMail: session?.user?.email
		}
	});
	return response;
}
