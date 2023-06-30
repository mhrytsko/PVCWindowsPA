<script>
	import menuHandlers from '@/mixins/menuHandlers.js';
	import DataTableConfig from '@/mixins/dataTableConfig.js';
	//import netApi from '@/mixins/netApi';

	export default {
		name: 'WindowList',
		mixins: [menuHandlers],

		data() {
			return {
				menuInfo: {
					controller: 'Windows',
					supportForm: 'management-window',
					goBack: {name: 'home'},
				},

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
							minWidth: 75,
							noPadding: true
						},

						{
							title: 'Criado em',
							align: 'start',
							sortable: true,
							key: 'creationDate',
						},
					],
					queryUpdate: this.queryUpdate,
				})
			};
		},

		methods: {
			getStrDate(value)
			{
				let dateT = Date.parse(value);
				if(Number.isNaN(dateT))
					return '';
				else
				{
					try
					{
						let date = new Date(dateT);
						return date.toLocaleString();
					}
					catch
					{
						return '';
					}
					
				}
			},
		},
	};
</script>

<template>
	<v-container>
		<v-responsive class="align-left text-left fill-height">
			<m-form-errors :errors="menuErrors" />

			<v-row dense>
				<v-col>
					<div class="table-wrapper">
						<m-data-table :table-config="tableConfig" title="Janelas" @row-click="handleClick">
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
					</div>
					
				</v-col>
			</v-row>

			
			<v-row>
				<v-col cols="12">
					<v-btn block class="mb-8" color="blue" size="large" variant="tonal" @click="onAddItem">
						Adicionar uma nova janela
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
