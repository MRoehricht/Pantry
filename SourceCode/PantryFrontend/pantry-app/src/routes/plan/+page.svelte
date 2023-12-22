<script lang="ts">
	import type {
		CreateMeal,
		Meal,
		MealRecipeOverview,
		WeekDay
	} from '$lib/modules/plan/types/Plan';

	// Add your script code here
	import type { RecipeOverview } from '$lib/modules/recipe/types/Recipe';
	import { getModalStore, type ModalSettings } from '@skeletonlabs/skeleton';

	const modalStore = getModalStore();

	let weekDays: WeekDay[] = [];

	function getWeekNumber(): [number, number] {
		const d = new Date();
		d.setUTCDate(d.getUTCDate() + 4 - (d.getUTCDay() || 7));
		const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
		const weekNo = Math.ceil(((d.getTime() - yearStart.getTime()) / 86400000 + 1) / 7);
		return [d.getUTCFullYear(), weekNo];
	}

	// Fetch data whenever 'week' changes
	$: loadData(), week;

	function getMondayOfWeek(year: number, week: number): Date {
		const firstDayOfYear = new Date(year, 0, 1);
		const daysOffset =
			firstDayOfYear.getDay() > 4 ? 7 - firstDayOfYear.getDay() : -firstDayOfYear.getDay();
		const firstMondayOfYear = new Date(year, 0, 1 + daysOffset);
		const targetDate = new Date(
			firstMondayOfYear.getTime() + (week - 1) * 7 * 24 * 60 * 60 * 1000
		);
		targetDate.setDate(targetDate.getDate() + 1);
		return targetDate;
	}

	function getDaysOfWeek(year: number, week: number): WeekDay[] {
		const weekStart = getMondayOfWeek(year, week);

		const daysOfWeek = Array.from({ length: 7 }, (v, i) => {
			const date = new Date(weekStart.valueOf() + i * 24 * 60 * 60 * 1000);
			const name = getWeekDayName(date.getDay());

			return { name, date: formatDate(date), meals: [] } as WeekDay;
		});
		return daysOfWeek;
	}

	function formatDate(date: Date): string {
		const offset = date.getTimezoneOffset();
		const dateWithOffset = new Date(date.getTime() - offset * 60 * 1000);
		return dateWithOffset.toISOString().split('T')[0];
	}

	async function loadData() {
		weekDays.length = 0;
		weekDays = getDaysOfWeek(year, week);
		console.log(weekDays);

		const weekDayPromises = weekDays.map(async (weekDay) => {
			const date = weekDay.date;
			const response: Response = await fetch('/plan/date/' + date, {
				method: 'GET'
			});
			if (response.ok) {
				const meals: Meal[] = await response.json();
				const mealPromises = meals.map(async (meal) => {
					const responseRecipe: Response = await fetch('/recipes/' + meal.recipeId, {
						method: 'GET'
					});
					if (responseRecipe.ok) {
						const overviewItem: RecipeOverview = await responseRecipe.json();
						const mealRecipeOverview: MealRecipeOverview = {
							meal: meal,
							recipeOverview: overviewItem
						};

						weekDay.meals.push(mealRecipeOverview);
					}
				});
				await Promise.all(mealPromises);
			}
		});

		await Promise.all(weekDayPromises);
		weekDays = [...weekDays];
	}

	function getWeekDayName(day: number): string {
		const weekDays = [
			'Sonntag',
			'Montag',
			'Dienstag',
			'Mittwoch',
			'Donnerstag',
			'Freitag',
			'Samstag'
		];
		return weekDays[day];
	}

	const result = getWeekNumber();
	let week = result[1];
	let year = result[0];

	function showFindRecipeModal(date: string) {
		const modal: ModalSettings = {
			title: 'Gericht hinzufügen',
			buttonTextCancel: 'Abbrechen',
			type: 'component',
			component: 'FindRecipeModal',
			response: async (recipeResponse: RecipeOverview) => {
				if (recipeResponse.id) {
					let createMealRequest: CreateMeal = {
						date: date,
						recipeId: recipeResponse.id
					};
					const response: Response = await fetch('/plan/', {
						method: 'POST',
						body: JSON.stringify({ createMealRequest })
					});
					if (!response.ok) {
						const modalSettingPutError: ModalSettings = {
							type: 'alert',
							title: 'Fehler',
							body: 'Beim Speichern ist ein Fehler aufgetreten.'
						};
						modalStore.trigger(modalSettingPutError);
					} else {
						let weekDay = weekDays.find((weekDay) => weekDay.date == date);
						if (weekDay == null) return;
						const mealRecipeOverview: MealRecipeOverview = {
							meal: await response.json(),
							recipeOverview: recipeResponse
						};
						weekDay.meals.push(mealRecipeOverview);
						weekDays = [...weekDays];
					}
				}
			}
		};
		modalStore.trigger(modal);
	}

	function gotoNextWeek() {
		week++;
		if (week > 52) {
			week = 1;
			year++;
		}
	}

	function gotoLastWeek() {
		week--;
		if (week < 1) {
			week = 52;
			year--;
		}
	}

	function setWarsCooked(meal: Meal) {
		const modal: ModalSettings = {
			title: 'Gericht als gekocht markieren',
			buttonTextCancel: 'Abbrechen',
			buttonTextConfirm: 'Ja',
			type: 'confirm',
			component: 'ConfirmModal',
			response: async (confirmed: boolean) => {
				if (confirmed) {
					meal.wasCooked = true;
					const response: Response = await fetch('/plan/' + meal.id, {
						method: 'PUT',
						body: JSON.stringify({ meal })
					});
					if (!response.ok) {
						const modalSettingPutError: ModalSettings = {
							type: 'alert',
							title: 'Fehler',
							body: 'Beim Speichern ist ein Fehler aufgetreten.'
						};
						modalStore.trigger(modalSettingPutError);
					} else {
						loadData();
					}
				}
			}
		};
		modalStore.trigger(modal);
	}
