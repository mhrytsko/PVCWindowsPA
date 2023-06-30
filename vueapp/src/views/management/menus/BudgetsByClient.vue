<script>
	import menuHandlers from '@/mixins/menuHandlers.js';
	import DataTableConfig from '@/mixins/dataTableConfig.js';
	import netApi from '@/api/network/index.js';

	import {set} from 'lodash-es'

	export default {
		name: 'BudgetByClientList',
		mixins: [menuHandlers],

		props:
		{
			clientId:
			{
				type: String
			},
		},

		data() {
			return {
				menuInfo: {
					controller: 'Budgets',
					supportForm: 'management-budget-client',
					goBack: {name: 'home'},
				},

				tableConfig: new DataTableConfig({
					headers: [
						{
							title: 'Nº',
							align: 'start',
							sortable: true,
							key: 'budgetNumber',
						},

						{
							title: '',
							align: 'end',
							sortable: false,
							key: 'actions',
						},
					],
					queryUpdate: (query) => {
						this.queryUpdate(query, {
							clientId: this.clientId
						})
					},
				}),

				loadingPdf: false,
			};
		},

		methods: {
			onBeforeAddItem(params)
			{
				set(params, 'clientId', this.clientId)
			},

			onBeforeHandleClick(params)
			{
				set(params, 'clientId', this.clientId)
			},

			getBudget(budget, print = false) {
				this.loadingPdf = true;
				netApi.downloadData(
					'Budgets',
					'GetBudgetById',
					{id: budget.id},
					() => (this.loadingPdf = false),
					async (error) => {
						this.loadingPdf = false;
						await this.onRequestErrors(error);
					},
					{
						params: {id: budget.id}
					},
					print ? netApi.downloadType.PRINT : netApi.downloadType.DOWNLOAD
				);
			},

			printBudget(budget)
			{
				this.getBudget(budget, true);
			}
		},
	};
</script>

<template>
	<v-container>
		<v-responsive class="align-left text-left fill-height">
			<m-form-errors :errors="menuErrors" />

			<v-row>
				<v-col>
					<m-data-table :table-config="tableConfig" title="Orçamentos" @row-click="handleClick">
						<template v-slot:item.actions="{item}">
							<v-btn
								variant="text"
								size="small"
								class="me-2"
								@click.stop="getBudget(item.raw)"
								:loading="loadingPdf"
								icon="mdi-image-search-outline" />
						</template>
					</m-data-table>
				</v-col>
			</v-row>

			<v-row>
				<v-col cols="12">
					<v-btn block class="mb-8" color="blue" size="large" variant="tonal" @click="onAddItem">
						Adicionar um novo orçamento
					</v-btn>
				</v-col>
			</v-row>
		</v-responsive>
	</v-container>
</template>
