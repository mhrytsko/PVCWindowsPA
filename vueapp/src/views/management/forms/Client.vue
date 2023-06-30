<script>
	import Client from '@/models/client.js';
	import formHandlers from '@/mixins/formHandlers.js';
	
	import Budgets from '@/views/management/menus/BudgetsByClient.vue'
	
	import formRules from '@/mixins/formRules.js';
	import netApi from '@/api/network';
	import _ from 'lodash-es';

	export default {
		name: 'ClientForm',

		components:
		{
			Budgets
		},

		mixins: [formHandlers],

		data() {
			return {
				formInfo: {
					controller: 'Clients',
					goBack: {name: 'management-clients'},
					modelType: Client,
				},
				model: new Client(),

				formRules
			};
		},

		created() {

		},

		methods: {
			isEmpty: _.isEmpty
		},
	};
</script>

<template>
	<v-form @submit="submitForm" ref="form">
		<v-container align-items="left" fluid>
			<v-responsive class="align-center text-center fill-height">
				<v-row class="d-flex align-left justify-left">
					<v-col cols="auto">
						<div class="text-h4 font-weight-bold">Cliente</div>
					</v-col>
				</v-row>

				<m-form-errors :errors="formErrors" />

				<v-row>
					<v-col cols="12" sm="6" md="4" lg="3">
						<v-text-field v-model="model.personalDetail.firstName" label="Primeiro nome" />
					</v-col>
					<v-col cols="12" sm="6" md="4" lg="3">
						<v-text-field v-model="model.personalDetail.lastName" label="Apelido" />
					</v-col>
				</v-row>

				<v-row>
					<v-col sm="8" md="8" lg="6">
						<v-text-field
							v-model="model.personalDetail.email"
							type="email"
							label="E-mail"
							prepend-inner-icon="mdi-email-outline"
							:rules="[rules.email]" />
					</v-col>
				</v-row>

				<v-row>
					<v-col sm="6" md="4" lg="3">
						<v-text-field v-model="model.personalDetail.phone" label="Tel" prepend-inner-icon="mdi-phone" />
					</v-col>
				</v-row>

				<Budgets v-if="formState.loaded && !isEmpty(model.id)" :client-id="model.id" :fetch-data-on-mount="false" />
				
				<m-record-details v-bind="model" />

				<m-form-footer-buttons
					@submit="submitForm"
					@apply="applyForm"
					@cancel="cancelForm"
					@delete="deleteForm"
					:show-delete="!formNewMode"
					:show-apply="true" />
			</v-responsive>
		</v-container>
	</v-form>
</template>
