<script>
	import menuHandlers from '@/mixins/menuHandlers.js';
	import DataTableConfig from '@/mixins/dataTableConfig.js';

	import {mapState} from 'pinia';
	import {useBrandsDataStore} from '@/store/brands.js';

	import { find, get } from 'lodash-es'

	export default {
		name: 'BrandsList',
		mixins: [menuHandlers],

		data() {
			return {
				menuInfo: {
					controller: 'WindowColors',
					supportForm: 'management-window-color',
					goBack: {name: 'home'},
				},

				tableConfig: new DataTableConfig({
					headers: [
						{
							title: 'Cor',
							align: 'end',
							sortable: false,
							key: 'hexCode',
							width: 64,
						},

						{
							title: 'Nome',
							align: 'start',
							sortable: true,
							key: 'name',
						},

						{
							title: 'Marca',
							align: 'start',
							sortable: true,
							key: 'brand',
						},
					],
					queryUpdate: this.queryUpdate,
				}),
			};
		},

		computed:
		{
			...mapState(useBrandsDataStore, ['brands']),
		},

		methods:
		{
			getColorImage()
			{

			},

			getBrandImage(brandId)
			{
				return get(find(this.brands, { id: brandId }), 'image.fileSrc', '')
			}
		},
	};
</script>

<template>
	<v-container>
		<v-responsive class="align-left text-left fill-height">
			<v-row>
				<v-col>
					<m-data-table :table-config="tableConfig" title="Cores do perfil" @row-click="handleClick">
						<template v-slot:item.hexCode="{item}">
							<v-sheet v-if="item.raw.colorType === 0" :color="item.raw.hexCode" height="64" width="64" elevation="8" class="ma-1"/>
							<v-img v-else-if="item.raw.colorType === 1" :src="getImage(item.raw.imageId)" alt="Cor" height="64" width="64" elevation="8" class="ma-1" />
							<v-sheet v-else width="64"> - </v-sheet>
						</template>
						<template v-slot:item.brand="{item}">
							<v-img :src="getBrandImage(item.raw.brandId)" alt="Cor" height="30" />
						</template>
					</m-data-table>
				</v-col>
			</v-row>

			<v-row>
				<v-col cols="12">
					<v-btn block class="mb-8" color="blue" size="large" variant="tonal" @click="onAddItem">
						Adicionar uma nova cor
					</v-btn>
				</v-col>
			</v-row>
		</v-responsive>
	</v-container>
</template>
