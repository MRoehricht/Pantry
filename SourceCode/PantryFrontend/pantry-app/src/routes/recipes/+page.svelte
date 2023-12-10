<script lang="ts">
	import RecipeCard from '$lib/modules/recipe/components/RecipeCard.svelte';

	export let data;
	const { recipes } = data;
</script>

<h1 class="h1 mb-5">Rezepte</h1>
{#await data}
	<p>Loading</p>
{:then c}
	<div class="grid grid-cols-2 md:grid-cols-4 gap-4">
		{#each recipes as recipe}
			<RecipeCard {recipe} />
		{/each}
	</div>
{/await}
