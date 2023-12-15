import { env } from '$env/dynamic/private';
export async function GET({ params }): Promise<Response> {
	const response = await fetch(env.PRIVATE_PLAN_API_URL + '/meals/date/' + params.date);
	return response;
}
