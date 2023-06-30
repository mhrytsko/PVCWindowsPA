<script>
	import {mapState} from 'pinia';
	import {useBrandsDataStore} from '@/store/brands.js';
	import {useWindowProfilesDataStore} from '@/store/windowProfile.js';
	import {useWindowColorsDataStore} from '@/store/windowColors.js';

	import BrandsDialog from './BrandsDialog.vue';
	import ProfileDialog from './ProfileDialog.vue';
	import WindowColorDialog from './WindowColorDialog.vue';
	import Window3DARDialog from './Window3DARDialog.vue';
	import DefaultModelDialog from './DefaultModelDialog.vue';
	import WindowRuler from './WindowRuler.vue';

	import {reactive, nextTick} from 'vue';
	import _ from 'lodash-es';
	//import {v4 as uuidv4} from 'uuid';

	import WindowModel from '@/models/window.js';
	import LeafConfiguration from '@/models/leafConfiguration.js';
	import formHandlers from '@/mixins/formHandlers.js';

	import enums from '@/mixins/systemEnums.js';
	import dimTools from '@/api/dimensionTools.js';

	import netApi from '@/api/network/index.js';

	export default {
		name: 'ConfigWindow', // WindowSimulation

		emits: ['update:modelValue'],

		components: {
			DefaultModelDialog,
			BrandsDialog,
			ProfileDialog,
			WindowColorDialog,
			'window-3d-ar-dialog': Window3DARDialog,
			WindowRuler,
		},

		mixins: [formHandlers],

		props:
		{
			modelValue: {
				type: WindowModel,
			},
		},

		data() {
			//let windowId = uuidv4();
			return {
				lodash: _,

				windowOpeningType: enums.WindowOpeningTypeAsArray,
				windowOpeningDirection: enums.WindowOpeningDirectionAsArray,
				colorType: enums.ColorTypeAsArray,

				// Download buttons «Loading...»
				loadingPdf: false,
				loading2DImage: false,

				// Dialogs
				showSelectDefaultModel: this.modelValue ? false : true,
				showSelectBrand: false,
				showSelectProfile: false,
				showSelectIndorColor: false,
				showSelectOutdorColor: false,
				showRuler: false,
				showAR: false,

				// Selected brand
				profileBrandId: null,

				// Model (Window)
				model: this.modelValue,

				modelInitialized: false,

				// Selected Leaf (folha)
				selectedLeaf: undefined,

				// 2D Download Options
				photo2D: {
					withSchema: true,
					withSize: false,
					applyTextures: true,
				},

				// 3D & AR
				ar3D: {
					tryWall: false,
					showSkybox: false,
					useARCore: false,
				}
			};
		},

		computed: {
			...mapState(useBrandsDataStore, ['brands']),
			...mapState(useWindowProfilesDataStore, ['windowProfiles']),
			...mapState(useWindowColorsDataStore, ['windowColors']),

			brandWindowProfiles() {
				return _.filter(this.windowProfiles, {brandId: this.profileBrandId});
			},

			brandWindowColors() {
				return _.filter(this.windowColors, {brandId: this.profileBrandId});
			},

			folhasMatriz() {
				const numRows = _.isEmpty(this.model.leafConfigurations)
					? 0
					: _.maxBy(this.model.leafConfigurations, (leaf) => leaf.x).x + 1;
				const paneMatrix = Array(numRows)
					.fill()
					.map((__, row) =>
						Array(_.groupBy(this.model.leafConfigurations, {x: row}).true).fill({
							width: 0,
							height: 0,
							openingSystem: enums.WindowOpeningType.Fixed.value,
							openingDirection: enums.WindowOpeningDirection.None.value,
						})
					);

				// Initializar o paneMatrix com dados das folhas
				this.model.leafConfigurations.forEach((pane) => {
					paneMatrix[pane.x][pane.y] = pane;
				});

				return paneMatrix;
			},

			selectedLeafUId() {
				return _.get(this.selectedLeaf, 'uid');
			},
		},

		mounted() {
			console.log('Config window - mounted');
			if(this.model)
			{
				let profile = _.first(this.windowProfiles, {id: this.model.windowProfileId})
				this.profileBrandId = profile?.brandId;
				this.selectedLeaf = _.first(this.model.leafConfigurations);

				this.modelInitialized = true;
			}
		},

		beforeUnmount() {},

		methods: {
			onSelectDefaultModel(newModel) {
				if (newModel instanceof WindowModel) {
					this.model = new WindowModel(newModel);
					this.selectedLeaf = _.first(this.model.leafConfigurations);
					this.showSelectDefaultModel = false;

					this.$emit('update:modelValue', this.model);

					nextTick(() => { this.modelInitialized = true; })
				}
			},

			handleWindowSectionClick(eventData) {
				this.selectedLeaf = reactive(eventData.pane);
			},

			updateWidth(rowX) {
				_.forEach(this.folhasMatriz, (row, rowNum) => {
					if (rowX === undefined || rowX === rowNum) {
						const dimensoes = dimTools.dividirDimensao(this.model.width, row.length);
						_.forEach(row, (col, colIndex) => {
							col.width = dimensoes[colIndex];
						});
					}
				});
			},

			updateHeight() {
				const dimensoes = dimTools.dividirDimensao(this.model.height, this.folhasMatriz.length);

				_.forEach(this.folhasMatriz, (row, rowIndex) => {
					_.forEach(row, (col) => {
						col.height = dimensoes[rowIndex];
					});
				});
			},

			openSelectBrand() {
				this.showSelectBrand = true;
			},

			openSelectProfile() {
				this.showSelectProfile = true;
			},

			openSelectIndorColor() {
				this.showSelectIndorColor = true;
			},

			openSelectOutdorColor() {
				this.showSelectOutdorColor = true;
			},

			openRuler() {
				this.showRuler = true;
			},

			openAR() {
				this.showAR = true;
			},

			reorderMatrix() {
				const rowGroups = _.groupBy(_.orderBy(this.model.leafConfigurations, ['x', 'y'], ['asc', 'asc']), 'x');
				const orderedGroups = _.orderBy(_.toPairs(rowGroups), ([key]) => parseInt(key, 10)); // o -1 ficava no fim

				// reordenar
				let newOrderX = 0;
				_.forEach(orderedGroups, (row) => {
					let newOrderY = 0;
					_.forEach(row[1], (col) => {
						col.x = newOrderX;
						col.y = newOrderY++;
					});
					newOrderX++;
				});
			},

			removeFirstRow() {
				// validações do tamanho minimo
				let numRows = _.maxBy(this.model.leafConfigurations, 'x')?.x || 0;
				if (numRows === 0) return;

				// descelecionar a folha
				this.selectedLeaf = undefined;

				// remover row
				_.remove(this.model.leafConfigurations, (leaf) => leaf.x === 0);

				// reordenar rows
				this.reorderMatrix();

				// atualizar alturas
				this.updateHeight();

				// selecionar a folha
				this.selectedLeaf = this.folhasMatriz[0][0];
			},

			removeLastRow() {
				// validações do tamanho minimo
				let numRows = _.maxBy(this.model.leafConfigurations, 'x')?.x || 0;
				if (numRows === 0) return;

				// descelecionar a folha
				this.selectedLeaf = undefined;

				// remover row
				_.remove(this.model.leafConfigurations, (leaf) => leaf.x === numRows);

				// atualizar alturas
				this.updateHeight();

				// selecionar a folha
				this.selectedLeaf = this.folhasMatriz[0][0];
			},

			addNewRowAtStart() {
				const numRows = _.maxBy(this.model.leafConfigurations, 'x')?.x || 0;
				const maxRows = Math.floor(this.model.height / 300);

				// Validar o numero máximo de secções verticais ( - 1  => começa no 0)
				if (Number.isNaN(maxRows) || numRows >= maxRows - 1) return;

				this.model.leafConfigurations.push(
					new LeafConfiguration({
						windowId: this.model.id,
						x: -1,
						y: -1,
						width: this.model.width,
						height: 0,
						openingSystem: enums.WindowOpeningType.Fixed.value,
						openingDirection: enums.WindowOpeningDirection.None.value,
					})
				);

				this.reorderMatrix();

				this.updateHeight();

				// selecionar a folha
				this.selectedLeaf = this.folhasMatriz[0][0];
			},

			addNewRowAtEnd() {
				const numRows = _.maxBy(this.model.leafConfigurations, 'x')?.x || 0;
				const maxRows = Math.floor(this.model.height / 300);

				// Validar o numero máximo de secções verticais  ( - 1  => começa no 0)
				if (Number.isNaN(maxRows) || numRows >= maxRows - 1) return;

				this.model.leafConfigurations.push(
					new LeafConfiguration({
						windowId: this.model.id,
						x: numRows + 1,
						y: 0,
						width: this.model.width,
						height: 0,
						openingSystem: enums.WindowOpeningType.Fixed.value,
						openingDirection: enums.WindowOpeningDirection.None.value,
					})
				);

				this.updateHeight();

				// selecionar a folha
				this.selectedLeaf = _.last(this.model.leafConfigurations);
			},

			removeLeaf() {
				const selectedRow = _.get(this.selectedLeaf, 'x', 0);
				const curRow = _.filter(this.model.leafConfigurations, (leaf) => leaf.x === selectedRow);
				const numRows = _.maxBy(this.model.leafConfigurations, 'x')?.x || 0;
				const lastColumn = _.maxBy(curRow, 'y')?.y || 0;
				const isLastColumn = lastColumn === 0;

				// validação
				if (isLastColumn && numRows === 0) return;

				// descelecionar a folha
				this.selectedLeaf = undefined;

				// remover a ultima folha
				_.remove(this.model.leafConfigurations, (leaf) => leaf.x === selectedRow && leaf.y === lastColumn);

				if (isLastColumn) {
					this.reorderMatrix();
					this.updateHeight();
				} else this.updateWidth(selectedRow);

				// selecionar a folha
				this.selectedLeaf = isLastColumn ? this.folhasMatriz[0][0] : _.last(this.folhasMatriz[selectedRow]);
			},

			addLeaf() {
				const selectedRow = _.get(this.selectedLeaf, 'x', 0);
				const curRow = _.filter(this.model.leafConfigurations, (leaf) => leaf.x === selectedRow);
				const numColumns = _.maxBy(curRow, 'y')?.y || 0;
				const maxColumns = Math.floor(this.model.width / 300);

				// Validar o numero máximo de secções horizontais  ( - 1  => começa no 0)
				if (Number.isNaN(maxColumns) || numColumns >= maxColumns - 1) return;

				const selectedRowHeight = _.get(this.selectedLeaf, 'height', 0);

				this.model.leafConfigurations.push(
					new LeafConfiguration({
						windowId: this.model.id,
						x: selectedRow,
						y: numColumns + 1,
						width: 0,
						height: selectedRowHeight,
						openingSystem: enums.WindowOpeningType.Fixed.value,
						openingDirection: enums.WindowOpeningDirection.None.value,
					})
				);

				this.updateWidth(selectedRow);

				// selecionar a folha
				this.selectedLeaf = _.last(this.folhasMatriz[selectedRow]);
			},

			increaseVertical() {
				const increaseBy = 50;
				const selectedRow = _.get(this.selectedLeaf, 'x', 0);

				const prevRow = _.filter(this.model.leafConfigurations, (leaf) => leaf.x === selectedRow - 1);
				const curRow = _.filter(this.model.leafConfigurations, (leaf) => leaf.x === selectedRow);
				const nextRow = _.filter(this.model.leafConfigurations, (leaf) => leaf.x === selectedRow + 1);

				let increaseSize = 0;

				// retirar da anterior
				if (!_.isEmpty(prevRow)) {
					let rowSize = _.maxBy(prevRow, 'height')?.height || 0;
					let canIncrease =
						rowSize - increaseBy > 300 ? increaseBy : rowSize === 300 ? 0 : (300 - rowSize) * -1;

					_.forEach(prevRow, (leaf) => (leaf.height -= canIncrease));

					increaseSize += canIncrease;
				}

				// retirar da próxima
				if (!_.isEmpty(nextRow)) {
					const rowSize = _.maxBy(nextRow, 'height')?.height || 0;
					const canIncrease =
						rowSize - increaseBy > 300 ? increaseBy : rowSize === 300 ? 0 : (300 - rowSize) * -1;

					_.forEach(nextRow, (leaf) => (leaf.height -= canIncrease));

					increaseSize += canIncrease;
				}

				// adicionar ao selecionado
				_.forEach(curRow, (leaf) => (leaf.height += increaseSize));
			},

			decreaseVertical() {
				const decreaseBy = 50;
				const selectedRow = _.get(this.selectedLeaf, 'x', 0);

				const prevRow = _.filter(this.model.leafConfigurations, (leaf) => leaf.x === selectedRow - 1);
				const curRow = _.filter(this.model.leafConfigurations, (leaf) => leaf.x === selectedRow);
				const nextRow = _.filter(this.model.leafConfigurations, (leaf) => leaf.x === selectedRow + 1);

				const rowSize = _.maxBy(curRow, 'height')?.height || 0;

				const hasPrev = !_.isEmpty(prevRow),
					hasNext = !_.isEmpty(nextRow);

				const toIncrease = hasPrev && hasNext ? 2 : hasPrev || hasNext ? 1 : 0,
					needDecrease = decreaseBy * toIncrease,
					canDecrease =
						rowSize - needDecrease > 300 ? needDecrease : rowSize === 300 ? 0 : (300 - rowSize) * -1,
					increaseSize = canDecrease > 0 && toIncrease > 0 ? Math.floor(canDecrease / toIncrease) : 0,
					decreaseSize = increaseSize * toIncrease;

				// adicionar ao anterior
				if (hasPrev) _.forEach(prevRow, (leaf) => (leaf.height += increaseSize));

				// adicionar ao próxima
				if (hasNext) _.forEach(nextRow, (leaf) => (leaf.height += increaseSize));

				// retirar do selecionado
				_.forEach(curRow, (leaf) => (leaf.height -= decreaseSize));
			},

			increaseHorizontal() {
				const increaseBy = 50;
				const selectedRow = _.get(this.selectedLeaf, 'x', 0);
				const selectedColumn = _.get(this.selectedLeaf, 'y', 0);
				const curRow = _.filter(this.model.leafConfigurations, (leaf) => leaf.x === selectedRow);

				const prevLeaf = _.find(curRow, (leaf) => leaf.x === selectedRow && leaf.y === selectedColumn - 1);
				const curLeaf = _.find(curRow, (leaf) => leaf.x === selectedRow && leaf.y === selectedColumn);
				const nextLeaf = _.find(curRow, (leaf) => leaf.x === selectedRow && leaf.y === selectedColumn + 1);

				let increaseSize = 0;

				// retirar da anterior
				if (!_.isEmpty(prevLeaf)) {
					let rowSize = _.get(prevLeaf, 'width', 0);
					let canIncrease =
						rowSize - increaseBy > 300 ? increaseBy : rowSize === 300 ? 0 : (300 - rowSize) * -1;

					prevLeaf.width -= canIncrease;

					increaseSize += canIncrease;
				}

				// retirar da próxima
				if (!_.isEmpty(nextLeaf)) {
					const rowSize = _.get(nextLeaf, 'width', 0);
					const canIncrease =
						rowSize - increaseBy > 300 ? increaseBy : rowSize === 300 ? 0 : (300 - rowSize) * -1;

					nextLeaf.width -= canIncrease;

					increaseSize += canIncrease;
				}

				// adicionar ao selecionado
				curLeaf.width += increaseSize;
			},

			decreaseHorizontal() {
				const decreaseBy = 50;
				const selectedRow = _.get(this.selectedLeaf, 'x', 0);
				const selectedColumn = _.get(this.selectedLeaf, 'y', 0);
				const curRow = _.filter(this.model.leafConfigurations, (leaf) => leaf.x === selectedRow);

				const prevLeaf = _.find(curRow, (leaf) => leaf.x === selectedRow && leaf.y === selectedColumn - 1);
				const curLeaf = _.find(curRow, (leaf) => leaf.x === selectedRow && leaf.y === selectedColumn);
				const nextLeaf = _.find(curRow, (leaf) => leaf.x === selectedRow && leaf.y === selectedColumn + 1);

				const rowSize = _.get(curLeaf, 'width', 0);
				const hasPrev = !_.isEmpty(prevLeaf),
					hasNext = !_.isEmpty(nextLeaf);

				const toIncrease = hasPrev && hasNext ? 2 : hasPrev || hasNext ? 1 : 0,
					needDecrease = decreaseBy * toIncrease,
					canDecrease =
						rowSize - needDecrease > 300 ? needDecrease : rowSize === 300 ? 0 : (300 - rowSize) * -1,
					increaseSize = canDecrease > 0 && toIncrease > 0 ? Math.floor(canDecrease / toIncrease) : 0,
					decreaseSize = increaseSize * toIncrease;

				// adicionar ao anterior
				if (hasPrev) prevLeaf.width += increaseSize;

				// adicionar ao próxima
				if (hasNext) nextLeaf.width += increaseSize;

				// retirar do selecionado
				curLeaf.width -= decreaseSize;
			},

			getPdf(print = false) {
				this.loadingPdf = true;
				netApi.downloadData(
					'Budgets',
					'GetWindowBudget',
					this.model,
					() => (this.loadingPdf = false),
					async (error) => {
						this.loadingPdf = false;
						await this.onRequestErrors(error);
					},
					undefined,
					print ? netApi.downloadType.PRINT : netApi.downloadType.DOWNLOAD
				);
			},

			printPdf() {
				this.getPdf(true)
			},

			get2DImage() {
				this.loading2DImage = true;
				netApi.downloadData(
					'Images',
					'GetWindow2DImage',
					this.model,
					() => (this.loading2DImage = false),
					async (error) => {
						this.loading2DImage = false;
						await this.onRequestErrors(error);
					},
					{
						params: this.photo2D,
					},
					netApi.downloadType.IMAGE
				);
			},
		},

		watch: {
			profileBrandId(newValue) {
				if (this.modelInitialized) {
					if (!_.some(this.brandWindowProfiles, {brandId: newValue})) this.model.windowProfileId = null;
					if (!_.some(this.brandWindowColors, {brandId: newValue})) this.model.indorColorId = null;
					if (!_.some(this.brandWindowColors, {brandId: newValue})) this.model.outdorColorId = null;
				}
			},

			'model.width'() {
				if (this.modelInitialized) this.updateWidth();
			},

			'model.height'() {
				if (this.modelInitialized) this.updateHeight();
			},

			model: {
				handler() {
					if (this.modelInitialized) this.$emit('update:modelValue', this.model);
				},
				deep: true,
			},
		},
	};
