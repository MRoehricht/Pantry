<script lang="ts">
	import ItemNumberDetails from './ItemNumberDetails.svelte';

	export let label: string;
	export let inEdidtMode: boolean;
	export let value: number | null;
	export let placeholder: string;
</script>

{#if !inEdidtMode && value != null && value > 0}
	<ItemNumberDetails {label} {inEdidtMode} bind:value {placeholder} />
{:else if inEdidtMode}
	<ItemNumberDetails {label} {inEdidtMode} bind:value {placeholder} />
{/if}
