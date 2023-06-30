import {defineStore} from 'pinia';

import netApi from '@/api/network/index.js';

import {map} from 'lodash-es';
import windowColor from '@/models/windowColor.js';

const defaultValues = {
	windowColors: [],
};


export const useWindowColorsDataStore = defineStore('windoColorsData', {
	state: () => defaultValues,//state,
	actions: {
		async fetchData() {
			netApi.get('WindowColors', undefined, {RowsPerPage: -1}, data => {
				this.windowColors.splice(0, this.windowColors.length, ...map(data.items, item => new windowColor(item)))
			})
		},
	},
});
