<script lang="ts">
	export let data;
</script>

<div class="flex items-center">
	<a href="/goods"
		><button type="button" class="btn-icon variant-filled"
			><i class="fa-solid fa-caret-left"></i></button
		>
	</a>
	<h1 class="h1 m-5">{data.good.name}</h1>
</div>
