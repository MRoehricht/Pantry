import { env } from '$env/dynamic/private';

export async function GET(): Promise<Response> {
	const response = await fetch(env.PRIVATE_PANTRY_API_URL + '/suggestions');
	return response;
}
