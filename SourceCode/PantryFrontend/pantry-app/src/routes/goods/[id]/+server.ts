import { env } from '$env/dynamic/private';
import { getSession } from '@auth/sveltekit';
import { redirect } from '@sveltejs/kit';

export async function DELETE({ params, request }): Promise<Response> {
	const session = await getSession(request, { providers: [] });
	if (!session?.user?.email) {
		throw redirect(307, '/');
	}

	const response = await fetch(env.PRIVATE_PANTRY_API_URL + '/goods/' + params.id, {
		method: 'DELETE',
		headers: {
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

	const { goodRatingCreateDto } = await request.json();
	const response = await fetch(
		env.PRIVATE_PANTRY_API_URL + '/goodratings/' + goodRatingCreateDto.GoodId,
		{
			method: 'POST',
			body: JSON.stringify(goodRatingCreateDto),
			headers: {
				'Content-Type': 'application/json',
				UserEMail: session?.user?.email
			}
		}
	);

	return response;
}
