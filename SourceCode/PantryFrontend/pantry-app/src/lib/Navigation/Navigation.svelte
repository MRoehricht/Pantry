<script lang="ts">
	import { AppRail, AppRailAnchor, AppRailTile, getDrawerStore } from '@skeletonlabs/skeleton';
	import { page } from '$app/stores';

	const drawerStore = getDrawerStore();

	function drawerClose(): void {
		drawerStore.close();
	}
</script>

<AppRail>
	<!--
    <svelte:fragment slot="lead">
        <AppRailAnchor href="/"><i class="fa-solid fa-bars"></i></AppRailAnchor>
    </svelte:fragment>
    -->
	<!-- --- -->
	<AppRailAnchor
		name="goods-tile"
		title="goods-tile"
		href="/goods"
		selected={$page.url.pathname === '/goods'}
		on:click={drawerClose}
	>
		<svelte:fragment slot="lead"><i class="fa-solid fa-shop fa-xl p-2"></i></svelte:fragment>
		<span>Waren</span>
	</AppRailAnchor>
	<AppRailAnchor
		name="plan-tile"
		title="plan-tile"
		href="/plan"
		selected={$page.url.pathname === '/plan'}
		on:click={drawerClose}
	>
		<svelte:fragment slot="lead"
			><i class="fa-solid fa-calendar-days fa-xl p-2"></i>
		</svelte:fragment>
		<span>Plan</span>
	</AppRailAnchor>
	<AppRailAnchor
		name="recipes-tile"
		title="recipes-tile"
		href="/recipes"
		selected={$page.url.pathname === '/recipes'}
		on:click={drawerClose}
	>
		<svelte:fragment slot="lead">
			<i class="fa-solid fa-bowl-food fa-xl p-2"></i>
		</svelte:fragment>
		<span>Rezepte</span>
	</AppRailAnchor>
	<AppRailAnchor
		name="scanner-tile"
		title="scanner-tile"
		href="/scanner"
		selected={$page.url.pathname === '/scanner'}
		on:click={drawerClose}
	>
		<svelte:fragment slot="lead">
			<i class="fa-solid fa-barcode fa-xl p-2"></i>
		</svelte:fragment>
		<span>Scanner</span>
	</AppRailAnchor>
	<!-- --- -->
	<svelte:fragment slot="trail">
		<AppRailAnchor href="/goods" title="Account" on:click={drawerClose}
			><i class="fa-solid fa-user fa-xl"></i>
		</AppRailAnchor>
	</svelte:fragment>
</AppRail>
