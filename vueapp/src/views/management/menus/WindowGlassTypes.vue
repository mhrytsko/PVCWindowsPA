<script>
	import menuHandlers from '@/mixins/menuHandlers.js';
	import DataTableConfig from '@/mixins/dataTableConfig.js';

	export default {
		name: 'WindowGlassTypesList',
		mixins: [menuHandlers],

		data() {
			return {
				menuInfo: {
					controller: 'WindowGlassTypes',
					supportForm: 'management-window-glass-type',
					goBack: {name: 'home'},
				},

				tableConfig: new DataTableConfig({
					headers: [
						{
							title: 'Window photo',
							align: 'end',
							sortable: false,
							key: 'image',
							width: 64,
						},

						{
							title: 'Marca',
							align: 'start',
							sortable: true,
							key: 'brand.name',
						},

						{
							title: 'Name',
							align: 'start',
							sortable: true,
							key: 'name',
						},
					],
					queryUpdate: this.queryUpdate,
				}),
			};
		},

		methods: {},
	};
</script>

<template>
	<v-container>
		<v-responsive class="align-left text-left fill-height">
			<v-row>
				<v-col>
					<m-data-table :table-config="tableConfig" title="Tipos de janelas" @row-click="handleClick">
						<template v-slot:item.image="{item}">
							<v-img :src="getImage(item.raw.imageId)" alt="Window photo" height="64" />
						</template>
					</m-data-table>
				</v-col>
			</v-row>

			<v-row>
				<v-col cols="12">
					<v-btn block class="mb-8" color="blue" size="large" variant="tonal" @click="onAddItem">
						Adicionar um novo tipo de vidro
					</v-btn>
				</v-col>
			</v-row>
		</v-responsive>
	</v-container>
</template>
