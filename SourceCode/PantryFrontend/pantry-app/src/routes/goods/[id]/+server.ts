import { env } from '$env/dynamic/private';

export async function DELETE({ request }): Promise<Response> {
	const { id } = await request.json();
	const response = await fetch(env.PRIVATE_PANTRY_API_URL + '/goods/' + id, {
		method: 'DELETE'
	});

	return response;
}

export async function POST({ request }): Promise<Response> {
	const { goodRatingCreateDto } = await request.json();
	const response = await fetch(
		env.PRIVATE_PANTRY_API_URL + '/goodratings/' + goodRatingCreateDto.GoodId,
		{
			method: 'POST',
			body: JSON.stringify(goodRatingCreateDto),
			headers: {
				'Content-Type': 'application/json'
			}
		}
	);

	return response;
}
