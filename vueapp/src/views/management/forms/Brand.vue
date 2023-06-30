<script>
	import Brand from '@/models/brand.js';
	import formHandlers from '@/mixins/formHandlers.js';

	import {handleFileChange} from '@/mixins/imageTools';

	export default {
		name: 'BrandForm',

		mixins: [formHandlers],

		data() {
			return {
				formInfo: {
					controller: 'Brands',
					goBack: {name: 'management-brands'},
					modelType: Brand,
				},
				model: new Brand(),
				file: null,
			};
		},

		methods: {
			handleFileChange(event) {
				handleFileChange(event, (result) => this.model.setImage(result));
			},
		},
	};
</script>

<template>
	<v-form @submit="submitForm" ref="form">
		<v-container align-items="left" fluid>
			<v-responsive class="align-center text-center fill-height">
				<v-row class="d-flex align-left justify-left">
					<v-col cols="auto">
						<div class="text-h4 font-weight-bold">Marca</div>
					</v-col>
				</v-row>

				<m-form-errors :errors="formErrors" />

				<v-row>
					<v-col>
						<v-text-field v-model="model.name" label="Nome" required :counter="150" ref="name" id="brand-name"></v-text-field>
					</v-col>
				</v-row>

				<v-row>
					<v-col>
						<v-textarea v-model="model.description" label="Descrição" :counter="10000"></v-textarea>
					</v-col>
				</v-row>

				<v-row>
					<v-col>
						<v-text-field v-model="model.site" label="Web site" type="url"></v-text-field>
					</v-col>
				</v-row>

				<v-row>
					<v-col>
						<v-img
							:src="model.image?.fileSrc"
							alt="Logótipo"
							width="200"
							height="200"
							:class="{'bg-grey-lighten-2': !model.image?.fileSrc}"></v-img>
					</v-col>
				</v-row>
				<v-row>
					<v-col sm="6" md="5" lg="4">
						<v-file-input
							v-model="file"
							label="Logótipo"
							accept="image/*"
							@change="handleFileChange"
							prepend-icon="mdi-camera"></v-file-input>
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
