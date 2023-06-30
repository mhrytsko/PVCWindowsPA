import {defineStore} from 'pinia';
//import {useStorage} from '@vueuse/core';

import netApi from '@/api/network/index.js';

import {map} from 'lodash-es';
import Brand from '@/models/brand.js';

const defaultValues = {
	brands: [],
};

//const state = useStorage('brands-data', defaultValues, sessionStorage, {mergeDefaults: true});


export const useBrandsDataStore = defineStore('brandsData', {
	state: () => defaultValues,//state,
	actions: {
		async fetchData() {
			netApi.get('Brands', undefined, {RowsPerPage: -1}, data => {
				this.brands.splice(0, this.brands.length, ...map(data.items, item => new Brand(item)))
			})
		},
	},
});
