<script lang="ts">
	import { RadioGroup, RadioItem } from '@skeletonlabs/skeleton';
	import GoodCard from '$lib/modules/goods/components/GoodCard.svelte';
	import type { Good } from '$lib/modules/goods/types/Good';
	import { Modal, getModalStore } from '@skeletonlabs/skeleton';
	import type { ModalSettings, ModalComponent, ModalStore } from '@skeletonlabs/skeleton';

	const modalStore = getModalStore();
	let total: Good[] = [];

	async function add() {
		const response: Response = await fetch('/goods', { method: 'GET' });
		total = await response.json();
	}

	export let data;
	const { goods } = data;

	let value: number = 0;

	const modal: ModalSettings = {
		type: 'prompt',
		// Data
		title: 'Enter Name',
		body: 'Provide your first name in the field below.',
		// Populates the input value and attributes
		value: 'Skeleton',
		valueAttr: { type: 'text', minlength: 3, maxlength: 10, required: true },
		// Returns the updated response value
		response: (r: string) => console.log('response:', r)
	};
</script>

<div class="grid grid-cols-1 md:grid-cols-2 gap-2">
	<div class="flex justify-start">
		<h1 class="h1 mb-5">Waren</h1>
		<button
			type="button"
			class="btn-icon variant-filled justify-self-center mb-5 ml-5 mt-2"
			on:click={() => modalStore.trigger(modal)}><i class="fa-solid fa-plus"></i></button
		>
	</div>

	<div class="justify-self-end mb-5">
		<RadioGroup active="variant-filled-primary" hover="hover:variant-soft-primary">
			<RadioItem bind:group={value} name="justify" value={0}>Alphabetisch</RadioItem>
			<RadioItem bind:group={value} name="justify" value={1}>Bewertung</RadioItem>
			<RadioItem bind:group={value} name="justify" value={2}>Tags</RadioItem>
			<!--<RadioItem bind:group={value} name="justify" value={3}>Lagerort</RadioItem>-->
		</RadioGroup>
	</div>
</div>

{#await data}
	<p>Loading</p>
{:then c}
	<div class="grid grid-cols-2 md:grid-cols-4 gap-4">
		{#each goods as good}
			<GoodCard {good} />
		{/each}
	</div>
{/await}
