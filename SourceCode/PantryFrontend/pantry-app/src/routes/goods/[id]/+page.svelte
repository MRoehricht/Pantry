<script lang="ts">
	import { InputChip, popup, getModalStore } from '@skeletonlabs/skeleton';
	import type { ModalSettings, PopupSettings } from '@skeletonlabs/skeleton';
	import {
		getUnitOfMeasurementDisplayName,
		UnitOfMeasurementDisplayName,
		type Good,
		type GoodRatingCreateDto
	} from '$lib/modules/goods/types/Good.js';
	import StringItemLable from '$lib/components/ItemDetails/StringItemLabel.svelte';
	import NumberItemLabel from '$lib/components/ItemDetails/NumberItemLabel.svelte';
	import TextareaItemLabel from '$lib/components/ItemDetails/TextareaItemLabel.svelte';
	import StringItemLabel from '$lib/components/ItemDetails/StringItemLabel.svelte';
	import ItemTextLabel from '$lib/components/ItemDetails/ItemTextLabel.svelte';
	import ItemNumberDetails from '$lib/components/ItemDetails/ItemNumberDetails.svelte';

	export let data;
	let good: Good;
	good = data.good;
	let goodBackUp = JSON.stringify(data.good);

	const modalStore = getModalStore();

	const popupClick: PopupSettings = {
		event: 'click',
		target: 'popupClick',
		placement: 'bottom',
		closeQuery: '#edit, #delete, #cancel'
	};

	async function deleteGood() {
		const response: Response = await fetch('/goods/' + good.id, {
			method: 'DELETE'
		});

		if (!response.ok) {
			const modalSettingDeleteError: ModalSettings = {
				type: 'alert',
				title: 'Fehler',
				body: 'Beim Löschen ist ein Fehler aufgetreten.'
			};
			modalStore.trigger(modalSettingDeleteError);
			throw new Error(`HTTP error! status: ${response.status}`);
		} else {
			window.location.href = '/goods';
		}
	}

	async function putGood() {
		const response: Response = await fetch('/goods', {
			method: 'PUT',
			body: JSON.stringify({ good })
		});

		if (!response.ok) {
			const modalSettingPutError: ModalSettings = {
				type: 'alert',
				title: 'Fehler',
				body: 'Beim Speichern ist ein Fehler aufgetreten.'
			};
			modalStore.trigger(modalSettingPutError);
			throw new Error(`HTTP error! status: ${response.status}`);
		} else {
			inEdidtMode = false;
		}
	}

	const modalSettingDelete: ModalSettings = {
		type: 'confirm',
		buttonTextCancel: 'Abbrechen',
		buttonTextConfirm: 'Löschen',
		title: 'Ware löschen',
		body: 'Wollen Sie diese Ware wirklich löschen?',
		response: async (confirmed: boolean) => {
			if (confirmed) {
				await deleteGood();
			}
		}
	};

	function showRatingModal() {
		const modal: ModalSettings = {
			title: 'Ware bewerten',
			body: 'Bewerten Sie diese Ware.',
			buttonTextCancel: 'Abbrechen',
			type: 'component',
			component: 'RatingModal',
			response: async (rating: number) => {
				if (rating && rating > 0) {
					const goodRatingCreateDto: GoodRatingCreateDto = {
						goodId: good.id,
						rating: rating
					};
					const response: Response = await fetch('/goods/' + good.id, {
						method: 'POST',
						body: JSON.stringify({ goodRatingCreateDto })
					});

					if (!response.ok) {
						const modalSettingPutError: ModalSettings = {
							type: 'alert',
							title: 'Fehler',
							body: 'Beim Speichern ist ein Fehler aufgetreten.'
						};
						modalStore.trigger(modalSettingPutError);
						throw new Error(`HTTP error! status: ${response.status}`);
					}
				}
			}
		};
		modalStore.trigger(modal);
	}

	function resetGood() {
		good = JSON.parse(goodBackUp);
		inEdidtMode = false;
	}

	let inEdidtMode = false;

	$: if (good.amount == null) {
		good.amount = 0;
	}
</script>

