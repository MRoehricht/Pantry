<script lang="ts">
	import GoodCard from '$lib/modules/goods/components/GoodCard.svelte';
	import type { Good } from '$lib/modules/goods/types/Good';
	let total: Good[] = [];

	async function add() {
		const response: Response = await fetch('/goods', { method: 'GET' });
		total = await response.json();
	}

	export let data;
	const { goods } = data;
</script>

<h1 class="h1 mb-5">Waren</h1>

{#await data}
	<p>Loading</p>
{:then c}
	<div class="grid grid-cols-2 md:grid-cols-4 gap-4">
		{#each goods as good}
			<GoodCard {good} />
		{/each}
	</div>
{/await}
