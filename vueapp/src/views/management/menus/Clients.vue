<script>
	import menuHandlers from '@/mixins/menuHandlers.js';
	import DataTableConfig from '@/mixins/dataTableConfig.js';
	//import netApi from '@/mixins/netApi';

	export default {
		name: 'ClientsList',
		mixins: [menuHandlers],

		data() {
			return {
				menuInfo: {
					controller: 'Clients',
					supportForm: 'management-client',
					goBack: {name: 'home'},
				},

				tableConfig: new DataTableConfig({
					headers: [
						{
							title: 'Nome',
							align: 'start',
							sortable: true,
							key: 'personalDetail.firstName',
						},

						{
							title: 'Apelido',
							align: 'start',
							sortable: true,
							key: 'personalDetail.lastName',
						},
					],
					queryUpdate: this.queryUpdate,
				})
			};
		},

		methods: {
			
		},
	};
</script>

<template>
	<v-container>
		<v-responsive class="align-left text-left fill-height">
			<m-form-errors :errors="menuErrors" />

			<v-row>
				<v-col>
					<div class="table-wrapper">
						<m-data-table :table-config="tableConfig" title="Clientes" @row-click="handleClick" />
					</div>
				</v-col>
			</v-row>

			
			<v-row>
				<v-col cols="12">
					<v-btn block class="mb-8" color="blue" size="large" variant="tonal" @click="onAddItem">
						Adicionar um novo cliente
					</v-btn>
				</v-col>
			</v-row>
		</v-responsive>
	</v-container>
</template>

<style scoped lang="scss">
.table-wrapper {
	overflow-x: scroll;
}
</style>
