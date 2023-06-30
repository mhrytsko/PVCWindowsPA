<script>
	import {mapState} from 'pinia';
	import {useWindowColorsDataStore} from '@/store/windowColors.js';
	import {ColorType} from '@/mixins/systemEnums.js';

	import {find} from 'lodash-es'

	export default {
		emits: ['update:show'],

		props: {
            modelValue: {
                type: Object,
                required: true
            },

			show: {
				type: Boolean,
				required: true,
				default: false,
			},

			// 3D & AR
			useARCore: {
				type: Boolean,
				default: false,
			},

			tryWall: {
				type: Boolean,
				default: true,
			},

			showSkybox: {
				type: Boolean,
				default: true,
			},
		},

		computed:
		{
			...mapState(useWindowColorsDataStore, ['windowColors']),
			indorColor() {
				return find(this.windowColors, {id: this.modelValue?.indorColorId});
			},

			indorColorId()
			{
				return this.indorColor?.id ?? undefined
			},

			indorColorType()
			{
				let colorType = this.indorColor?.colorType;
				return colorType !== undefined ? colorType : ColorType.Solid.value
			}
		},

		methods: {
			onClose() {
				this.$emit('update:show', false);
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
		
		transition="dialog-bottom-transition">
		<v-card>
			<v-toolbar>
				<v-btn icon dark @click="onClose">
					<v-icon>mdi-close</v-icon>
				</v-btn>
			</v-toolbar>
			<v-card-title>
				<span class="text-h5">3D & Realidade Aumentada (AR)</span>
			</v-card-title>
			<v-card-text>
                <m-simple-3d-window
                    v-if="show && modelValue"
                    :window-width="modelValue.width"
                    :window-height="modelValue.height"
                    :panes="modelValue.leafConfigurations"
					:indorColorId="indorColorId"
					:indorColorType="indorColorType"
					:useARCore="useARCore"
					:tryWall="tryWall"
					:showSkybox="showSkybox" />
			</v-card-text>
		</v-card>
	</v-dialog>
</template>
