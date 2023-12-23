<script lang="ts">
	import '../app.postcss';
	import {
		AppShell,
		AppBar,
		initializeStores,
		Drawer,
		getDrawerStore,
		LightSwitch,
		Modal,
		Toast,
		type ModalComponent,
		storePopup
	} from '@skeletonlabs/skeleton';
	import Navigation from '$lib/components/Navigation/Navigation.svelte';
	import { page } from '$app/stores';

	initializeStores();
	const drawerStore = getDrawerStore();
	function drawerOpen(): void {
		drawerStore.open({});
	}

	import RatingModal from '$lib/components/Modals/RatingModal.svelte';
	import IngredientEditModal from '$lib/components/Modals/IngredientEditModal.svelte';
	import FindRecipeModal from '$lib/components/Modals/FindRecipeModal.svelte';
	import { autoUpdate, computePosition, offset, shift, flip, arrow } from '@floating-ui/dom';
	import { signOut } from '@auth/sveltekit/client';

	const modalRegistry: Record<string, ModalComponent> = {
		RatingModal: { ref: RatingModal },
		IngredientEditModal: { ref: IngredientEditModal },
		FindRecipeModal: { ref: FindRecipeModal }
	};
	storePopup.set({ computePosition, autoUpdate, offset, shift, flip, arrow });
</script>

<Toast />
<Modal components={modalRegistry} />
{#if $page.data.session}
	<Drawer>
		<h2 class="p-4">Navigation</h2>
		<hr />
		<Navigation />
	</Drawer>
{/if}

<!-- App Shell -->
<AppShell slotSidebarLeft="bg-surface-500/5 w-0 lg:w-20">
	<svelte:fragment slot="header">
		<!-- App Bar -->
		<AppBar>
			<svelte:fragment slot="lead">
				<div class="flex items-center">
					<button class="lg:hidden btn btn-sm mr-4" on:click={drawerOpen}>
						<span>
							<svg viewBox="0 0 100 80" class="fill-token w-4 h-4">
								<rect width="100" height="20" />
								<rect y="30" width="100" height="20" />
								<rect y="60" width="100" height="20" />
							</svg>
						</span>
					</button>
					<a href="/"> <strong class="text-xl uppercase">PANTRY</strong></a>
				</div>
			</svelte:fragment>

			<svelte:fragment slot="trail">
				<a
					class="btn btn-sm variant-ghost-surface"
					href="https://github.com/MRoehricht/Pantry"
					target="_blank"
					rel="noreferrer"
				>
					GitHub
				</a>
				<LightSwitch />
				{#if $page.data.session}
					<button on:click={() => signOut()} class="btn btn-sm variant-ghost-surface">
						Abmelden
					</button>
				{/if}
			</svelte:fragment>
		</AppBar>
	</svelte:fragment>
	<svelte:fragment slot="sidebarLeft">
		<Navigation />
	</svelte:fragment>
	<!-- Page Route Content -->
	<div class="m-5">
		<slot />
	</div>
</AppShell>
