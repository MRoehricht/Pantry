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
	minimumAmount: number;
	storageLocation: string | null;
	ean: number | null;
	currentPrice: number;
	shoppinglistName: string | null;
	details: GoodDetails;
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
