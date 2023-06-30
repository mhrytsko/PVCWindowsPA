<script>
	import WindowModel from '@/models/window.js';
	import LeafConfiguration from '@/models/leafConfiguration.js';

	import {get, isEmpty, cloneDeep} from 'lodash-es';

	import {v4 as uuidv4} from 'uuid';

	import {WindowOpeningType, WindowOpeningDirection} from '@/mixins/systemEnums.js';

	const windowId = uuidv4();

	const defaultModels = {
		fixo: new WindowModel({
						id: windowId,
						width: 800,
						height: 1400,
						leafConfigurations: [
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 0,
								width: 800,
								height: 1400,
								openingSystem: WindowOpeningType.Fixed.value,
								openingDirection: WindowOpeningDirection.None.value,
							})
						],
					}),

		oscilobatente_1_folha: new WindowModel({
						id: windowId,
						width: 800,
						height: 1400,
						leafConfigurations: [
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 0,
								width: 800,
								height: 1400,
								openingSystem: WindowOpeningType.TiltAndTurn.value,
								openingDirection: WindowOpeningDirection.LeftRight.value,
							})
						],
					}),

		oscilobatente_2_folhas: new WindowModel({
						id: windowId,
						width: 1200,
						height: 1400,
						leafConfigurations: [
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 0,
								width: 600,
								height: 1400,
								openingSystem: WindowOpeningType.SideHung.value,
								openingDirection: WindowOpeningDirection.RightLeft.value,
							}),
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 1,
								width: 600,
								height: 1400,
								openingSystem: WindowOpeningType.TiltAndTurn.value,
								openingDirection: WindowOpeningDirection.LeftRight.value,
							}),
						],
					}),

		oscilobatente_1_folha_com_bandeira: new WindowModel({
						id: windowId,
						width: 800,
						height: 1850,
						leafConfigurations: [
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 0,
								width: 800,
								height: 450,
								openingSystem: WindowOpeningType.Fixed.value,
								openingDirection: WindowOpeningDirection.None.value,
							}),
							new LeafConfiguration({
								windowId: windowId,
								x: 1,
								y: 0,
								width: 800,
								height: 1400,
								openingSystem: WindowOpeningType.TiltAndTurn.value,
								openingDirection: WindowOpeningDirection.RightLeft.value,
							})
						],
					}),

		oscilobatente_2_folhas_com_bandeira: new WindowModel({
						id: windowId,
						width: 1200,
						height: 1850,
						leafConfigurations: [
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 0,
								width: 1200,
								height: 450,
								openingSystem: WindowOpeningType.Fixed.value,
								openingDirection: WindowOpeningDirection.None.value,
							}),
							new LeafConfiguration({
								windowId: windowId,
								x: 1,
								y: 0,
								width: 600,
								height: 1400,
								openingSystem: WindowOpeningType.SideHung.value,
								openingDirection: WindowOpeningDirection.RightLeft.value,
							}),
							new LeafConfiguration({
								windowId: windowId,
								x: 1,
								y: 1,
								width: 600,
								height: 1400,
								openingSystem: WindowOpeningType.TiltAndTurn.value,
								openingDirection: WindowOpeningDirection.LeftRight.value,
							})
						],
					}),

		alta_oscilobatente_1_folha: new WindowModel({
						id: windowId,
						width: 600,
						height: 2100,
						leafConfigurations: [
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 0,
								width: 600,
								height: 2100,
								openingSystem: WindowOpeningType.TiltAndTurn.value,
								openingDirection: WindowOpeningDirection.RightLeft.value,
							})
						],
					}),

		alta_oscilobatente_2_folhas: new WindowModel({
						id: windowId,
						width: 1200,
						height: 2100,
						leafConfigurations: [
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 0,
								width: 600,
								height: 2100,
								openingSystem: WindowOpeningType.SideHung.value,
								openingDirection: WindowOpeningDirection.RightLeft.value,
							}),
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 1,
								width: 600,
								height: 2100,
								openingSystem: WindowOpeningType.TiltAndTurn.value,
								openingDirection: WindowOpeningDirection.LeftRight.value,
							})
						],
					}),

		oscilobatente_3_folhas: new WindowModel({
						id: windowId,
						width: 2050,
						height: 1400,
						leafConfigurations: [
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 0,
								width: 680,
								height: 1400,
								openingSystem: WindowOpeningType.SideHung.value,
								openingDirection: WindowOpeningDirection.RightLeft.value,
							}),
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 1,
								width: 685,
								height: 1400,
								openingSystem: WindowOpeningType.TiltAndTurn.value,
								openingDirection: WindowOpeningDirection.LeftRight.value,
							}),
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 2,
								width: 685,
								height: 1400,
								openingSystem: WindowOpeningType.SideHung.value,
								openingDirection: WindowOpeningDirection.LeftRight.value,
							})
						],
					}),

		oscilobatente_3_folhas_com_fixa: new WindowModel({
						id: windowId,
						width: 2050,
						height: 1400,
						leafConfigurations: [
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 0,
								width: 680,
								height: 1400,
								openingSystem: WindowOpeningType.SideHung.value,
								openingDirection: WindowOpeningDirection.RightLeft.value,
							}),
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 1,
								width: 685,
								height: 1400,
								openingSystem: WindowOpeningType.TiltAndTurn.value,
								openingDirection: WindowOpeningDirection.LeftRight.value,
							}),
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 2,
								width: 685,
								height: 1400,
								openingSystem: WindowOpeningType.Fixed.value,
								openingDirection: WindowOpeningDirection.None.value,
							})
						],
					}),

		basculante: new WindowModel({
						id: windowId,
						width: 1000,
						height: 1000,
						leafConfigurations: [
							new LeafConfiguration({
								windowId: windowId,
								x: 0,
								y: 0,
								width: 1000,
								height: 1000,
								openingSystem: WindowOpeningType.TiltOnly.value,
								openingDirection: WindowOpeningDirection.Tilt.value,
							})
						],
					}),

	}

	export default {
		emits: ['update:modelValue', 'update:show'],

		props: {
			/*modelValue: {
				type: Object,
			},*/

			show: {
				type: Boolean,
				required: true,
				default: false,
			},
		},

		data() {
			
			return {
				defaultModels: [
					{
						id: 'fixo',
						titulo: 'Janela fixa',
						vidro: 'duplo ClimatGuard Premium 4-16-4',
						description: '',
					},

					{
						id: 'oscilobatente_1_folha',
						titulo: 'Janela Oscilobatente 1 folha',
						vidro: 'duplo ClimatGuard Premium 4-16-4',
						description: '',
					},

					{
						id: 'oscilobatente_2_folhas',
						titulo: 'Janela Oscilobatente 2 folhas',
						vidro: 'duplo ClimatGuard Premium 4-16-4',
						description: '',
					},

					{
						id: 'oscilobatente_1_folha_com_bandeira',
						titulo: 'Janela Oscilobatente 1 folha c/ Bandeira',
						vidro: 'duplo ClimatGuard Premium 4-16-4',
						description: '',
					},

					{
						id: 'oscilobatente_2_folhas_com_bandeira',
						titulo: 'Janela Oscilobatente 2 folhas c/ Bandeira',
						vidro: 'duplo ClimatGuard Premium 4-16-4',
						description: '',
					},

					{
						id: 'alta_oscilobatente_1_folha',
						titulo: 'Janela Alta Oscilobatente 1 folha',
						vidro: 'duplo ClimatGuard Premium 4-16-4',
						description: '',
					},

					{
						id: 'alta_oscilobatente_2_folhas',
						titulo: 'Janela Alta Oscilobatente 2 folhas',
						vidro: 'duplo ClimatGuard Premium 4-16-4',
						description: '',
					},

					{
						id: 'oscilobatente_3_folhas',
						titulo: 'Janela Oscilobatente 3 folhas',
						vidro: 'duplo ClimatGuard Premium 4-16-4',
						description: '',
					},

					{
						id: 'oscilobatente_3_folhas_com_fixa',
						titulo: 'Janela Oscilobatente 3 folhas com 1 fixa',
						vidro: 'duplo ClimatGuard Premium 4-16-4',
						description: '',
					},

					{
						id: 'basculante',
						titulo: 'Janela Basculante',
						vidro: 'duplo ClimatGuard Premium 4-16-4',
						description: '',
					},
				],
			};
		},

		methods: {
			isEmpty,

			onSelect(value) {
				const selectedValue = get(value, 0);
				if(selectedValue)
				{
					const selectedModel = get(defaultModels, selectedValue);
					if(selectedModel)
					{
						let model = cloneDeep(selectedModel)
						this.$emit('update:modelValue', model);
					}
				}
			},

			onClose() {
				this.$emit('update:show', false);
			},
		},
	};
</script>

<template>
	<v-dialog :modelValue="show" @update:modelValue="onClose" persistent width="auto" scrollable>
		<v-card>
			<v-card-title>
				<span class="text-h5">Selecione uma janela predefinida</span>
			</v-card-title>
			<v-card-text>
				<v-list color="primary" @update:selected="onSelect">
					<v-list-item
						v-for="janela in defaultModels"
						:key="janela.id"
						:value="janela.id"
						elevation="3"
						class="ma-2">
						<v-row>
							<v-col cols="auto">
								<v-img :src="`/img/${janela.id}.png`" alt="Perfil" width="250" height="250" />
							</v-col>
							<v-col>
								<span class="text-h6">
									{{ janela.titulo }}
								</span>
							</v-col>
						</v-row>
					</v-list-item>
				</v-list>
			</v-card-text>
		</v-card>
	</v-dialog>
</template>
