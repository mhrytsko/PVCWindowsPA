import {mapState} from 'pinia';
import {useUserDataStore} from '@/store/userData.js';
import { reactive } from 'vue';

import {isEmpty, isString} from 'lodash-es';

import netApi from '@/api/network';

//import sysEnums from '@/mixins/systemEnums.js';

import formRules from '@/mixins/formRules.js';

import navigation from '@/mixins/navigation.js';

import Swal from 'sweetalert2';

export default {
	mixins: [formRules, navigation],

	props: {
		id: {
			type: String,
		},
	},

	data() {
		return {
			recordId: this.id,
			formInfo: {
				controller: undefined,
				goBack: undefined,
				modelType: undefined,
			},
			formState:
			{
				loaded: false,
				loading: false
			},
			formErrors: [],
		};
	},

	computed: {
		...mapState(useUserDataStore, {
			userIsAuthenticated: 'isAuthenticated'
		}),

		formIsBlocked() {
			return false;
		},

		formNewMode() {
			return isEmpty(this.recordId);
		},
	},

	created() {
		this.fetchData();
	},

	methods: {
		setModel(data) {

			if(!this.model && this.formInfo.modelType)
				this.model = reactive(new this.formInfo.modelType(data));
			else if(typeof this.model.hydrate === 'function')
				this.model.hydrate(data);
			else this.model = data;
		},

		async fetchData() {
			console.log('fetch!');

			if(!this.userIsAuthenticated) return;

			if (isEmpty(this.formInfo?.controller) || isEmpty(this.recordId)) return;

			this.formErrors = [];

			netApi.get(
				this.formInfo.controller,
				this.recordId,
				{
					id: this.recordId,
				},
				(data) => {
					console.log(`Fetch data - ${this.formInfo.controller}`, data);

					this.setModel(data);

					this.formState.loaded = true

					if(typeof this.onAfterLoad === 'function')
						this.onAfterLoad()
				},
				async (error) => await this.onRequestErrors(error)
			);
		},

		//onBeforeSubmit() { },
		//onAfterSubmit(/*responseData*/) { },

		async submitForm(exitOnSuccess = true) {
			console.log('submit!');

			if(!this.userIsAuthenticated) return;

			if (isEmpty(this.formInfo?.controller)) return;

			this.formErrors = [];

			if(this.$refs.form)
			{
				const { valid } = await this.$refs.form.validate()
				if(!valid)
				{
					this.$eventBus.emit('show-generic-error', {
						message: 'O formulário contém erros',
					});
				}
			}

			if(typeof this.onBeforeSubmit === 'function')
				this.onBeforeSubmit()

			netApi.post(
				this.formInfo.controller,
				this.formNewMode ? 'New' : 'Edit',
				this.model?.toSrvData(),
				(data) => {
					if(typeof this.onAfterSubmit === 'function')
						this.onAfterSubmit(data)

					if(exitOnSuccess)
						this.handleRouteReplace(this.formInfo?.goBack);
					else
					{
						this.swalFire({
							title: 'Gravado com sucesso!',
							icon: 'success',
						})

						if(this.formNewMode)
						{
							this.recordId = data.id
							this.fetchData()
							const currentRoute = this.$router.currentRoute.value
							if(currentRoute)
							{
								this.handleRouteReplace({
									name: currentRoute.name,
									params: {
										...currentRoute.params,
										id: this.recordId
									}
								})
							}
						}
					}
						
				},
				async (error) => await this.onRequestErrors(error)
			);
		},

		async applyForm()
		{
			return this.submitForm(false)
		},

		cancelForm() {
			console.log('cancel!');
			this.handleRouteReplace(this.formInfo?.goBack);
		},

		deleteForm() {
			console.log('delete!');

			if(!this.userIsAuthenticated) return;

			if (isEmpty(this.formInfo?.controller) || isEmpty(this.recordId)) return;

			this.swalFire({
				title: 'Tem certeza?',
				text: 'Deseja eliminar este registro?',
				icon: 'warning',
				showCancelButton: true,
				confirmButtonText: 'Sim',
				cancelButtonText: 'Não',
			}).then((result) => {
				if (result.isConfirmed) {
					netApi.post(
						this.formInfo.controller,
						'Delete',
						null,
						() => {
							this.handleRouteReplace(this.formInfo?.goBack);
						},
						async (error) => await this.onRequestErrors(error),
						{ params: { id: this.recordId } }
					);
				}
			});
		},

		showValidationErrors(resposta) {
			if (resposta.errors) {
				this.formErrors = Object.values(resposta.errors).flat();
			}
		},

		async onRequestErrors(error) {
			/**
			 * 401 - Unauthorized
			 * 400 - Bad request / Validation errors
			 * 404 - Not found
			 * 500 - Server Error
			 */

			if (error.response.status === 400) {
				var data = null;
				try {
					const isJsonBlob = (data) => data instanceof Blob && (data.type === "application/json" || data.type === "application/problem+json");
					const responseData = isJsonBlob(error.response?.data) ? await (error.response?.data)?.text() : error.response?.data || {};
					data = (typeof responseData === "string") ? JSON.parse(responseData) : responseData;
				} catch {
					data = {
						message: `Erro ao processar o seu pedido (${error.response.status})`,
					};
				}

				if (data) {
					console.error(data);

					if (!isEmpty(data?.message))
						this.$eventBus.emit('show-generic-error', {
							message: data?.message,
						});
					else
						this.$eventBus.emit('show-generic-error', {
							message: `Erro ao processar o seu pedido (${error.response.status})`,
						});

					this.showValidationErrors(data);
				}
			} else
				this.$eventBus.emit('show-generic-error', {
					message: `Erro ao processar o seu pedido (${error.response.status})`,
				});
		},

		swalFire(options) {
			return Swal.fire(options);
		},

		getImage(id) {
			if (!isEmpty(id) && isString(id)) return netApi.apiUrl('Images', 'image', id);
			else return '';
		},
	},
};