<div class="grid grid-cols-2 md:grid-cols-2 gap-2">
	<div class="flex items-center">
		<a href="/goods">
			<button type="button" class="btn-icon variant-filled-primary rounded-md"
				><i class="fa-solid fa-caret-left"></i>
			</button>
		</a>
		<h1 class="h1 ml-5 mr-5">{good.name}</h1>
	</div>
	<div class="justify-self-end justify-self flex items-center">
		<button class="btn-icon variant-filled-secondary rounded-md" use:popup={popupClick}
			><i class="fa-solid fa-ellipsis-vertical"></i>
		</button>
	</div>
</div>

<div class="card p-4 max-w-sm" data-popup="popupClick">
	<div class="grid grid-cols-1 gap-2">
		<button id="rate" class="btn variant-filled-primary" on:click={showRatingModal}
			>Bewerten</button
		>

		{#if inEdidtMode == true}
			<button id="cancel" class="btn variant-filled-surface" on:click={resetGood}
				>Abbrechen</button
			>
		{:else}
			<button
				id="edit"
				class="btn variant-filled-surface"
				on:click={() => (inEdidtMode = true)}>Bearbeiten</button
			>
		{/if}

		<button
			id="delete"
			class="btn variant-filled-error"
			on:click={() => {
				modalStore.trigger(modalSettingDelete);
			}}>Löschen</button
		>
	</div>
	<div class="arrow bg-surface-100-800-token" />
</div>

<ItemTextLabel bind:value={good.name} label="Name" {inEdidtMode} />
<TextareaItemLabel bind:value={good.description} label="Beschreibung" {inEdidtMode} />
<ItemNumberDetails bind:value={good.amount} label="Menge" {inEdidtMode} placeholder="1" />
<NumberItemLabel
	bind:value={good.minimumAmount}
	label="Mindestbestand"
	{inEdidtMode}
	placeholder="1"
/>

<label class="label">
	<span>Maßeinheit</span>
	{#if inEdidtMode == false}
		<input
			class="input rounded-md p-2"
			type="text"
			value={getUnitOfMeasurementDisplayName(good.unitOfMeasurement)}
			readonly
		/>
	{:else}
		<select bind:value={good.unitOfMeasurement} class="input rounded-md p-2">
			{#each Object.entries(UnitOfMeasurementDisplayName) as [unit, displayName]}
				<option value={Number(unit)}>{displayName}</option>
			{/each}
		</select>
	{/if}
</label>

<StringItemLabel bind:value={good.storageLocation} label="Lagerort" {inEdidtMode} />
<NumberItemLabel bind:value={good.ean} label="EAN" {inEdidtMode} placeholder="9846568745" />

{#if !inEdidtMode && good.currentPrice != null && good.currentPrice > 0}
	<p>Derzeitiger Preis</p>
	<div class="input-group input-group-divider grid-cols-[auto_1fr_auto] rounded-md">
		<div class="input-group-shim"><i class="fa-solid fa-euro-sign"></i></div>
		<input
			class="input rounded-md p-2"
			type="number"
			placeholder="1.99"
			bind:value={good.currentPrice}
			readonly={!inEdidtMode}
		/>
	</div>
{:else if inEdidtMode}
	<p>Derzeitiger Preis</p>
	<div class="input-group input-group-divider grid-cols-[auto_1fr_auto] rounded-md">
		<div class="input-group-shim"><i class="fa-solid fa-euro-sign"></i></div>
		<input
			class="input rounded-md p-2"
			type="number"
			placeholder="1.99"
			bind:value={good.currentPrice}
			readonly={!inEdidtMode}
		/>
	</div>
{/if}

<StringItemLabel bind:value={good.shoppinglistName} label="Einkaufslistenname" {inEdidtMode} />

{#if !inEdidtMode && good.details.tags != null && good.details.tags.length > 0}
	<div>
		{#each good.details.tags as tag}
			<span class="chip variant-filled m-2">{tag}</span>
		{/each}
	</div>
{:else if inEdidtMode}
	<label for="Tags" class="label">
		<span>Tags</span>
		<InputChip
			padding="p-2"
			allowUpperCase={true}
			name="Tags"
			bind:value={good.details.tags}
			placeholder="Add Tag..."
		/>
	</label>
{/if}

{#if inEdidtMode}
	<div class="grid grid-flow-col justify-stretch mt-5">
		<button
			type="button"
			class="btn variant-filled-success flex self-stretch"
			on:click={async () => putGood()}
		>
			<span><i class="fa-solid fa-floppy-disk"></i></span>
			<span>Speichern</span>
		</button>
	</div>
{/if}
