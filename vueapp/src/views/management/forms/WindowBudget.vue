<script>
	import Window from '@/models/window.js';
	import formHandlers from '@/mixins/formHandlers.js';
	
	import ConfigWindow from '@/views/windowConfig/ConfigWindow.vue';

	//import netApi from '@/api/network';
	//import _ from 'lodash-es'

	export default {
		name: 'WindowBudgetForm',

		components: {
			ConfigWindow
		},

		mixins: [formHandlers],

		props:
		{
			budgetId:
			{
				type: String
			},
		},

		data() {
			return {
				formInfo: {
					controller: 'Windows',
					goBack: {name: 'management-budget', params: { id: this.budgetId }},
					modelType: Window,
				},

				model: undefined,
			};
		},
	};
</script>

<template>
	<v-form @submit="submitForm" ref="form">
		<v-container align-items="left" fluid>
			<v-responsive class="align-center text-center fill-height">
				<v-row class="d-flex align-left justify-left">
					<v-col cols="auto">
						<div class="text-h4 font-weight-bold">Janela</div>
					</v-col>
				</v-row>

				<m-form-errors :errors="formErrors" />

				<v-row v-if="formNewMode || formState.loaded">
					<v-col cols="12">
						<config-window v-model="model" />
					</v-col>
				</v-row>
				

				<v-divider class="pb-1 mt-5" />
				
				<m-record-details v-if="model" v-bind="model" />

				<m-form-footer-buttons
					v-if="model"
					@submit="submitForm"
					@apply="applyForm"
					@cancel="cancelForm"
					@delete="deleteForm"
					:show-delete="!formNewMode"
					:show-apply="!formNewMode" />
			</v-responsive>
		</v-container>
	</v-form>
</template>
