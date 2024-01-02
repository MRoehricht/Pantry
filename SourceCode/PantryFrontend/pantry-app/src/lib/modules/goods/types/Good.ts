export type GoodDetails = {
	tags: string[];
	purchaseLocations: string[];
	ratings: number[];
	totalPurchaseNumber: number;
	daysUntilConsume: number | null;
};

export type Good = {
	id: string;
	name: string;
	description: string | null;
	amount: number;
	minimumAmount: number | null;
	storageLocation: string | null;
	ean: number | null;
	currentPrice: number | null;
	shoppinglistName: string | null;
	details: GoodDetails;
	unitOfMeasurement: UnitOfMeasurement;
	//priceHistories: any[];
};

export type GoodOverview = {
	id: string;
	name: string;
	description: string;
	tags: string[];
	rating: number | null;
};

export type Goods = {
	goods: Good[];
};

export type GoodCreateDto = {
	name: string;
};

export type GoodRatingCreateDto = {
	goodId: string;
	rating: number;
};

export interface GoodSuggestion {
	id: string;
	name: string;
	unitOfMeasurement: UnitOfMeasurement;
}

export enum UnitOfMeasurement {
	Milliliter = 0,
	Gram = 1,
	Piece = 2
}

export const UnitOfMeasurementDisplayName = {
	[UnitOfMeasurement.Milliliter]: 'Milliliter (ml)',
	[UnitOfMeasurement.Gram]: 'Gramm (g)',
	[UnitOfMeasurement.Piece]: 'Stück (Stk)'
};

export function getUnitOfMeasurementDisplayName(unitOfMeasurement: UnitOfMeasurement): string {
	return UnitOfMeasurementDisplayName[unitOfMeasurement];
}
