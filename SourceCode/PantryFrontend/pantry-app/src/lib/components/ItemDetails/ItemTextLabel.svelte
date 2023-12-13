<script lang="ts">
	export let label: string;
	export let inEdidtMode: boolean;
	export let placeholder: string | null = null;
	export let value: any | null;

	$: if (placeholder === null) placeholder = label;
</script>

<label class="label">
	<span>{label}</span>
	<input
		class="input rounded-md p-2"
		type="text"
		{placeholder}
		bind:value
		readonly={!inEdidtMode}
	/>
</label>
