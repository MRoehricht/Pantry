export type Recipe = {
	id: string;
	name: string;
	description: string;
	ingredients: Ingredient[];
	details: Details;
};

export type Ingredient = {
	name: string;
	countOff: number;
	unit: string;
	pantryItemId: string;
};

export type Details = {
	reviews: number[];
	cookedOn: string[];
	tags: string[];
};
