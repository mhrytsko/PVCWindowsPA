<script>
	export default {
		props: {
			errors: {
				type: Array,
				default: () => [],
			},
		},
	};
</script>

<template>
	<v-row density="compact" >
		<v-col>
			<v-alert
				v-for="(erro, index) in errors" :key="index"
				type="error"
				variant="outlined"
				closable
				density="compact"
				border="start"
				border-color="error"
				elevation="2">
				{{ erro }}
			</v-alert>
		</v-col>
	</v-row>
</template>
