export type OverviewItem = {
	id: string;
	name: string;
	description: string;
	tags: string[];
	rating: number | null;
};

export type OverviewItemCreateDto = {
	name: string;
	description: string | null;
};
