<script>
	import UserViewModel from './userViewModel.js';
	import sysEnums from '@/mixins/systemEnums.js';

	import formHandlers from '@/mixins/formHandlers.js';

	import formRules from '@/mixins/formRules.js';

	export default {
		name: 'UserData',

		mixins: [formRules, formHandlers],

		props: {
			id: {
				type: String,
				required: true,
			},
		},

		data() {
			return {
				formInfo: {
					controller: 'Users',
					goBack: {name: 'management-users'},
					modelType: UserViewModel,
				},

				model: new UserViewModel(),

				roleItems: sysEnums.PermissionLevelAsArray,
			};
		},
	};
</script>

<template>
	<v-form @submit="submitForm">
		<v-container align-items="left" fluid>
			<v-responsive class="align-center text-center fill-height">
				<v-row class="d-flex align-left justify-left">
					<v-col cols="auto">
						<div class="text-h4 font-weight-bold">Utilizador</div>
					</v-col>
				</v-row>

				<m-form-errors :errors="formErrors" />

				<v-row>
					<v-col sm="6" md="5" lg="4">
						<v-text-field
							v-model="model.userName"
							label="User Name"
							:rules="[rules.required]"
							maxlength="50"
							required
							prepend-inner-icon="mdi-account-outline" />
					</v-col>
				</v-row>

				<v-row>
					<v-col sm="6" md="5" lg="4">
						<v-text-field
							v-model="model.password"
							label="Palavra-passe"
							type="password"
							prepend-inner-icon="mdi-lock-outline" />
					</v-col>
				</v-row>

				<v-row>
					<v-col sm="6" md="4" lg="3">
						<v-select
							v-model="model.role"
							:items="roleItems"
							label="Role"
							item-title="text"
							item-value="value" />
					</v-col>
				</v-row>

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

				<v-row>
					<v-col sm="auto">
						<v-switch v-model="model.state" label="Ativo" color="primary" :false-value="0" :true-value="1" />
					</v-col>
				</v-row>

				<m-record-details v-if="model" v-bind="model" />

				<m-form-footer-buttons
					@submit="submitForm"
					@cancel="cancelForm"
					@delete="deleteForm"
					:show-delete="!formNewMode" />
			</v-responsive>
		</v-container>
	</v-form>
</template>
