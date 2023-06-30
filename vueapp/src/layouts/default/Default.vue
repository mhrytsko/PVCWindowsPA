<script>
	import SideBar from './SideBar.vue';
	import GenericErrors from '@/components/GenericErrors.vue';

	import {mapState, mapActions} from 'pinia';
	import {useUserDataStore} from '@/store/userData.js';

	import navigation from '@/mixins/navigation.js';

	export default {
		name: 'DefaultLayoutView',
		mixins: [navigation],
		components: {
			SideBar,
			GenericErrors,
		},

		data() {
			return {
				drawer: true,
			};
		},

		computed: {
			...mapState(useUserDataStore, {
				userRole: 'role',
				userIsAuthenticated: 'isAuthenticated'
			}),
		},

		methods: {
			...mapActions(useUserDataStore, {
				userLogOut: 'logOut'
			}),

			checkRole(accessLevel) {
				return this.userRole >= accessLevel.value;
			},

			logIn() {
				this.handleRouteReplace({name: 'login'});
			},

			logOut()
			{
				this.userLogOut();
				this.handleRouteReplace({name: 'home'});
			}
		},

		watch: {
			'$route.name': {
				handler: function (routeName) {
					if (routeName === 'login') this.drawer = true;
				},
				deep: true,
				immediate: true,
			},
		},
	};
</script>

<template>
	<v-card>
		<v-layout>
			<v-app>
				<side-bar :drawer="drawer"></side-bar>

				<v-app-bar color="primary" density="compact" :elevation="2" size="64">
					<template v-slot:prepend>
						<v-app-bar-nav-icon @click.stop="drawer = !drawer"></v-app-bar-nav-icon>
					</template>

					<v-app-bar-title>Janelas PVC</v-app-bar-title>

					<template v-slot:append>
						<v-btn
							v-if="!userIsAuthenticated"
								prepend-icon="mdi-power"
								class="text-blue text-decoration-none"
								variant="outlined"
								@click="logIn">
								Entrar
							</v-btn>
						<v-menu v-else>
							<template v-slot:activator="{props}">
								<v-btn icon="mdi-dots-vertical" v-bind="props"></v-btn>
							</template>
							<v-list density="compact" nav>
								<v-list-item
									v-show="userIsAuthenticated"
									prepend-icon="mdi-power"
									title="Log out"
									value="log-out"
									@click="logOut" />
							</v-list>
						</v-menu>
					</template>
				</v-app-bar>

				<v-main min-height="300px" scrollable>
					<router-view />
				</v-main>

				<generic-errors />
			</v-app>
		</v-layout>
	</v-card>
</template>

<style>
	#user-avatar > div.v-list-item__content > div.v-list-item-subtitle {
		-webkit-box-orient: horizontal;
	}
</style>
