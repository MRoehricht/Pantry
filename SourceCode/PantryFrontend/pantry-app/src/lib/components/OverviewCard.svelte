<script lang="ts">
	import type { OverviewItem } from '$lib/modules/core/types/Core';
	import { Ratings } from '@skeletonlabs/skeleton';

	export let url: string = '';
	export let showRating: boolean = false;
	export let showTags: boolean = false;
	export let item: OverviewItem;
</script>

<a
	href="{url}/{item.id}"
	class="block max-w-sm p-6 bg-white border border-gray-200 rounded-lg shadow hover:bg-gray-100 dark:bg-gray-800 dark:border-gray-700 dark:hover:bg-gray-700 card-hover"
>
	<h5 class="mb-2 text-2xl font-bold tracking-tight dark:text-white">
		{item.name}
	</h5>
	<p class="font-normal text-gray-700 dark:text-gray-400">
		{#if !showRating && !showTags && item.description && item.description.length > 0}
			{item.description}
		{/if}
		{#if showRating && item.rating != null}
			<Ratings value={item.rating} max={5}>
				<svelte:fragment slot="empty"><i class="fa-regular fa-star"></i></svelte:fragment>
				<svelte:fragment slot="half"
					><i class="fa-regular fa-star-half-stroke"></i></svelte:fragment
				>
				<svelte:fragment slot="full"><i class="fa-solid fa-star"></i></svelte:fragment>
			</Ratings>
		{/if}
		{#if showTags && item.tags && item.tags.length > 0}
			{#each item.tags as tag}
				<span class="chip variant-filled m-2">{tag}</span>
			{/each}
		{/if}
	</p>
</a>
