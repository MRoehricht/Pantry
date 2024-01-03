<script lang="ts">
	import { onMount, type SvelteComponent } from 'svelte';
	import {
		Autocomplete,
		Ratings,
		getModalStore,
		popup,
		type AutocompleteOption,
		type PopupSettings
	} from '@skeletonlabs/skeleton';
	import type { RecipeOverview } from '$lib/modules/recipe/types/Recipe';

	// Props
	/** Exposes parent props to this component. */
	export let parent: SvelteComponent;

	const modalStore = getModalStore();
	let showNameEmptyError = false;
	let showRecipeNotFoundError = false;
	let recipes: RecipeOverview[] = [];
	let selectedRecipe: RecipeOverview = {
		name: '',
		id: '',
		description: '',
		rating: 0,
		tags: []
	};

	// Handle Form Submission
	function onFormSubmit(): void {
		if (selectedRecipe.name == null || selectedRecipe.name.length < 1) {
			showNameEmptyError = true;
		}
		if (selectedRecipe.id == null || selectedRecipe.id.length < 1) {
			let foundRecipe = recipes.find((recipe) => recipe.name == selectedRecipe.name);
			if (foundRecipe) {
				selectedRecipe.id = foundRecipe.id;
			} else {
				showRecipeNotFoundError = true;
			}
		}

		if (showNameEmptyError || showRecipeNotFoundError) return;

		if ($modalStore[0].response) $modalStore[0].response(selectedRecipe);
		modalStore.close();
	}

	// Base Classes
	const cBase = 'card p-4 w-modal shadow-xl space-y-4';
	const cHeader = 'text-2xl font-bold';

	type RecipeOption = AutocompleteOption<string>;
	let recipeOptions: RecipeOption[];

	onMount(async () => {
		const response: Response = await fetch('/recipes', {
			method: 'GET'
		});
		recipes = await response.json();
		recipeOptions = recipes.map((recipe) => {
			return {
				label: recipe.name,
				value: recipe.id
			};
		});
	});

	const popupRecipeSettings: PopupSettings = {
		event: 'focus-click',
		target: 'popupAutocomplete',
		placement: 'bottom-start'
	};

	function onPopupSelect(event: CustomEvent<RecipeOption>): void {
		selectedRecipe.name = event.detail.label;
		selectedRecipe.id = event.detail.value;
	}
</script>

<!-- @component This example creates a simple form modal. -->

{#if $modalStore[0]}
	<div class="modal-example-form {cBase}">
		<header class={cHeader}>{$modalStore[0].title ?? '(title missing)'}</header>
		<label class="label mb-2">
			<span>Name</span>
			<input
				class="input rounded-md p-2 autocomplete"
				name="autocomplete-search"
				type="text"
				bind:value={selectedRecipe.name}
				placeholder="Name..."
				minlength="1"
				use:popup={popupRecipeSettings}
			/>
			<div data-popup="popupAutocomplete" class="card popup-box">
				<Autocomplete
					bind:input={selectedRecipe.name}
					options={recipeOptions}
					on:selection={onPopupSelect}
					limit={5}
					emptyState="Keine Rezepte gefunden"
				/>
			</div>
			{#if showNameEmptyError}
				<span class="text-error-500">Bitte vergeben Sie ein Rezept.</span>
			{/if}
			{#if showRecipeNotFoundError}
				<span class="text-error-500"
					>Das von Ihnen gewählte Rezept ist nicht vorhanden.</span
				>
			{/if}
		</label>

		<!-- prettier-ignore -->
		<footer class="modal-footer {parent.regionFooter}">
        <button class="btn {parent.buttonNeutral}" on:click={parent.onClose}>{parent.buttonTextCancel}</button>
        <button class="btn {parent.buttonPositive}" on:click={onFormSubmit}>Hinzufügen</button>
    </footer>
	</div>
{/if}

<style>
	.popup-box {
		z-index: 10;
	}
</style>
