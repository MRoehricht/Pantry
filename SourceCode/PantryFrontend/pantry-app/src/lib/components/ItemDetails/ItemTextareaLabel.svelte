<script lang="ts">
	import { onMount } from 'svelte';

	export let label: string;
	export let inEdidtMode: boolean;
	export let value: any | null;
	let textarea: HTMLTextAreaElement;

	function autoGrow(element: HTMLTextAreaElement): void {
		element.style.height = 'auto';
		element.style.height = element.scrollHeight + 'px';
	}

	onMount(() => {
		autoGrow(textarea);
	});
</script>

<label class="label">
	<span>{label}</span>
	<textarea
		bind:this={textarea}
		style="overflow: auto; resize: none; min-height: 50px; "
		class="textarea p-2"
		rows="5"
		placeholder={label}
		bind:value
		contenteditable
		on:input={(e) => {
			// @ts-ignore
			autoGrow(e.target);
		}}
		readonly={!inEdidtMode}
	/>
</label>
