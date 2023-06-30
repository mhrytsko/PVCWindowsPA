<script>
	import {mapState} from 'pinia';
	import {useUserDataStore} from '@/store/userData.js';

	import {PermissionLevel} from '@/mixins/systemEnums.js';
	export default {
		name: 'HomePage',

		data() {
			return {
				levels: PermissionLevel,
			};
		},

		computed: {
			...mapState(useUserDataStore, ['role', 'isAuthenticated']),
		},

		methods: {
			checkRole(accessLevel) {
				return this.role >= accessLevel.value;
			},
		},
	};
</script>

<template>
	<v-container class="fill-height">
		<v-responsive class="align-center text-center fill-height">
			<v-icon icon="mdi-window-open-variant" size="x-large" />

			<div class="text-body-2 font-weight-light mb-n1">Welcome to</div>

			<h1 class="text-h2 font-weight-bold">Orçamentos de Janelas PVC</h1>

			<div class="py-14"></div>

			<v-row class="d-flex align-center justify-center">
				<v-col cols="auto" v-if="checkRole(levels.Technic)">
					<v-btn :to="{name: 'management-budgets'}" min-width="164" rel="noopener noreferrer" variant="text">
						<v-icon icon="mdi-view-dashboard" size="large" start />

						Orçamentos
					</v-btn>
				</v-col>

				<v-col cols="auto">
					<v-btn
						color="primary"
						:to="{name: 'config-window'}"
						min-width="228"
						rel="noopener noreferrer"
						size="x-large"
						variant="flat">
						<v-icon icon="mdi-speedometer" size="large" start />

						Começar
					</v-btn>
				</v-col>

				<v-col cols="auto" v-if="checkRole(levels.Technic)">
					<v-btn :to="{name: 'management-clients'}" min-width="164" rel="noopener noreferrer" variant="text">
						<v-icon icon="mdi-account-group" size="large" start />

						Clientes
					</v-btn>
				</v-col>
			</v-row>

			<v-row v-if="!isAuthenticated" class="d-flex align-center justify-center">
				<v-card-text class="text-center">
					<v-btn
						class="text-blue text-decoration-none"
						variant="text"
						append-icon="mdi-chevron-right"
						:to="{name: 'login'}">
						Iniciar sessão
					</v-btn>
				</v-card-text>
			</v-row>
		</v-responsive>
	</v-container>
</template>
