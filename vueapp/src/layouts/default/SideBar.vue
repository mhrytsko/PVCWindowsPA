<script>
	import {mapState, mapActions} from 'pinia';
	import {useUserDataStore} from '@/store/userData.js';

	import {isEmpty, find} from 'lodash-es';

	import systemEnums from '@/mixins/systemEnums.js';
	import navigation from '@/mixins/navigation.js';

	export default {
		mixins: [navigation],
		props: {
			drawer: {
				type: Boolean,
				default: false,
			},
		},
		data() {
			return {
				levels: systemEnums.PermissionLevel,
			};
		},
		computed: {
			...mapState(useUserDataStore, ['firstName', 'lastName', 'role', 'isAuthenticated']),

			userTitle() {
				return (isEmpty(this.firstName)
					? ''
					: `${this.firstName} `) + (isEmpty(this.lastName)
					? ''
					: `${this.lastName}`);
			},

			userSubtitle() {
				return (
					find(systemEnums.PermissionLevel, (userRole) => userRole.value === this.role)?.text ||
					systemEnums.PermissionLevel.Public.text
				);
			},
		},

		methods: {
			...mapActions(useUserDataStore, ['logOut']),

			checkRole(accessLevel) {
				return this.role >= accessLevel.value;
			},

			onLogOut() {
				this.logOut();
				this.handleRouteReplace({name: 'home'});
			},
		},
	};
</script>

<template>
	<v-navigation-drawer :permanent="!drawer" :rail="drawer" location="left">
		<template v-if="isAuthenticated">
			<v-list-item id="user-avatar" :title="userTitle" :subtitle="userSubtitle" nav>
				<template #prepend>
					<v-avatar color="info" icon="mdi-account" />
				</template>
			</v-list-item>
			<v-divider></v-divider>
		</template>

		<v-list density="compact" nav>
			<v-list-item prepend-icon="mdi-home-outline" title="Página inicial" :to="{name: 'home'}" value="home" />

			<v-list-item
				prepend-icon="mdi-window-closed-variant"
				title="Configurar Janelas"
				:to="{name: 'config-window'}"
				value="config-window"></v-list-item>
		</v-list>

		<v-divider v-if="checkRole(levels.Client)" />
		<v-list v-if="checkRole(levels.Client)" density="compact" nav>
			<v-list-item prepend-icon="mdi-window-shutter-cog" title="Janelas" value="management-windows" :to="{ name: 'management-windows' }"></v-list-item>
			<v-list-item prepend-icon="mdi-list-box-outline" title="Orçamentos" value="management-budgets" :to="{ name: 'management-budgets' }"></v-list-item>
			<v-list-item v-if="checkRole(levels.Technic)" prepend-icon="mdi-account-group" title="Clientes" value="management-clients" :to="{ name: 'management-clients' }"></v-list-item>
		</v-list>

		<v-divider v-if="checkRole(levels.Admin)" />
		<v-list v-if="checkRole(levels.Admin)" density="compact" nav>
			<v-list-subheader>Gestão</v-list-subheader>
			<v-list-item
				prepend-icon="mdi-account-multiple"
				title="Utilizadores"
				:to="{name: 'management-users'}"
				value="management-users" />

			<v-list-item
				prepend-icon="mdi-trademark"
				title="Marcas"
				:to="{name: 'management-brands'}"
				value="management-brands"></v-list-item>

			<v-list-item
				prepend-icon="mdi-artboard"
				title="Perfis"
				:to="{name: 'management-window-profiles'}"
				value="management-window-profiles"></v-list-item>

			<v-list-item
				prepend-icon="mdi-mirror-rectangle"
				title="Vidros"
				:to="{name: 'management-window-glass-types'}"
				value="management-window-glass-types"></v-list-item>

			<v-list-item
				prepend-icon="mdi-palette"
				title="Cores do perfil"
				:to="{name: 'management-window-colors'}"
				value="management-window-colors"></v-list-item>

			<v-list-item
				prepend-icon="mdi-tools"
				title="Tools"
				:to="{name: 'management-tools'}"
				value="management-tools"></v-list-item>
		</v-list>

		<v-divider></v-divider>
		<v-list-item
			v-if="isAuthenticated"
			prepend-icon="mdi-power"
			title="Log out"
			value="logOut"
			@click="onLogOut"></v-list-item>
	</v-navigation-drawer>
</template>
