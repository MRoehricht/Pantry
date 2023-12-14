<script lang="ts">
	import { RadioGroup, RadioItem, getModalStore, getToastStore } from '@skeletonlabs/skeleton';
	import Card from '$lib/components/OverviewCard.svelte';
	import type { Good, GoodCreateDto, GoodOverview } from '$lib/modules/goods/types/Good';
	import type { ModalSettings, ToastSettings } from '@skeletonlabs/skeleton';
	import { onMount } from 'svelte';
	import { Orderings } from '$lib/types/Orderings';
	import OverviewCard from '$lib/components/OverviewCard.svelte';
	import type { OverviewItem, OverviewItemCreateDto } from '$lib/modules/core/types/Core';

	export let url: string; // /goods
	export let overviewContext: string;
	export let createTitle: string = 'Erstellen';
	export let createBody: string = 'Geben Sie den Namen an.';

	const modalStore = getModalStore();
	const toastStore = getToastStore();

	let overviewItems: OverviewItem[] = [];
	let currentOrdering: Orderings = Orderings.Alphabetical;
	let isLoading = true;
	let showRating: boolean = false;
	let showTags: boolean = false;

	onMount(async () => {
		isLoading = true;
		overviewItems = await getOverviewItems();
		isLoading = false;
	});

	const modal: ModalSettings = {
		type: 'prompt',
		buttonTextCancel: 'Abbrechen',
		buttonTextSubmit: 'Erstellen',
		title: createTitle,
		body: createBody,
		value: '',
		valueAttr: {
			type: 'text',
			minlength: 1,
			required: true,
			placeholder: 'Name',
			class: 'modal-prompt-input input rounded-md p-2'
		},
		response: (response: string) => {
			if (response && response.length > 0) {
				postOverviewItem(response);
			}
		}
	};

	async function postOverviewItem(name: string) {
		if (name == null || name.length == 0) return;
		const createDto: OverviewItemCreateDto = {
			name: name,
			description: null
		};
		const response: Response = await fetch(url, {
			method: 'POST',
			body: JSON.stringify({ createDto })
		});

		if (!response.ok) {
			const modalSettingPutError: ModalSettings = {
				type: 'alert',
				title: 'Fehler',
				body: 'Beim Erstellen ist ein Fehler aufgetreten.'
			};
			modalStore.trigger(modalSettingPutError);
			throw new Error(`HTTP error! status: ${response.status}`);
		} else {
			var itemRespose: OverviewItem = await response.json();
			const t: ToastSettings = {
				action: {
					label: 'öffnen',
					response: () => (window.location.href = url + '/' + itemRespose.id)
				},
				message: name + ' wurde erstellt.'
			};
			toastStore.trigger(t);
			overviewItems = await getOverviewItems();
		}
	}

	async function getOverviewItems(): Promise<OverviewItem[]> {
		isLoading = true;
		const response: Response = await fetch(url, {
			method: 'GET'
		});
		isLoading = false;
		if (!response.ok) {
			const modalSettingPutError: ModalSettings = {
				type: 'alert',
				title: 'Fehler',
				body: 'Beim Laden ist ein Fehler aufgetreten.'
			};
			modalStore.trigger(modalSettingPutError);
			throw new Error(`HTTP error! status: ${response.status}`);
		} else {
			const overviewItems: OverviewItem[] = await response.json();
			return overviewItems;
		}
	}

	async function reorderOverviewItems(ordering: Orderings) {
		if (isLoading) return;
		showRating = false;
		showTags = false;
		if (ordering == Orderings.Alphabetical) {
			overviewItems = (await getOverviewItems()).sort((a, b) => a.name.localeCompare(b.name));
		} else if (ordering == Orderings.Reviews) {
			showRating = true;
			overviewItems = (await getOverviewItems()).sort((a, b) => {
				if (a.rating === null && b.rating === null) {
					return a.name.localeCompare(b.name);
				}
				if (a.rating === null) {
					return 1;
				}
				if (b.rating === null) {
					return -1;
				}
				return b.rating - a.rating;
			});
		} else if (ordering == Orderings.Tags) {
			showTags = true;
			overviewItems = (await getOverviewItems()).sort((a, b) => {
				if (a.tags.length === 0 && b.tags.length === 0) {
					return a.name.localeCompare(b.name);
				}
				if (a.tags.length === 0) {
					return 1;
				}
				if (b.tags.length === 0) {
					return -1;
				}
				return a.tags[0].localeCompare(b.tags[0]);
			});
		}
	}

	$: reorderOverviewItems(currentOrdering);
</script>

<div class="grid grid-cols-1 md:grid-cols-2 gap-2">
	<div class="flex justify-start">
		<h1 class="h1 mb-5">{overviewContext}</h1>
		<button
			type="button"
			class="btn-icon variant-filled justify-self-center mb-5 ml-5 mt-2"
			on:click={() => modalStore.trigger(modal)}><i class="fa-solid fa-plus"></i></button
		>
	</div>

	<div class="justify-self-end mb-5">
		<RadioGroup active="variant-filled-primary" hover="hover:variant-soft-primary">
			<RadioItem bind:group={currentOrdering} name="justify" value={0}>Alphabetisch</RadioItem
			>
			<RadioItem bind:group={currentOrdering} name="justify" value={1}>Bewertung</RadioItem>
			<RadioItem bind:group={currentOrdering} name="justify" value={2}>Tags</RadioItem>
		</RadioGroup>
	</div>
</div>
<!-- 
{#if isLoading}
	<div class="grid grid-cols-2 md:grid-cols-4 gap-4">
		{#each Array(4) as _, index (index)}
			<div class="placeholder animate-pulse">
				<div
					class="block max-w-sm p-6 bg-white border border-gray-200 rounded-lg shadow hover:bg-gray-100 dark:bg-gray-800 dark:border-gray-700 dark:hover:bg-gray-700"
				>
					<h5 class="mb-2 text-2xl font-bold tracking-tight dark:text-white">...</h5>
					<p class="font-normal text-gray-700 dark:text-gray-400">...</p>
				</div>
			</div>
		{/each}
	</div>
	
{:else}
	<div class="grid grid-cols-2 md:grid-cols-4 gap-4">
		{#each goods as good}
			<GoodCard {good} {showRating} />
		{/each}
	</div>
{/if}
-->

<div class="grid grid-cols-2 md:grid-cols-4 gap-4">
	{#each overviewItems as item}
		<OverviewCard {url} {item} {showRating} {showTags} />
	{/each}
</div>
