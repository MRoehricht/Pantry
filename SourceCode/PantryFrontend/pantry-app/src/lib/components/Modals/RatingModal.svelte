<script lang="ts">
	import type { SvelteComponent } from 'svelte';
	import { Ratings, getModalStore } from '@skeletonlabs/skeleton';

	// Props
	/** Exposes parent props to this component. */
	export let parent: SvelteComponent;

	const modalStore = getModalStore();

	// Handle Form Submission
	function onFormSubmit(): void {
		if ($modalStore[0].response) $modalStore[0].response(value.current);
		modalStore.close();
	}

	// Base Classes
	const cBase = 'card p-4 w-modal shadow-xl space-y-4';
	const cHeader = 'text-2xl font-bold';

	let value = { current: 0, max: 5 };

	function iconClick(event: CustomEvent<{ index: number }>): void {
		value.current = event.detail.index;
	}
</script>

<!-- @component This example creates a simple form modal. -->

{#if $modalStore[0]}
	<div class="modal-example-form {cBase}">
		<header class={cHeader}>{$modalStore[0].title ?? '(title missing)'}</header>
		<article>{$modalStore[0].body ?? '(body missing)'}</article>

		<Ratings bind:value={value.current} max={value.max} interactive on:icon={iconClick}>
			<svelte:fragment slot="empty"
				><i class="fa-regular fa-star text-3xl"></i></svelte:fragment
			>
			<svelte:fragment slot="half"
				><i class="fa-regular fa-star-half-stroke text-3xl"></i></svelte:fragment
			>
			<svelte:fragment slot="full"><i class="fa-solid fa-star text-3xl"></i></svelte:fragment>
		</Ratings>

		<!-- prettier-ignore -->
		<footer class="modal-footer {parent.regionFooter}">
        <button class="btn {parent.buttonNeutral}" on:click={parent.onClose}>{parent.buttonTextCancel}</button>
        <button class="btn {parent.buttonPositive}" on:click={onFormSubmit}>Bewerten</button>
    </footer>
	</div>
{/if}
