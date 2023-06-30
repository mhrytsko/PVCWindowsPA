<template>
	<default-layout/>
</template>

<script>
	//import { useConfigStore } from '@/store/config.js'
	import {mapActions} from 'pinia';

	import {useBrandsDataStore} from '@/store/brands.js';
	import {useWindowProfilesDataStore} from '@/store/windowProfile.js';
	import {useWindowColorsDataStore} from '@/store/windowColors.js';

	import DefaultLayout from '@/layouts/default/Default.vue'

	import navigation from '@/mixins/navigation.js';

	export default {
		name: 'App',
		components: {
			DefaultLayout
		},
		mixins: [navigation],

		data() {
			return {
				
			}
		},
		methods:
		{
			...mapActions(useBrandsDataStore, {
				fetchBrandsData: 'fetchData'
			}),

			...mapActions(useWindowProfilesDataStore, {
				fetchWindowProfilesData: 'fetchData'
			}),

			...mapActions(useWindowColorsDataStore, {
				fetchWindowColorsData: 'fetchData'
			}),

			redirectToRoute(routeObj)
			{
				this.handleRoutePush(routeObj)
			}
		},

		mounted() {
			this.$eventBus.on('redirect-to-route', this.redirectToRoute);

			this.fetchBrandsData()
			this.fetchWindowProfilesData()
			this.fetchWindowColorsData()
		},

		beforeUnmount() {
			this.$eventBus.off('redirect-to-route', this.redirectToRoute);
		},
	};
</script>

<style>

</style>
