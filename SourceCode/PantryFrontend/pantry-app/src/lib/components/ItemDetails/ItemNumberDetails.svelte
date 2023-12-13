<script lang="ts">
	export let label: string;
	export let inEdidtMode: boolean;
	export let value: any | null;
	export let placeholder: string;
</script>

<label class="label">
	<span>{label}</span>
	<input
		class="input rounded-md p-2"
		type="number"
		{placeholder}
		bind:value
		readonly={!inEdidtMode}
	/>
</label>
