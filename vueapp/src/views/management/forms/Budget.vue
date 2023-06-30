<script>
	import Budget from '@/models/budget.js';
	import BudgetWindow from '@/models/budgetWindow.js';
	import formHandlers from '@/mixins/formHandlers.js';
	import DataTableConfig from '@/mixins/dataTableConfig.js';

	import netApi from '@/api/network';
	import _ from 'lodash-es';

	export default {
		name: 'BudgetForm',

		mixins: [formHandlers],

		props:
		{
			clientId:
			{
				type: String
			},
		},

		data() {
			return {
				formInfo: {
					controller: 'Budgets',
					goBack: {name: 'management-budgets'},
					modelType: Budget,
				},
				model: new Budget({ clientId: this.clientId }),

				tableConfig: new DataTableConfig({
					headers: [
						{
							title: 'Janela',
							align: 'start',
							sortable: true,
							key: 'image',
							width: 100,
						},

						{
							title: 'Largura x Altura',
							align: 'center',
							sortable: true,
							key: 'dimension',
						},

						{
							title: 'Criado em',
							align: 'start',
							sortable: true,
							key: 'creationDate',
						},
					],
					selected: [],
					showSelect: true,
					returnObject: false,
					queryUpdate: this.queryUpdate,
				}),

				loadingPdf: false,

				alreadyInitialized: false,
			};
		},

		created() {
			this.fetchListData();
		},

		methods: {
			onAfterLoad() {},

			onAfterListLoad() {
				if (!this.alreadyInitialized) {
					this.tableConfig.selected = _.map(
						_.filter(this.tableConfig.items, (w) => _.some(this.model.budgetWindows, {windowId: w.id})),
						(item) => item.id
					);
					this.alreadyInitialized = true;
				}
			},

			onBeforeSubmit() {
				this.model.budgetWindows = _.map(
					this.tableConfig.selected,
					(selectedWindow) =>
						new BudgetWindow({
							budgetId: this.model.id,
							windowId: selectedWindow,
						})
				);
			},

			setListData(data) {
				console.log('set list data!', data);

				let _data = _.defaults(data, {
					items: [],
					itemsLength: 0,
				});

				this.tableConfig.items = _data.items;
				this.tableConfig.itemsLength = _data.itemsLength;
				this.tableConfig.loading = false;
			},

			async fetchListData(query = undefined) {
				console.log('fetch list!');

				this.menuErrors = [];

				this.tableConfig.loading = true;
				netApi.get(
					'Windows',
					null,
					query,
					(data) => {
						this.setListData(data);
						this.onAfterListLoad();
					},
					() => {
						this.setListData(null);
					}
				);
			},

			queryUpdate(query) {
				this.fetchListData(query);
			},

			getBudget(print = false) {
				this.loadingPdf = true;
				netApi.downloadData(
					'Budgets',
					'GetBudgetById',
					{id: this.model.id},
					() => (this.loadingPdf = false),
					async (error) => {
						this.loadingPdf = false;
						await this.onRequestErrors(error);
					},
					{
						params: {
							id: this.model.id,
						},
					},
					print ? netApi.downloadType.PRINT : netApi.downloadType.DOWNLOAD
				);
			},

			printBudget()
			{
				this.getBudget(true);
			},

			getStrDate(value) {
				let dateT = Date.parse(value);
				if (Number.isNaN(dateT)) return '';
				else {
					try {
						let date = new Date(dateT);
						return date.toLocaleString();
					} catch {
						return '';
					}
				}
			},

			handleClick(eventData) {
				console.log('handle click', eventData, eventData.item.raw.id);
			},

			onUpdateModelValue(newValue) {
				console.log('onUpdateModelValue', newValue);
				this.tableConfig.selected = newValue;
			},

			onAddItem()
			{
				console.log('add item!');

				if(_.isEmpty(this.recordId)) return;

				let params = {
					id: null,
					budgetId: this.recordId
				};

				if(typeof this.onBeforeAddItem === 'function')
					this.onBeforeAddItem(params)

				this.handleRoutePush({
					name: 'management-window-budget',
					params,
				});
			},
		},
	};
</script>

<template>
	<v-form @submit="submitForm" ref="form">
		<v-container align-items="left" fluid>
			<v-responsive class="align-center text-center fill-height">
				<v-row class="d-flex align-left justify-left">
					<v-col cols="auto">
						<div class="text-h4 font-weight-bold">Orçamento</div>
					</v-col>
				</v-row>

				<m-form-errors :errors="formErrors" />

				<v-row>
					<v-col cols="auto">
						<v-text-field v-model="model.budgetNumber" label="Nª" variant="underlined" readonly />
					</v-col>
				</v-row>

				<v-row>
					<v-col cols="12">
						<m-data-table
							:table-config="tableConfig"
							title="Janelas"
							@row-click="handleClick"
							@update:modelValue="onUpdateModelValue">
							<template v-slot:item.image="{item}">
								<m-simple-2d-window
									class="pt-1"
									:window-width="item.raw.width"
									:window-height="item.raw.height"
									:panes="item.raw.leafConfigurations"
									:canvas-width="100"
									:canvas-height="100" />
							</template>
							<template v-slot:item.dimension="{item}">
								<span class="text-overline">(L) {{ item.raw.width }} x (A) {{ item.raw.height }}</span>
							</template>
							<template v-slot:item.creationDate="{item}">
								<span class="text-overline">{{ getStrDate(item.raw.creationDate) }}</span>
							</template>
						</m-data-table>
					</v-col>
				</v-row>

				<v-row>
					<v-col cols="12">
						<v-btn block class="mb-8" color="blue" size="large" variant="tonal" @click="onAddItem">
							Adicionar uma nova janela
						</v-btn>
					</v-col>
				</v-row>

				<v-row>
					<v-col cols="auto">
						<v-btn
							prepend-icon="mdi-file-download-outline"
							size="x-large"
							@click="getBudget"
							:loading="loadingPdf"
							:disabled="formNewMode">
							Orçamento em Pdf
						</v-btn>
					</v-col>
					<v-col cols="auto">
						<v-tooltip :open-on-click="true" text="O ficheiro conterá apenas informações gravadas">
							<template v-slot:activator="{props}">
								<v-btn variant="plain" icon v-bind="props" v-on="props">
									<v-icon color="grey-lighten-1"> mdi-help-circle-outline </v-icon>
								</v-btn>
							</template>
						</v-tooltip>
					</v-col>
				</v-row>
				<v-row>
					<v-col cols="auto">
						<v-btn
							prepend-icon="mdi-printer-outline"
							size="large"
							@click="printBudget"
							:loading="loadingPdf"
							:disabled="formNewMode">
							Imprimir
						</v-btn>
					</v-col>
				</v-row>
				
				<m-record-details v-bind="model" />

				<m-form-footer-buttons
					@submit="submitForm"
					@apply="applyForm"
					@cancel="cancelForm"
					@delete="deleteForm"
					:show-delete="!formNewMode"
					:show-apply="true" />
			</v-responsive>
		</v-container>
	</v-form>
</template>
