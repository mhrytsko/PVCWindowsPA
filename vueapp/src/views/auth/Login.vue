<script>
	import {mapActions, mapState} from 'pinia';

	import {map, isArray, isString, isEmpty} from 'lodash-es';
	import Swal from 'sweetalert2';

	import netApi from '@/api/network';
	import navigation from '@/mixins/navigation.js';

	import {useUserDataStore} from '@/store/userData.js';

	export default {
		name: 'LoginPage',
		mixins: [navigation],
		data() {
			return {
				model: {
					username: '',
					password: '',
				},

				visible: false,
			};
		},
		computed: {
			...mapState(useUserDataStore, {
				userIsAuthenticated: 'isAuthenticated',
			}),
		},
		mounted() {
			// Se já é autentificado, sair para página inicial
			if (this.userIsAuthenticated) this.handleRouteReplace({name: 'home'});
		},
		methods: {
			...mapActions(useUserDataStore, {
				setUserData: 'setUser',
			}),
			async login() {
				netApi.post(
					'Auth',
					'Login',
					{
						username: this.model.username,
						password: this.model.password,
					},
					(data) => {
						if (data.success) {
							// or use a cookie instead
							this.setUserData(data);

							let redirect = this.$route.query?.redirect;
							this.handleRouteReplace(redirect ? {path: redirect} : {name: 'home'});
						}
					},
					(error) => {
						// Validation errors
						if (error.response.status === 400) {
							// Bad request
						} else if (error.response.status === 401) {
							// Unauthorized
						}

						var requestErrors = error.response.data?.errors;
						var errors = map(requestErrors, (er, propName) => {
							return isArray(er)
								? er.join(' ')
								: isString(er)
								? er
								: `Campo ${propName} está com o vlaor inválido!`;
						});

						if (!isEmpty(errors)) {
							this.$eventBus.emit('show-generic-error', {
								message: errors.join('<br>'),
							});
						}
					}
				);
			},

			recoverPassword() {
				return Swal.fire({
					title: 'Not yet, sorry',
					text: 'Estará disponível em breve',
					icon: 'warning',
				});
			},

			signUp() {
				this.handleRouteReplace({name: 'sign-up'});
			},
		},
	};
</script>

<template>
	<v-container fluid>
		<v-row justify="center">
			<v-col cols="12" sm="8" md="6">
				<v-card class="mx-auto pa-12 pb-8" elevation="8" max-width="448" rounded="lg">
					<v-form @submit.prevent="login">
						<div class="text-subtitle-1 text-medium-emphasis">Account</div>

						<v-text-field
							v-model="model.username"
							density="compact"
							placeholder="Username"
							prepend-inner-icon="mdi-account-outline"
							variant="outlined" />

						<div class="text-subtitle-1 text-medium-emphasis d-flex align-center justify-space-between">
							Password

							<a
								class="text-caption text-decoration-none text-blue"
								href="#"
								rel="noopener noreferrer"
								target="_blank"
								@click.prevent.stop="recoverPassword">
								Esqueceu a senha de login?
							</a>
						</div>

						<v-text-field
							v-model="model.password"
							:append-inner-icon="visible ? 'mdi-eye-off' : 'mdi-eye'"
							:type="visible ? 'text' : 'password'"
							density="compact"
							placeholder="Enter your password"
							prepend-inner-icon="mdi-lock-outline"
							variant="outlined"
							@click:append-inner="visible = !visible" />

						<v-card class="mb-12" color="surface-variant" variant="tonal">
							<v-card-text class="text-medium-emphasis text-caption">
								Aviso: Após 3 tentativas de login falhadas consecutivas, a sua conta será
								temporariamente bloqueada por três horas. Se você precisar fazer login agora, você
								também pode clicar em "Esqueceu a senha de login?" em cima para redefinir a senha de
								login.
							</v-card-text>
						</v-card>

						<v-btn block class="mb-8" color="blue" size="large" variant="tonal" type="submit">
							Log In
						</v-btn>

						<v-card-text class="text-center">
							<!-- <v-btn
								class="text-blue text-decoration-none"
								variant="text"
								append-icon="mdi-chevron-right"
								:to="{ name: 'sign-up' }">
								Sign up now
							</v-btn> -->
							<a
								class="text-blue text-decoration-none"
								href="#"
								rel="noopener noreferrer"
								@click.prevent.stop="signUp">
								Sign up now <v-icon icon="mdi-chevron-right"></v-icon>
							</a>
						</v-card-text>
					</v-form>
				</v-card>
			</v-col>
		</v-row>
	</v-container>
</template>
