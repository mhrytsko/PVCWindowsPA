<script>
	import menuHandlers from '@/mixins/menuHandlers.js';
	import DataTableConfig from '@/mixins/dataTableConfig.js';

	export default {
		name: 'BrandsList',
		mixins: [menuHandlers],

		data() {
			return {
				menuInfo: {
					controller: 'Brands',
					supportForm: 'management-brand',
					goBack: {name: 'home'},
				},

				tableConfig: new DataTableConfig({
					headers: [
						{
							title: 'Logótipo',
							align: 'end',
							sortable: false,
							key: 'image',
							width: 64,
						},

						{
							title: 'Marca',
							align: 'start',
							sortable: true,
							key: 'name',
						},
					],
					queryUpdate: this.queryUpdate,
				}),
			};
		},
	};
</script>

<template>
	<v-container>
		<v-responsive class="align-left text-left fill-height">
			<v-row>
				<v-col>
					<m-data-table :table-config="tableConfig" title="Marcas" @row-click="handleClick">
						<template v-slot:item.image="{item}">
							<v-img :src="getImage(item.raw.imageId)" alt="Logótipo" height="64" />
						</template>
					</m-data-table>
				</v-col>
			</v-row>

			<v-row>
				<v-col cols="12">
					<v-btn block class="mb-8" color="blue" size="large" variant="tonal" @click="onAddItem">
						Adicionar uma nova marca
					</v-btn>
				</v-col>
			</v-row>
		</v-responsive>
	</v-container>
</template>
