<script>
	import {v4 as uuidv4} from 'uuid';
	import DataTableConfig from '@/mixins/dataTableConfig.js';

	export default {
		name: 'DataTable',
		inheritAttrs: false,

		emits: [
			'row-click',
			'update-options',
			'search',
			'update:modelValue'
		],

		props: {
			tableConfig: {
				type: DataTableConfig,
				default: () => new DataTableConfig(),
			},

			height: {
				type: String,
				default: '400px',
			},

			title: {
				type: String,
				default: undefined,
			},

			//items-per-page-options - https://vuetifyjs.com/en/api/v-data-table/#props-items-per-page-options
		},

		data() {
			return {
				wrapperId: uuidv4(),
				tableWrapperId: uuidv4(),
				tableId: uuidv4(),
				
				search: this.tableConfig?.search || ''
			};
		},

		// Inside your component's computed properties
		computed: {
			filteredHeaders() {
				return this.tableConfig.headers.filter((header) => this.$slots[`item.${header.key}`]);
			},
		},

		methods: {
			/*
				$emit: ((event: "update:modelValue", value: any[]) => void) 
				& ((event: "update:options", value: any) => void) 
				& ((event: "update:page", value: number) => void) 
				& ((event: "update:itemsPerPage", value: number) => void) 
				& ((event: "update:sortBy", value: any) => void) 
				& ((event: "update:groupBy", value: any) => void) 
				& ((event: "update:expanded", value: any) => void);
			*/

			onRowClick(event, eObj) {
				// Lógica para manipular o clique na linha
				console.log('Click na linha:', event, eObj);
				this.$emit('row-click', { event, item: eObj.item })
			},

			updateOptions(newOptions) {
				console.log("updateOptions:", newOptions);

				this.$emit('update-options', newOptions)
				this.tableConfig?.updateOptions({...newOptions, search: this.search});
			},

			onSearch()
			{
				this.$emit('search', this.search)
				this.tableConfig?.onSearch(this.search);
			},

			onUpdateModelValue(newValue) {
				this.$emit('update:modelValue', newValue)
			}
		},
	};
</script>

<template>
	<v-card :id="wrapperId">
		<v-card-title>
			<v-row v-if="title" dense>
				<v-col cols="auto">
					<span class="text-h5 pa-2 headline">{{ title }}</span>
				</v-col>
			</v-row>
			<v-row justify="end" dense>
				<v-col cols="9" sm="7" md="7" lg="6">
					<v-text-field v-model.lazy="search" append-icon="mdi-magnify" label="Pesquisar" @update:modelValue="onSearch"></v-text-field>
				</v-col>
			</v-row>
		</v-card-title>
		<v-container :id="tableWrapperId" class="ma-0, pa-0">
			<v-data-table-server
				:modelValue="tableConfig.selected"
				@update:modelValue="onUpdateModelValue"
				:id="tableId"
				v-bind="tableConfig"
				:height="height"
				@update:options="updateOptions"
				:onClick:row="onRowClick"
				density="comfortable"
				fixed-header
				hover>
				<!-- Slots de customização das células -->
				<template v-for="header in filteredHeaders" v-slot:[`item.${header.key}`]="{item}">
					<slot :name="`item.${header.key}`" :item="item"></slot>
				</template>

				<!-- Slot tfoot -->
				<template v-if="$slots.tfoot" v-slot:tfoot>
					<slot name="tfoot"></slot>
				</template>
			</v-data-table-server>
		</v-container>
	</v-card>
</template>

<style lang="scss" scoped>
	:deep(div.v-table__wrapper) {
		& > table > thead.v-data-table__thead > tr > th.v-data-table__th {
			background: rgba(var(--v-theme-secondary));

			.v-data-table-header__content {
				font-weight: bold;
			}
		}
	}
</style>
