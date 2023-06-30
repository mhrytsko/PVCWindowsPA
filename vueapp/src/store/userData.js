import {defineStore} from 'pinia';
import {useStorage} from '@vueuse/core';

import {keys, pick, assignIn} from 'lodash-es';

const defaultUserValues = {
	id: '',
	userName: 'guest',
	type: 0,
	role: 0,
	firstName: '',
	lastName: '',
	token: '',
	isAuthenticated: false
};

function getUserValues(newValues) {
	return pick(newValues || defaultUserValues, keys(defaultUserValues));
}

const state = useStorage('user-data', defaultUserValues, /*sessionStorage*/localStorage, {mergeDefaults: true});

export const useUserDataStore = defineStore('userData', {
	state: () => state,
	getters: {
		getAuthToken: (state) => state.token,
	},
	actions: {
		setUser(newData) {
			assignIn(this, getUserValues(newData));
		},
		logOut() {
			assignIn(this, defaultUserValues);
		},
	},
});
