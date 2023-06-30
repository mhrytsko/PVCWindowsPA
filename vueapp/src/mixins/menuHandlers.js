import _ from 'lodash-es';

import netApi from '@/api/network';

//import sysEnums from '@/mixins/systemEnums.js';

import navigation from '@/mixins/navigation.js';

import Swal from 'sweetalert2';

export default {
	mixins: [navigation],

	props: {
		fetchDataOnMount:
		{
			type: Boolean,
			default: true
		},
	},

	data() {
		return {
			menuInfo: {
				controller: undefined,
				supportForm: undefined,
				goBack: {name: 'home'},
			},
			menuErrors: [],
			tableConfig: undefined,
		};
	},

	computed: {},

	mounted() {
		if(this.fetchDataOnMount)
			this.fetchData();
	},

	methods: {
		setData(data) {
			console.log('set data!', data);

			let _data = _.defaults(data, {
				items: [],
				itemsLength: 0,
			});

			this.tableConfig.items = _data.items;
			this.tableConfig.itemsLength = _data.itemsLength;
			this.tableConfig.loading = false;
		},

		async fetchData(query = undefined, additionalData = undefined) {
			console.log('fetch!');

			if (_.isEmpty(this.menuInfo?.controller) || _.isEmpty(this.tableConfig)) return;

			this.menuErrors = [];

			this.tableConfig.loading = true;
			netApi.get(
				this.menuInfo?.controller,
				null,
				{
					...query,
					...additionalData
				},
				(data) => {
					this.setData(data);
				},
				() => {
					this.setData(null);
				}
			);
		},

		getImage(id) {
			if(!_.isEmpty(id) && _.isString(id))
				return netApi.apiUrl('Images', 'image', id);
			else return ''
		},

		queryUpdate(query, additionalData) {
			this.fetchData(query, additionalData)
		},

		handleClick(eventData) {
			console.log('handle click', eventData);

			let params = {
				id: eventData.item.raw.id,
			};

			if(typeof this.onBeforeHandleClick === 'function')
				this.onBeforeHandleClick(params)

			if (!_.isEmpty(this.menuInfo?.supportForm)) {
				this.handleRoutePush({
					name: this.menuInfo?.supportForm,
					params
				});
			}
		},

		onAddItem() {
			console.log('add item!');

			let params = {
				id: null,
			};

			if(typeof this.onBeforeAddItem === 'function')
				this.onBeforeAddItem(params)

			if (!_.isEmpty(this.menuInfo?.supportForm)) {
				this.handleRoutePush({
					name: this.menuInfo?.supportForm,
					params,
				});
			}
		},

		showValidationErrors(resposta) {
			if (resposta.errors) {
				this.menuErrors = Object.values(resposta.errors).flat();
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
				const data = error.response.data;

				if (data) {
					console.error(data);

					if (!_.isEmpty(data?.message))
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
	},
};
