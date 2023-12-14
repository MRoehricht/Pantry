<script lang="ts">
	import {
		InputChip,
		popup,
		Modal,
		getModalStore,
		storePopup,
		ProgressRadial,
		ProgressBar
	} from '@skeletonlabs/skeleton';
	import type { ModalSettings, PopupSettings } from '@skeletonlabs/skeleton';
	import { computePosition, autoUpdate, offset, shift, flip, arrow } from '@floating-ui/dom';
	import type { Good, GoodRatingCreateDto } from '$lib/modules/goods/types/Good.js';
	import type {
		Ingredient,
		IngredientCreateDto,
		Recipe
	} from '$lib/modules/recipe/types/Recipe.js';
	import StringItemLable from '$lib/components/ItemDetails/StringItemLabel.svelte';
	import ItemTextLabel from '$lib/components/ItemDetails/ItemTextLabel.svelte';
	import TextareaItemLabel from '$lib/components/ItemDetails/TextareaItemLabel.svelte';
	import { fade, fly } from 'svelte/transition';

	export let data;
	let recipe: Recipe;
	recipe = data.recipe;
	let backUpData = JSON.stringify(data.recipe);
	let inEdidtMode = false;
	let selectedIngredient: Ingredient | null = null;
	let loadingIngredient: number = -1;
	let selectedIngredientIndex: number = -1;
	let isSaving: boolean = false;

	const modalStore = getModalStore();
	storePopup.set({ computePosition, autoUpdate, offset, shift, flip, arrow });

	const popupClick: PopupSettings = {
		event: 'click',
		target: 'popupClick',
		placement: 'bottom',
		closeQuery: '#edit, #delete, #cancel'
	};

	const ingredientPopupClick: PopupSettings = {
		event: 'click',
		target: 'ingredientPopupClick',
		placement: 'bottom',
		closeQuery: '#editIngredient, #deleteIngredient'
	};

	async function remove() {
		isSaving = true;
		const response: Response = await fetch('/recipes/' + recipe.id, {
			method: 'DELETE'
		});
		isSaving = false;
		if (!response.ok) {
			const modalSettingDeleteError: ModalSettings = {
				type: 'alert',
				title: 'Fehler',
				body: 'Beim Löschen ist ein Fehler aufgetreten.'
			};
			modalStore.trigger(modalSettingDeleteError);
			//throw new Error(`HTTP error! status: ${response.status}`);
		} else {
			window.location.href = '/recipes';
		}
	}

	async function put() {
		isSaving = true;
		const response: Response = await fetch('/recipes', {
			method: 'PUT',
			body: JSON.stringify({ recipe })
		});

		isSaving = false;
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
				await remove();
			}
		}
	};

	const modalIngredientDelete: ModalSettings = {
		type: 'confirm',
		buttonTextCancel: 'Abbrechen',
		buttonTextConfirm: 'Löschen',
		title: 'Zutat löschen',
		body: 'Wollen Sie diese Zutat wirklich löschen?',
		response: async (confirmed: boolean) => {
			if (confirmed) {
				await removeIngredient();
			}
		}
	};

	function showIngredientEditModal(ingredient: Ingredient | null) {
		let name: string | null = null;
		if (ingredient != null) {
			name = ingredient.name;
		}

		const modal: ModalSettings = {
			title: ingredient == null ? 'Zutat erstellen' : 'Zutat bearbeiten',
			buttonTextCancel: 'Abbrechen',
			type: 'component',
			component: 'IngredientEditModal',
			meta: { ingredient: ingredient },
			response: async (resonse: Ingredient) => {
				if (resonse.name) {
					const ingredientCreateDto: IngredientCreateDto = {
						recipeId: recipe.id,
						ingredient: resonse,
						name: name
					};

					if (name == null) {
						loadingIngredient = selectedIngredientIndex;
						const response: Response = await fetch('/recipes/ingredients/', {
							method: 'POST',
							body: JSON.stringify({ ingredientCreateDto })
						});
						loadingIngredient = -1;
						if (!response.ok) {
							const modalSettingPutError: ModalSettings = {
								type: 'alert',
								title: 'Fehler',
								body: 'Beim Speichern ist ein Fehler aufgetreten.'
							};
							modalStore.trigger(modalSettingPutError);
						} else {
							recipe.ingredients.push(resonse);
						}
					} else {
						const response: Response = await fetch('/recipes/ingredients/', {
							method: 'PUT',
							body: JSON.stringify({ ingredientCreateDto })
						});
						loadingIngredient = -1;
						if (!response.ok) {
							const modalSettingPutError: ModalSettings = {
								type: 'alert',
								title: 'Fehler',
								body: 'Beim Speichern ist ein Fehler aufgetreten.'
							};
							modalStore.trigger(modalSettingPutError);
						} else {
							recipe.ingredients = recipe.ingredients.map((ingredient) =>
								ingredient.name === ingredientCreateDto.name
									? ingredientCreateDto.ingredient
									: ingredient
							);
						}
					}

					recipe.ingredients = recipe.ingredients;
				}
			}
		};
		modalStore.trigger(modal);
	}

	async function removeIngredient() {
		if (selectedIngredient == null) return;
		loadingIngredient = selectedIngredientIndex;
		const ingredientCreateDto: IngredientCreateDto = {
			recipeId: recipe.id,
			ingredient: selectedIngredient,
			name: selectedIngredient.name
		};

		const response: Response = await fetch('/recipes/ingredients/', {
			method: 'DELETE',
			body: JSON.stringify({ ingredientCreateDto })
		});
		loadingIngredient = -1;
		if (!response.ok) {
			const modalSettingDeleteError: ModalSettings = {
				type: 'alert',
				title: 'Fehler',
				body: 'Beim Löschen ist ein Fehler aufgetreten.'
			};
			modalStore.trigger(modalSettingDeleteError);
		} else {
			let index = recipe.ingredients.findIndex(
				(ingredient) => ingredient.name === selectedIngredient?.name
			);

			if (index !== -1) {
				recipe.ingredients.splice(index, 1);
				recipe.ingredients = recipe.ingredients; // Trigger reactivity
			}
		}
	}

	function showRatingModal() {
		const modal: ModalSettings = {
			title: 'Gericht bewerten',
			body: 'Bewerten Sie diese Rezept.',
			buttonTextCancel: 'Abbrechen',
			type: 'component',
			component: 'RatingModal',
			response: async (rating: number) => {
				if (rating && rating > 0) {
					const response: Response = await fetch('/recipes/' + recipe.id, {
						method: 'POST',
						body: JSON.stringify({ rating })
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

	function reset() {
		recipe = JSON.parse(backUpData);
		inEdidtMode = false;
	}

	$: console.log(selectedIngredient);
</script>

<div class="flex">
	<div class="flex-none">
		<a href="/recipes">
			<button type="button" class="btn-icon variant-filled-primary rounded-md"
				><i class="fa-solid fa-caret-left"></i>
			</button>
		</a>
	</div>
	<div class="grow">
		<h1 class="h1 ml-5 mr-5">{recipe.name}</h1>
	</div>
	<div class="flex-none">
		<button
			disabled={isSaving}
			class="btn-icon variant-filled-secondary rounded-md"
			use:popup={popupClick}
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
			<button id="cancel" class="btn variant-filled-surface" on:click={reset}
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

<div class="card p-4 max-w-sm" data-popup="ingredientPopupClick">
	<div class="grid grid-cols-1 gap-2">
		<button
			id="editIngredient"
			class="btn variant-filled-surface"
			on:click={() => {
				showIngredientEditModal(selectedIngredient);
			}}>Bearbeiten</button
		>
		<button
			id="deleteIngredient"
			class="btn variant-filled-error"
			on:click={() => {
				modalStore.trigger(modalIngredientDelete);
			}}>Löschen</button
		>
	</div>
	<div class="arrow bg-surface-100-800-token" />
</div>

<ItemTextLabel bind:value={recipe.name} label="Name" {inEdidtMode} />
<TextareaItemLabel bind:value={recipe.description} label="Beschreibung" {inEdidtMode} />
{#if inEdidtMode}
	<div class="grid grid-flow-col justify-stretch mt-5">
		<button
			disabled={isSaving}
			type="button"
			class="btn variant-filled-success flex self-stretch"
			on:click={async () => put()}
		>
			<div>
				<div class="flex flex-auto">
					<span><i class="fa-solid fa-floppy-disk"></i></span>
					<span class="ml-2">Speichern</span>
				</div>
				<div>
					{#if isSaving}
						<div class="flex justify-stretch">
							<ProgressBar height="h-1" />
						</div>
					{/if}
				</div>
			</div>
		</button>
	</div>
{/if}
<div>
	<hr
		class="m-5 h-0.5 border-t-0 bg-neutral-100 bg-gradient-to-r opacity-100 dark:opacity-30 to-transparent"
	/>
</div>

<div>
	<div class="w-full flex">
		<span class="flex-start">
			<h1 class="h2 mb-5">Zutaten</h1>
		</span>
		<span class="flex">
			<button
				type="button"
				class="btn-icon variant-filled justify-self-center mb-5 ml-5 mt-2"
				on:click={() => {
					showIngredientEditModal(null);
				}}><i class="fa-solid fa-plus"></i></button
			>
		</span>
	</div>

	<div class="w-full text-token space-y-2">
		<ol class="list">
			{#each recipe.ingredients as ingredient, i}
				<li
					transition:fade
					class="block p-6 bg-white border border-gray-200 rounded-lg shadow hover:bg-gray-100 dark:bg-gray-800 dark:border-gray-700 dark:hover:bg-gray-700 card-hover"
				>
					<div
						class="w-full grid grid-cols-4 {loadingIngredient == i
							? 'grid-rows-2'
							: 'grid-rows-1'}   gap-0"
					>
						<div>
							<span class="badge-icon p-4 variant-soft-primary"
								><h4 class="h4">{i + 1}</h4></span
							>
						</div>
						<div>
							<h3 class="h4">{ingredient.name}</h3>
						</div>
						<div>
							<h3 class="h4">{ingredient.countOff} {ingredient.unit}</h3>
						</div>
						<div class="justify-self-end">
							<button
								disabled={loadingIngredient == i}
								type="button"
								class="btn-icon variant-filled-secondary border-0"
								on:click={() => {
									selectedIngredient = ingredient;
									selectedIngredientIndex = i;
								}}
								use:popup={ingredientPopupClick}
								><i class="fa-solid fa-ellipsis-vertical"></i>
							</button>
						</div>
						{#if loadingIngredient == i || false}
							<div class="col-start-2 col-span-2 h-1">
								<ProgressBar height="h-1" />
							</div>
						{/if}
					</div>
				</li>
			{/each}
		</ol>
	</div>
</div>
