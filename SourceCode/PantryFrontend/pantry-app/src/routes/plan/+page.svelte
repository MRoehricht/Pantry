<script>
	// Add your script code here

	const weekDays = [
		'Montag',
		'Dienstag',
		'Mittwoch',
		'Donnersag',
		'Freitag',
		'Samstag',
		'Sonntag'
	];
</script>

<h1 class="h1">Plan</h1>

<div class="grid grid-cols-7 gap-4">
	{#each weekDays as weekDay}
		<div class="card card-hover">
			<div class="flex justify-center">
				<span>{weekDay}</span>
			</div>
			<hr class="!border-dotted" />
			<div class="flex justify-center">
				<span>Frühstück</span>
			</div>
		</div>
	{/each}
</div>

<style>
	/* Add your styles here */
</style>
