export type GoodDetails = {
	tags: string[];
	purchaseLocations: string[];
	ratings: number[];
	totalPurchaseNumber: number;
	daysUntilConsume: number;
};

export type Good = {
	id: string;
	name: string;
	description: string;
	amount: number;
	minimumAmount: number;
	storageLocation: string;
	ean: number;
	currentPrice: number;
	shoppinglistName: string;
	details: GoodDetails;
	priceHistories: any[];
};

export type Goods = {
	goods: Good[];
};
