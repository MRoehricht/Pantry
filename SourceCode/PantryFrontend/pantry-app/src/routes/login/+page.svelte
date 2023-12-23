<script>
	import { signIn, signOut } from '@auth/sveltekit/client';
	import { page } from '$app/stores';
</script>

<div style="display: flex; justify-content: center; align-items: center;">
	<h1>SvelteKit Auth Example</h1>
	<p>
		{#if $page.data.session}
			{#if $page.data.session.user?.image}
				<span
					style="background-image: url('{$page.data.session.user.image}')"
					class="avatar"
				/>
			{/if}
			<span class="signedInText">
				<small>Signed in as</small><br />
				<strong>{$page.data.session.user?.name ?? 'User'}</strong>
			</span>
			<button on:click={() => signOut()} class="button">Sign out</button>
		{:else}
			<span class="notSignedInText">You are not signed in</span>

			<button type="button" on:click={() => signIn('github')} class="btn variant-filled">
				<span><i class="fa-brands fa-github"></i></span>
				<span>Sign In with GitHub</span>
			</button>
		{/if}
	</p>
</div>
