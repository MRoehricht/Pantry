<script lang="ts">
	import type { Good } from '../types/Good';

	export let good: Good;
</script>

<a
	href="/goods/{good.id}"
	class="block max-w-sm p-6 bg-white border border-gray-200 rounded-lg shadow hover:bg-gray-100 dark:bg-gray-800 dark:border-gray-700 dark:hover:bg-gray-700"
>
	<h5 class="mb-2 text-2xl font-bold tracking-tight dark:text-white">
		{good.name}
	</h5>
	<p class="font-normal text-gray-700 dark:text-gray-400">
		{good.description}
	</p>
</a>