</script>

<div class="flex mb-5">
	<div class="grow">
		<h1 class="h1 ml-5 mr-5">Plan</h1>
	</div>
	<div><h2 class="h2 ml-5 mr-5">KW {week}</h2></div>
	<div><h2 class="h2 ml-5 mr-5">{year}</h2></div>
	<div class="flex-none">
		<button
			type="button"
			class="btn-icon variant-filled-primary rounded-md"
			on:click={gotoLastWeek}
			><i class="fa-solid fa-caret-left"></i>
		</button>
		<button
			type="button"
			class="btn-icon variant-filled-primary rounded-md"
			on:click={gotoNextWeek}
			><i class="fa-solid fa-caret-right"></i>
		</button>
	</div>
</div>

<div class="grid grid-cols-7 gap-4">
	{#each weekDays as weekDay}
		<div class="card card-hover">
			<div class="flex justify-center">
				<span>{weekDay.name}</span>
			</div>
			<div class="flex justify-items-stretch">
				<button
					class="flex-1 btn variant-ghost-primary h-16 rounded-md"
					on:click={() => {
						showFindRecipeModal(weekDay.date);
					}}>+</button
				>
			</div>
			{#each weekDay.meals as meal}
				<div class="flex justify-center mb-2">
					<span class="grow ml-5 {meal.meal.wasCooked ? 'strikethrough' : ''} "
						>{meal.recipeOverview.name}</span
					>
					{#if !meal.meal.wasCooked}
						<button
							class="flex-end mr-5"
							title="Als gekocht markieren"
							on:click={() => {
								setWarsCooked(meal.meal);
							}}><i class="fa-regular fa-circle-check"></i></button
						>
					{/if}
				</div>
			{/each}
		</div>
	{/each}
</div>

<style>
	.strikethrough {
		text-decoration: line-through;
	}
	.hover-div button {
		visibility: hidden;
	}

	.hover-div:hover button {
		visibility: visible;
	}
</style>
