<script>
	import {mapState} from 'pinia';
	import {useBrandsDataStore} from '@/store/brands.js';

	import {get} from 'lodash-es';

	export default {
		emits: ['update:modelValue', 'update:show'],

		props: {
			modelValue: {
				required: true,
			},

			show: {
				type: Boolean,
				required: true,
				default: false,
			},
		},

		methods: {
			onSelect(value) {
				const selectedValue = get(value, '0', this.modelValue);
				this.$emit('update:modelValue', selectedValue);
			},

			onClose() {
				this.$emit('update:show', false);
			},
		},

		computed: {
			...mapState(useBrandsDataStore, ['brands']),
		},
	};
</script>

<template>
	<v-dialog
		:modelValue="show"
		@update:modelValue="onClose"
		fullscreen
		:scrim="false"
		scrollable
		transition="dialog-bottom-transition">
		<v-card>
			<v-toolbar>
				<v-btn icon dark @click="onClose">
					<v-icon>mdi-close</v-icon>
				</v-btn>
			</v-toolbar>
			<v-card-title>
				<span class="text-h5">Marcas</span>
			</v-card-title>
			<v-card-text>
				<v-list color="primary" :selected="[modelValue]" @update:selected="onSelect">
					<v-list-item v-for="brand in brands" :key="brand.id" :value="brand.id" elevation="2" class="ma-1">
						<v-row>
							<v-col>
								<v-img :src="brand.image?.fileSrc" alt="Logótipo" width="64" height="64" />
							</v-col>
							<v-col> </v-col>
						</v-row>
						<v-row>
							<v-col>
								<v-expansion-panels @click="(e) => e.stopPropagation()">
									<v-expansion-panel
										class="text-with-line-breaks"
										title="Descrição"
										:text="brand.description" />
								</v-expansion-panels>
							</v-col>
						</v-row>
					</v-list-item>
				</v-list>
			</v-card-text>
		</v-card>
	</v-dialog>
</template>