</script>

<template>
	<v-form ref="form">
		<v-container align-items="left">
			<template v-if="model">
				<v-row>
					<v-col cols="12" sm="6" md="3" lg="3">
						<v-select
							v-model="profileBrandId"
							:items="brands"
							item-value="id"
							item-title="name"
							label="Marca do perfil"
							append-icon="mdi-open-in-new"
							clearable
							@click:append="openSelectBrand" />
					</v-col>
					<v-col cols="12" sm="6" md="3" lg="3">
						<v-select
							v-model="model.windowProfileId"
							:items="brandWindowProfiles"
							item-value="id"
							item-title="name"
							label="Perfil"
							append-icon="mdi-open-in-new"
							clearable
							@click:append="openSelectProfile" />
					</v-col>
				</v-row>

				<v-row>
					<v-col cols="12" sm="6" md="3" lg="3">
						<v-select
							v-model="model.indorColorId"
							:items="brandWindowColors"
							item-value="id"
							item-title="name"
							label="Cor no interior"
							append-icon="mdi-open-in-new"
							clearable
							@click:append="openSelectIndorColor" />
					</v-col>
					<v-col cols="12" sm="6" md="3" lg="3">
						<v-select
							v-model="model.outdorColorId"
							:items="brandWindowColors"
							item-value="id"
							item-title="name"
							label="Cor no exterior"
							append-icon="mdi-open-in-new"
							clearable
							@click:append="openSelectOutdorColor" />
					</v-col>
				</v-row>

				<v-row>
					<v-col cols="auto">
						<v-btn icon="mdi-ruler" @click="openRuler"/>
					</v-col>
				</v-row>
				<v-row>
					<v-col cols="6" sm="6" md="3" lg="2">
						<m-numeric v-model.lazy="model.width" :min="300" :max="5000" label="Largura" suffix="mm" />
					</v-col>
					<v-col cols="6" sm="6" md="3" lg="2">
						<m-numeric v-model.lazy="model.height" :min="300" :max="3000" label="Altura" suffix="mm" />
					</v-col>
				</v-row>

				<v-divider class="pb-1" />

				<v-row dense class="pb-2" style="max-width: 400px">
					<v-col cols="12">
						<v-row dense class="pb-3" style="max-width: 350px">
							<v-col cols="4">
								<v-btn @click="removeFirstRow" rounded="xl" class="float-right">
									<v-icon icon="mdi-minus"></v-icon>1
									<v-icon icon="mdi-arrow-collapse-down"></v-icon>
								</v-btn>
							</v-col>
							<v-col cols="1" align-self="center" class="text-center">
								<span class="text-h5">{{ folhasMatriz.length }}</span>
							</v-col>
							<v-col cols="4">
								<v-btn @click="addNewRowAtStart" rounded="xl" class="float-left">
										<v-icon icon="mdi-plus"></v-icon>1
										<v-icon icon="mdi-arrow-collapse-up"></v-icon>
								</v-btn>
							</v-col>

							<template v-if="selectedLeaf">
								<v-col cols="3">
									<v-btn @click="addLeaf" rounded="xl" class="float-right">
										<v-icon icon="mdi-arrow-collapse-right"></v-icon>
										<v-icon icon="mdi-plus"></v-icon>1
									</v-btn>
								</v-col>
							</template>
						</v-row>
					</v-col>
					<v-col cols="auto">
						<m-simple-2d-window
							:window-width="model.width"
							:window-height="model.height"
							:panes="model.leafConfigurations"
							:selected-leaf="selectedLeaf"
							@leaf-click="handleWindowSectionClick"
							:canvas-width="300"
							:canvas-height="300" />
					</v-col>

					<v-col cols="12">
						<v-row dense class="pb-3" style="max-width: 350px">
							<v-col cols="4">
								<v-btn @click="removeLastRow" rounded="xl" class="float-right">
									<v-icon icon="mdi-minus"></v-icon>1
									<v-icon icon="mdi-arrow-collapse-up"></v-icon>
								</v-btn>
							</v-col>
							<v-col cols="1" align-self="center" class="text-center">
								<span class="text-h5">{{ folhasMatriz.length }}</span>
							</v-col>
							<v-col cols="4">
								<v-btn @click="addNewRowAtEnd" rounded="xl" class="float-left">
									<v-icon icon="mdi-plus"></v-icon>1
									<v-icon icon="mdi-arrow-collapse-down"></v-icon>
								</v-btn>
							</v-col>

							<template v-if="selectedLeaf">
								<v-col cols="3">
									<v-btn @click="removeLeaf" rounded="xl" class="float-right" >
										<v-icon icon="mdi-arrow-collapse-left"></v-icon>
										<v-icon icon="mdi-minus"></v-icon>1
									</v-btn>
								</v-col>
							</template>
						</v-row>
					</v-col>

					<v-col cols="12">
						<small
							><i><b>Selecionam uma folha para editar</b></i></small
						>
					</v-col>
				</v-row>

				<v-divider class="pb-1" />

				<template v-if="selectedLeaf">
					<v-row>
						<v-col cols="auto">
							<v-row dense>
								<v-col cols="auto">
									<div class="text-body-1">
										Redimensionar a folha (<span
											><b>{{ selectedLeaf.width }}</b></span
										>
										x
										<span
											><b>{{ selectedLeaf.height }}</b></span
										>)
									</div>
								</v-col>
							</v-row>
							<v-row dense>
								<v-col cols="auto" class="pr-3">
									<v-btn icon="mdi-arrow-expand-vertical" size="large" @click="increaseVertical" />
								</v-col>
								<v-col cols="auto" class="pr-3">
									<v-btn
										icon="mdi-arrow-collapse-vertical"
										size="large"
										@click="decreaseVertical" />
								</v-col>
								<v-col cols="auto" class="pr-3">
									<v-btn icon="mdi-arrow-expand-horizontal" size="large" @click="increaseHorizontal" />
								</v-col>
								<v-col cols="auto">
									<v-btn
										icon="mdi-arrow-collapse-horizontal"
										size="large"
										@click="decreaseHorizontal" />
								</v-col>
							</v-row>
						</v-col>
					</v-row>

					<v-row>
						<v-col cols="6" sm="6" md="3" lg="2">
							<v-select
								v-model="selectedLeaf.openingSystem"
								:items="windowOpeningType"
								label="Sistema de abertura"
								item-title="text"
								item-value="value" />
						</v-col>
						<v-col cols="6" sm="6" md="3" lg="2">
							<v-select
								v-model="selectedLeaf.openingDirection"
								:items="windowOpeningDirection"
								label="Direção da abertura"
								item-title="text"
								item-value="value" />
						</v-col>
					</v-row>

					<v-row>
						<v-col cols="auto">
							<v-switch v-model="selectedLeaf.frosted" label="Fosco" color="primary" />
						</v-col>
					</v-row>

					<!-- <v-row>
						<v-col cols="12">Debug: {{ selectedLeaf }}</v-col>
					</v-row> -->
				</template>

				<v-row>
					<v-col cols="12" sm="8" md="6" lg="4">
						<v-textarea
							v-model="model.description"
							label="Descrição da janela (opcional)"
							:counter="10000"
							:rows="2"
							:max-rows="2"
							no-resize />
					</v-col>
				</v-row>

				<v-row>
					<v-col cols="auto">
						<v-btn
							prepend-icon="mdi-file-download-outline"
							size="x-large"
							@click="getPdf"
							:loading="loadingPdf">
							Orçamento em Pdf
						</v-btn>
					</v-col>
				</v-row>
				<v-row>
					<v-col cols="auto">
						<v-btn
							prepend-icon="mdi-printer-outline"
							size="x-large"
							@click="printPdf"
							:loading="loadingPdf">
							Imprimir
						</v-btn>
					</v-col>
					<v-col cols="auto">
						<v-btn
							prepend-icon="mdi-image-search-outline"
							size="x-large"
							@click="get2DImage"
							:loading="loading2DImage">
							2D Foto
						</v-btn>
					</v-col>
				</v-row>

				<v-row dense>
					<v-col cols="auto">
						<v-checkbox
							v-model="photo2D.withSchema"
							label="Incluir direção da abertura"
							color="primary"
							hide-details />
					</v-col>
					<v-col cols="auto">
						<v-checkbox v-model="photo2D.withSize" label="Incluir dimensões" color="primary" hide-details />
					</v-col>
					<v-col cols="auto">
						<v-checkbox
							v-model="photo2D.applyTextures"
							label="Aplicar textura"
							color="primary"
							hide-details />
					</v-col>
				</v-row>

				<v-row>
					<v-col>
						<m-form-errors :errors="formErrors" />
					</v-col>
				</v-row>

				<v-row justify="start" class="text-left">
					<v-col>
						NOTA: Devido à varios fatores, as cores e texturas mostradas neste site podem divergir
						ligeiramente das cores reais dos perfis
					</v-col>
				</v-row>



				<v-divider class="pb-1 mt-5" />
				<v-row justify="start">
					<v-col cols="auto">
						<v-btn prepend-icon="mdi-cube-scan" size="large" @click="openAR">
							3D & Realidade Aumentada
						</v-btn>
					</v-col>
				</v-row>
				<v-row class="pb-2 mt-1">
					<!-- <v-col cols="auto">
						<v-switch v-model="ar3D.useARCore" label="Google AR" color="primary" />
					</v-col> -->
					<v-col cols="auto">
						<v-switch v-model="ar3D.tryWall" label="Paredes" color="primary" />
					</v-col>
					<v-col cols="auto">
						<v-switch v-model="ar3D.showSkybox" label="Skybox" color="primary" />
					</v-col>
				</v-row>

				<!-- Dialogs -->
				<v-row justify="center">
					<brands-dialog v-model="profileBrandId" v-model:show="showSelectBrand" />
					<profile-dialog
						v-model="model.windowProfileId"
						v-model:show="showSelectProfile"
						:profile-brand-id="profileBrandId" />
					<window-color-dialog
						v-model="model.indorColorId"
						v-model:show="showSelectIndorColor"
						:profile-brand-id="profileBrandId" />
					<window-color-dialog
						v-model="model.outdorColorId"
						v-model:show="showSelectOutdorColor"
						:profile-brand-id="profileBrandId" />

					<window-ruler v-model:show="showRuler" />

					<window-3d-ar-dialog
						v-model:show="showAR"
						v-model:modelValue="model"
						:useARCore="ar3D.useARCore"
						:tryWall="ar3D.tryWall"
						:showSkybox="ar3D.showSkybox" />
				</v-row>
			</template>
			<!-- Dialogs -->
			<v-row justify="center">
				<default-model-dialog @update:modelValue="onSelectDefaultModel" v-model:show="showSelectDefaultModel" />
			</v-row>
		</v-container>
	</v-form>
</template>

<style></style>
