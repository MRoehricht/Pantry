import type { UnitOfMeasurement } from "$lib/modules/goods/types/Good";

export type Recipe = {
	id: string;
	name: string;
	description: string;
	ingredients: Ingredient[];
	details: RecipeDetails;
};

export type Ingredient = {
	name: string;
	countOff: number;
	unit: UnitOfMeasurement;
	pantryItemId: string | null;
};

export type RecipeDetails = {
	reviews: number[];
	cookedOn: string[];
	tags: string[];
};

export type RecipeOverview = {
	id: string;
	name: string;
	description: string;
	tags: string[];
	rating: number | null;
};

export type RecipeCreateDto = {
	name: string;
	description: string | null;
};

export type IngredientCreateDto = {
	name: string | null;
	recipeId: string;
	ingredient: Ingredient;
};
