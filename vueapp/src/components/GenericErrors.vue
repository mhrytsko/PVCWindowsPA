<script>
	import {isEmpty, isObject} from 'lodash-es';

	export default {
		data() {
			return {
				snackbar: false,
				text: '',
				timeout: 3000,
			};
		},

		methods: {
			showMessage(event) {
				if (isObject(event) && !isEmpty(event?.message)) {
					this.text = event?.message || '';
					this.snackbar = true;
				}
			},

			close() {
				this.snackbar = false;
				this.text = '';
			},
		},

		mounted() {
			this.$eventBus.on('show-generic-error', this.showMessage);
		},

		beforeUnmount() {
			this.$eventBus.off('show-generic-error', this.showMessage);
		},
	};
</script>

<template>
	<v-snackbar v-model="snackbar" :timeout="timeout" multi-line>
    <span v-html="text"></span>
		<template v-slot:actions>
			<v-btn color="red" variant="text" @click="close"> Fechar </v-btn>
		</template>
	</v-snackbar>
</template>
