<script lang="ts">
	import { InputChip, popup, Modal, getModalStore, storePopup } from '@skeletonlabs/skeleton';
	import type { ModalSettings, PopupSettings } from '@skeletonlabs/skeleton';
	import { computePosition, autoUpdate, offset, shift, flip, arrow } from '@floating-ui/dom';
	import type { Good } from '$lib/modules/goods/types/Good.js';

	export let data;
	let good: Good;
	good = data.good;
	let goodBackUp = JSON.stringify(data.good);

	const modalStore = getModalStore();
	storePopup.set({ computePosition, autoUpdate, offset, shift, flip, arrow });

	const popupClick: PopupSettings = {
		event: 'click',
		target: 'popupClick',
		placement: 'bottom',
		closeQuery: '#edit, #delete, #cancel'
	};

	async function deleteGood() {
		const id = good.id;
		const response: Response = await fetch('/goods', {
			method: 'DELETE',
			body: JSON.stringify({ id })
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

	function resetGood() {
		good = JSON.parse(goodBackUp);
		inEdidtMode = false;
	}

	let inEdidtMode = false;
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

<label class="label">
	<span>Name</span>
	<input
		class="input rounded-md p-2"
		type="text"
		placeholder="Name"
		bind:value={good.name}
		readonly={!inEdidtMode}
	/>
</label>
{#if !inEdidtMode && good.description != null && good.description.length > 0}
	<label class="label">
		<span>Beschreibung</span>
		<textarea
			class="textarea p-2"
			rows="4"
			placeholder="Beschreibung"
			bind:value={good.description}
			readonly={!inEdidtMode}
		/>
	</label>
{:else if inEdidtMode}
	<label class="label">
		<span>Beschreibung</span>
		<textarea
			class="textarea p-2"
			rows="4"
			placeholder="Beschreibung"
			bind:value={good.description}
			readonly={!inEdidtMode}
		/>
	</label>
{/if}

<label class="label">
	<span>Menge</span>
	<input
		class="input rounded-md p-2"
		type="number"
		placeholder="1"
		bind:value={good.amount}
		readonly={!inEdidtMode}
	/>
</label>

{#if !inEdidtMode && good.minimumAmount != null && good.minimumAmount > 0}
	<label class="label">
		<span>Mindestbestand</span>
		<input
			class="input rounded-md p-2"
			type="number"
			placeholder="1"
			bind:value={good.minimumAmount}
			readonly={!inEdidtMode}
		/>
	</label>
{:else if inEdidtMode}
	<label class="label">
		<span>Mindestbestand</span>
		<input
			class="input rounded-md p-2"
			type="number"
			placeholder="1"
			bind:value={good.minimumAmount}
			readonly={!inEdidtMode}
		/>
	</label>
{/if}

{#if !inEdidtMode && good.storageLocation != null && good.storageLocation.length > 0}
	<label class="label">
		<span>Lagerort</span>
		<input
			class="input rounded-md p-2"
			type="text"
			placeholder="Lagerort"
			bind:value={good.storageLocation}
			readonly={!inEdidtMode}
		/>
	</label>
{:else if inEdidtMode}
	<label class="label">
		<span>Lagerort</span>
		<input
			class="input rounded-md p-2"
			type="text"
			placeholder="Lagerort"
			bind:value={good.storageLocation}
			readonly={!inEdidtMode}
		/>
	</label>
{/if}

{#if !inEdidtMode && good.ean != null && good.ean > 0}
	<label class="label">
		<span>EAN</span>
		<input
			class="input rounded-md p-2"
			type="number"
			placeholder="100000000"
			bind:value={good.ean}
			readonly={!inEdidtMode}
		/>
	</label>
{:else if inEdidtMode}
	<label class="label">
		<span>EAN</span>
		<input
			class="input rounded-md p-2"
			type="number"
			placeholder="100000000"
			bind:value={good.ean}
			readonly={!inEdidtMode}
		/>
	</label>
{/if}

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

{#if !inEdidtMode && good.shoppinglistName != null && good.shoppinglistName.length > 0}
	<label class="label">
		<span>Einkaufslistenname</span>
		<input
			class="input rounded-md p-2"
			type="text"
			placeholder="Einkaufslistenname"
			value={good.shoppinglistName}
			readonly={!inEdidtMode}
		/>
	</label>
{:else if inEdidtMode}
	<label class="label">
		<span>Einkaufslistenname</span>
		<input
			class="input rounded-md p-2"
			type="text"
			placeholder="Einkaufslistenname"
			value={good.shoppinglistName}
			readonly={!inEdidtMode}
		/>
	</label>
{/if}

{#if !inEdidtMode && good.description != null && good.description.length > 0}
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
