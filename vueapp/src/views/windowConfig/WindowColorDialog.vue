<script>
	import {mapState} from 'pinia';
	import {useWindowColorsDataStore} from '@/store/windowColors.js';

	import {get, filter, isEmpty} from 'lodash-es';

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

			profileBrandId: {
				type: String,
				default: null,
			},
		},

		methods: {
            isEmpty,

			onSelect(value) {
				const selectedValue = get(value, '0', this.modelValue);
				this.$emit('update:modelValue', selectedValue);
			},

			onClose() {
				this.$emit('update:show', false);
			},
		},

		computed: {
			...mapState(useWindowColorsDataStore, ['windowColors']),

			brandWindowColors() {
				return filter(this.windowColors, {brandId: this.profileBrandId});
			},
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
				<span class="text-h5">Perfil</span>
			</v-card-title>
			<v-card-text>
				<template v-if="isEmpty(brandWindowColors)">
					<v-sheet>
						<v-alert
							density="compact"
							type="warning"
							title="Cor do perfil"
							text="Primeiro, selecione uma marca de perfil para visualizar a lista." />
					</v-sheet>
				</template>
				<v-list color="primary" :selected="[modelValue]" @update:selected="onSelect">
					<v-list-item
						v-for="color in brandWindowColors"
						:key="color.id"
						:value="color.id"
						elevation="3"
						class="ma-1">
						<v-row>
							<v-col>
								<span class="text-h6">
									{{ color.name }}
								</span>
							</v-col>
						</v-row>
						<v-row>
							<v-col cols="auto">
								<v-sheet
									v-if="color.colorType === 0"
									:color="color.hexCode"
									height="250"
									width="250"
									elevation="8"
									class="ma-1" />
								<v-img
									v-else-if="color.colorType === 1"
									:src="color.image?.fileSrc"
									alt="Cor"
									height="250"
									width="250"
									elevation="8"
									class="ma-1" />
								<v-sheet v-else height="250" width="250"> ? </v-sheet>
							</v-col>
						</v-row>
					</v-list-item>
				</v-list>
			</v-card-text>
		</v-card>
	</v-dialog>
</template>
