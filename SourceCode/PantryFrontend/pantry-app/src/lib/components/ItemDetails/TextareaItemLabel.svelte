<script lang="ts">
	import ItemTextareaLabel from './ItemTextareaLabel.svelte';

	export let label: string;
	export let inEdidtMode: boolean;
	export let value: string | null;
</script>

{#if !inEdidtMode && value != null && value.length > 0}
	<ItemTextareaLabel {label} {inEdidtMode} bind:value />
{:else if inEdidtMode}
	<ItemTextareaLabel {label} {inEdidtMode} bind:value />
{/if}
