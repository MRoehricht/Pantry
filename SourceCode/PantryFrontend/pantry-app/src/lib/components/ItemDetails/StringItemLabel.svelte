<script lang="ts">
	import ItemTextLabel from './ItemTextLabel.svelte';

	export let label: string;
	export let inEdidtMode: boolean;
	export let value: string | null;
	export let placeholder: string | null = null;

	$: if (placeholder === null) placeholder = label;
</script>

{#if !inEdidtMode && value != null && value.length > 0}
	<ItemTextLabel {label} {inEdidtMode} bind:value {placeholder} />
{:else if inEdidtMode}
	<ItemTextLabel {label} {inEdidtMode} bind:value {placeholder} />
{/if}
