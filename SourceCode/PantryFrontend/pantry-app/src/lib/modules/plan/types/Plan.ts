import type { RecipeOverview } from "$lib/modules/recipe/types/Recipe";

export interface Meal {
	id: string;
	recipeId: string;
	date: string;
	wasCooked: boolean;
}

export type CreateMeal = {
	recipeId: string;
	date: string;
};

export type WeekDay = {
	name: string;
	date: string;
	meals: RecipeOverview[];
};
