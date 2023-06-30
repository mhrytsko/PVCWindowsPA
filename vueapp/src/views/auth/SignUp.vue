<script>
	import {mapActions, mapState} from 'pinia';

	import {map, isArray, isString, isEmpty} from 'lodash-es';

	import netApi from '@/api/network';
	import navigation from '@/mixins/navigation.js';
	import formRules from '@/mixins/formRules.js';

	import {useUserDataStore} from '@/store/userData.js';

	export default {
		name: 'SignUpPage',
		mixins: [navigation, formRules],
		data() {
			return {
				model: {
					username: null,
					password: null,
					confirmPassword: null,
					email: null
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

			validateConfirm()
			{
				return !(isEmpty(this.model.password) || isEmpty(this.model.confirmPassword) || this.model.password !== this.model.confirmPassword)
			},
			
			validConfirm(value)
			{
				return (isEmpty(value) || this.model.password !== value) ? 'A senha está incorreta' : true
			},

			async signUp()
			{
				const { valid } = await this.$refs.form.validate()

				if(!valid)
				{
					this.$eventBus.emit('show-generic-error', {
								message: 'O formulário contém erros',
							});
				}

				netApi.post(
					'Auth',
					'Register',
					{
						username: this.model.username,
						password: this.model.password,
						email: this.model.email
					},
					(data) => {
						if (data.success)
							this.handleRouteReplace({name: 'login'});
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

			goBack()
			{
				this.handleRouteReplace({ name: 'login' })
			}
		},
	};
</script>

<template>
	<v-container fluid>
		<v-row justify="center">
			<v-col cols="12" sm="8" md="6">
				<v-card class="mx-auto pa-12 pb-8" elevation="8" max-width="448" rounded="lg">
					<v-form @submit.prevent="signUp" ref="form">
						<div class="text-subtitle-1 text-medium-emphasis">Account</div>

						<v-text-field
							v-model="model.username"
							:rules="[rules.required]"
							density="compact"
							placeholder="Username"
							prepend-inner-icon="mdi-account-outline"
							variant="outlined" />


						<div class="text-subtitle-1 text-medium-emphasis d-flex align-center justify-space-between">
							E-mail
						</div>

						<v-text-field
							v-model="model.email"
							:rules="[rules.email]"
							type="email"
							density="compact"
							variant="outlined"
							placeholder="Enter your e-mail"
							prepend-inner-icon="mdi-email-outline" />


						<div class="text-subtitle-1 text-medium-emphasis d-flex align-center justify-space-between">
							Password
						</div>

						<v-text-field
							v-model="model.password"
							:rules="[rules.required, rules.minLength(5)]"
							:append-inner-icon="visible ? 'mdi-eye-off' : 'mdi-eye'"
							:type="visible ? 'text' : 'password'"
							density="compact"
							placeholder="Enter your password"
							prepend-inner-icon="mdi-lock-outline"
							variant="outlined"
							hint="At least 5 characters"
							@click:append-inner="visible = !visible" />

						<div class="text-subtitle-1 text-medium-emphasis d-flex align-center justify-space-between">
							Confirm password
						</div>

						<v-text-field
							v-model="model.confirmPassword"
							:rules="[rules.required, rules.minLength(5), validConfirm]"
							type="password"
							density="compact"
							placeholder="Confirm password"
							prepend-inner-icon="mdi-lock-outline"
							variant="outlined"/>


						<v-btn block class="mb-8" color="blue" size="large" variant="tonal" type="submit">
							Sign up
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
								@click.prevent.stop="goBack">
								<v-icon icon="mdi-chevron-left"></v-icon>
								Go back
							</a>
						</v-card-text>
					</v-form>
				</v-card>
			</v-col>
		</v-row>
	</v-container>
</template>
