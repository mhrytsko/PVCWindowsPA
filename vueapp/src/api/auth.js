
// class AuthService {
// 	login(user) {
// 	  return axios
// 		.post(API_URL + 'signin', {
// 		  username: user.username,
// 		  password: user.password
// 		})
// 		.then(response => {
// 		  if (response.data.accessToken) {
// 			localStorage.setItem('user', JSON.stringify(response.data));
// 		  }
  
// 		  return response.data;
// 		});
// 	}
  
// 	logout() {
// 	  localStorage.removeItem('user');
// 	}
  
// 	register(user) {
// 	  return axios.post(API_URL + 'signup', {
// 		username: user.username,
// 		email: user.email,
// 		password: user.password
// 	  });
// 	}
// }

import { useUserDataStore } from "@/store/userData.js";
import { isNumber } from 'lodash-es'

export default {
	isAuthenticated() {
		const store = useUserDataStore()

		return store.isAuthenticated;
	},

	checkUserRole(role) {
		const store = useUserDataStore()

		return store.role >= (isNumber(role) ? role : 0);
	},

	getAuthToken()
	{
		const store = useUserDataStore()

		return store.getAuthToken
	},

	logOut()
	{
		const store = useUserDataStore()
		store.logOut()
	}
};
