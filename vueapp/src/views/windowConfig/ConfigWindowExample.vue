<script>
	import {mapState} from 'pinia';
	import {useBrandsDataStore} from '@/store/brands.js';
	import {useWindowProfilesDataStore} from '@/store/windowProfile.js';

	import _ from 'lodash-es';

	import WindowModel from '@/models/window.js';
	import LeafConfiguration from '@/models/leafConfiguration.js'

	export default {
		name: 'ConfigWindowExample', // WindowSimulation

		data() {
			return {
				profileBrandId: null,
				profileId: null,

				model: new WindowModel({
					width: 500,
					height: 250,
					leafConfigurations: [ 
						new LeafConfiguration({
							x: 0,
							y: 0,
							width: 250,
							height: 250,
							openingSystem: 0//'fixed'
						}),
						new LeafConfiguration({
							x: 0,
							y: 1,
							width: 250,
							height: 250,
							openingSystem: 3//'oscilobatente',
						})
				]
				}),

				windowConfig: {
					windowWidth: 320,
					windowHeight: 390,
					profileWidth: 10,
					panes: [
						{x: 0, y: 0, width: 160, height: 250, type: 'oscilobatente', direction: 'left'},
						{x: 0, y: 1, width: 160, height: 250, type: 'oscilobatente', direction: 'right'},
						{x: 1, y: 0, width: 320, height: 140, type: 'fixed', direction: null},
					],
				},

				windowConfig2: {
					windowWidth: 320,
					windowHeight: 390,
					profileWidth: 20,
					panes: [
						{x: 0, y: 0, width: 160, height: 250, type: 'normal', direction: 'left'},
						{x: 0, y: 1, width: 160, height: 250, type: 'oscilobatente', direction: 'right'},
						{x: 1, y: 0, width: 320, height: 140, type: 'fixed', direction: null},
					],
				},

				windowConfig3: {
					windowWidth: 320,
					windowHeight: 430,
					profileWidth: 10,
					panes: [
						{x: 0, y: 0, width: 320, height: 90, type: 'fixed', direction: null},
						{x: 1, y: 0, width: 160, height: 250, type: 'normal', direction: 'left'},
						{x: 1, y: 1, width: 160, height: 250, type: 'oscilobatente', direction: 'right'},
						{x: 2, y: 0, width: 320, height: 90, type: 'fixed', direction: null},
					],
				},
			};
		},

		computed: {
			...mapState(useBrandsDataStore, ['brands']),
			...mapState(useWindowProfilesDataStore, ['windowProfiles']),

			brandWindowProfiles() {
				return _.filter(this.windowProfiles, {brandId: this.profileBrandId});
			},
		},

		mounted() {},

		beforeUnmount() {},

		methods: {},

		watch: {
			profileBrandId(newValue) {
				if (!_.some(this.brandWindowProfiles, {brandId: newValue})) this.profileId = null;
			},
		},
	};
</script>

<template>
	<div>
		<v-container>
			<v-row>
				<v-col cols="12" sm="6" md="3" lg="2">
					<v-select
						v-model="profileBrandId"
						:items="brands"
						item-value="id"
						item-title="name"
						label="Marca do perfil"
						required></v-select>
				</v-col>
				<v-col cols="12" sm="6" md="3" lg="3">
					<v-select
						v-model="profileId"
						:items="brandWindowProfiles"
						item-value="id"
						item-title="name"
						label="Perfil"
						required></v-select>
				</v-col>
			</v-row>

			<v-row>
				<v-col cols="12" sm="6" md="3" lg="2">
					<v-text-field v-model.number="model.width" label="Largura" type="number" suffix="mm" />
				</v-col>
				<v-col cols="12" sm="6" md="3" lg="2">
					<v-text-field v-model.number="model.height" label="Altura" type="number" suffix="mm" />
				</v-col>
			</v-row>

			<v-row>
				<v-col>
					<m-simple-2d-window
						:window-width="model.width"
						:window-height="model.height"
						:profile-width="30"
						:panes="model.leafConfigurations" />
				</v-col>
				<v-col>
					<m-simple-2d-window
						:window-width="model.width"
						:window-height="model.height"
						:profile-width="30"
						:panes="model.leafConfigurations"
						:canvas-width="500" :canvas-height="500"/>
				</v-col>
			</v-row>

			<br /><br /><br /><br /><br />
			<v-divider />
			<br />
			<v-row>
				<v-col>
					<m-simple-2d-window v-bind="windowConfig" />
				</v-col>
				<v-col>
					<m-simple-2d-window v-bind="windowConfig" :canvas-width="500" :canvas-height="500"/>
				</v-col>
			</v-row>
			<v-row>
				<v-col>
					<m-simple-2d-window v-bind="windowConfig2" />
				</v-col>
				<v-col>
					<m-simple-2d-window v-bind="windowConfig2" :canvas-width="500" :canvas-height="500"/>
				</v-col>
			</v-row>
			<v-row>
				<v-col>
					<m-simple-2d-window v-bind="windowConfig3" />
				</v-col>
				<v-col>
					<m-simple-2d-window v-bind="windowConfig3" :canvas-width="500" :canvas-height="500"/>
				</v-col>
			</v-row>
		</v-container>
	</div>
</template>

<style></style>
