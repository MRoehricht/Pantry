<script lang="ts">
	import type { SvelteComponent } from 'svelte';
	import { Ratings, getModalStore, type ModalSettings } from '@skeletonlabs/skeleton';
	import type { Ingredient } from '$lib/modules/recipe/types/Recipe';

	// Props
	/** Exposes parent props to this component. */
	export let parent: SvelteComponent;
	let ingredient: Ingredient;
	let submitButtonLabel: string;
	let showNameError = false;
	let showUnitError = false;
	let showCountOffError = false;

	const modalStore = getModalStore();

	// Handle Form Submission
	function onFormSubmit(): void {
		showNameError = false;
		showUnitError = false;
		showCountOffError = false;

		if (ingredient.name == null || ingredient.name.length < 1) {
			showNameError = true;
		}
		if (ingredient.unit == null || ingredient.unit.length < 1) {
			showUnitError = true;
		}
		if (ingredient.countOff == null || ingredient.countOff <= 0) {
			showCountOffError = true;
		}
		if (showNameError || showUnitError || showCountOffError) return;

		if ($modalStore[0].response) $modalStore[0].response(ingredient);
		modalStore.close();
	}

	// Base Classes
	const cBase = 'card p-4 w-modal shadow-xl space-y-4';
	const cHeader = 'text-2xl font-bold';

	if ($modalStore[0]?.meta?.ingredient) {
		ingredient = $modalStore[0]?.meta?.ingredient;
		submitButtonLabel = 'Speichern';
	} else {
		ingredient = {
			name: '',
			unit: '',
			countOff: 1,
			pantryItemId: null
		};
		submitButtonLabel = 'Erstellen';
	}
</script>

<!-- @component This example creates a simple form modal. -->

{#if $modalStore[0]}
	<div class="modal-example-form {cBase}">
		<header class={cHeader}>{$modalStore[0].title ?? '(title missing)'}</header>

		<form class="modal-form">
			<label class="label mb-2">
				<span>Name</span>
				<input
					class="input rounded-md p-2"
					type="text"
					bind:value={ingredient.name}
					placeholder="Name..."
					minlength="1"
				/>
				{#if showNameError}
					<span class="text-error-500">Bitte vergeben Sie einen Namen.</span>
				{/if}
			</label>
			<div class="flex">
				<label class="label flex-auto pr-2">
					<span>Menge</span>
					<input
						class="input rounded-md p-2"
						type="number"
						bind:value={ingredient.countOff}
						placeholder="150..."
					/>
					{#if showCountOffError}
						<span class="text-error-500">Bitte vergeben Sie eine Menge.</span>
					{/if}
				</label>
				<label class="label flex-auto pl-2">
					<span>Einheit</span>
					<input
						class="input rounded-md p-2"
						type="text"
						bind:value={ingredient.unit}
						placeholder="g / ml / Stk..."
					/>
					{#if showUnitError}
						<span class="text-error-500">Bitte vergeben Sie eine Einheit.</span>
					{/if}
				</label>
			</div>
		</form>

		<!-- prettier-ignore -->
		<footer class="modal-footer {parent.regionFooter}">
        <button class="btn {parent.buttonNeutral}" on:click={parent.onClose}>{parent.buttonTextCancel}</button>
        <button class="btn {parent.buttonPositive}" on:click={onFormSubmit}>{submitButtonLabel}</button>
    </footer>
	</div>
{/if}
