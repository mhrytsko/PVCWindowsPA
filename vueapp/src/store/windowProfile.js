import {defineStore} from 'pinia';
//import {useSessionStorage} from '@vueuse/core';

import netApi from '@/api/network/index.js';

import {map} from 'lodash-es';
import WindowProfile from '@/models/windowProfile.js';

const defaultValues = {
	windowProfiles: [],
};

//const state = useSessionStorage('window-profiles-data', defaultValues, {mergeDefaults: true});


export const useWindowProfilesDataStore = defineStore('windowProfilesData', {
	state: () => defaultValues,// state,
	actions: {
		async fetchData() {
			netApi.get('WindowProfiles', undefined, {RowsPerPage: -1}, data => {
				this.windowProfiles.splice(0, this.windowProfiles.length, ...map(data.items, item => new WindowProfile(item)))
			})
		},
	},
});
