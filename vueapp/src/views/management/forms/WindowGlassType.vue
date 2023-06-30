<script>
	import WindowGlassType from '@/models/windowGlassType.js';
	import formHandlers from '@/mixins/formHandlers.js';

	import {handleFileChange} from '@/mixins/imageTools';
	import netApi from '@/api/network';

	export default {
		name: 'WindowGlassTypeForm',

		mixins: [formHandlers],

		data() {
			return {
				formInfo: {
					controller: 'windowGlassTypes',
					goBack: {name: 'management-window-glass-types'},
					modelType: WindowGlassType,
				},
				model: new WindowGlassType(),
				file: null,
				brands: [],
			};
		},

		created() {
			this.loadBrands();
		},

		methods: {
			handleFileChange(event) {
				handleFileChange(event, (result) => this.model.setImage(result));
			},

			loadBrands() {
				netApi.get(
					'Brands',
					undefined,
					{
						rowsPerPage: -1,
					},
					(data) => {
						console.log(`Fetch data - Brands`, data);
						this.brands = data?.items || [];
					},
					async (error) => await this.onRequestErrors(error)
				);
			},
		},
	};
</script>

<template>
	<v-form @submit="submitForm" ref="form">
		<v-container fluid>
			<v-responsive class="align-center text-center fill-height">
				<v-row class="d-flex align-left justify-left">
					<v-col cols="auto">
						<div class="text-h4 font-weight-bold">Tipo de vidro</div>
					</v-col>
				</v-row>

				<m-form-errors :errors="formErrors" />

				<v-row>
					<v-col cols="12">
						<v-text-field v-model="model.name" label="Name" required></v-text-field>
					</v-col>
				</v-row>
				<v-row>
					<v-col cols="12">
						<v-textarea v-model="model.description" label="Descrição" :maxlength="10000"></v-textarea>
					</v-col>
				</v-row>
				<v-row>
					<v-col cols="6">
						<v-select v-model="model.brandId" :items="brands" label="Marca" required></v-select>
					</v-col>
				</v-row>
				<v-row>
					<v-col>
						<v-img
							:src="model.image?.fileSrc"
							alt="Foto do vidro"
							width="200"
							height="200"
							:class="{'bg-grey-lighten-2': !model.image?.fileSrc}"></v-img>
					</v-col>
				</v-row>
				<v-row>
					<v-col sm="6" md="5" lg="4">
						<v-file-input
							v-model="file"
							label="Foto do vidro"
							accept="image/*"
							@change="handleFileChange"
							prepend-icon="mdi-camera"></v-file-input>
					</v-col>
				</v-row>
				<v-row>
					<v-col cols="6">
						<v-text-field v-model.number="model.thickness" label="Espessura do vidro" type="number" suffix="mm"></v-text-field>
					</v-col>
					<v-col cols="6">
						<v-text-field v-model.number="model.chamberCount" label="Número de camadas" type="number"></v-text-field>
					</v-col>
				</v-row>
				<v-row>
					<v-col cols="6" sm="4" md="2" lg="2">
						<v-text-field
							v-model.number="model.thermalInsulation"
							label="Isolamento térmico"
							type="number"
							prefix="Uw até"
							suffix="W/m²K"></v-text-field>
					</v-col>
					<v-col cols="6" sm="4" md="2" lg="2">
						<v-text-field
							v-model.number="model.soundInsulation"
							label="Isolamento acústico"
							type="number"
							prefix="Rw até"
							suffix="dB"></v-text-field>
					</v-col>
				</v-row>
				<v-row>
					<v-col cols="12">
						<v-text-field v-model="model.antiTheftResistance" label="Resistência antirroubo"></v-text-field>
					</v-col>
				</v-row>
				<v-row>
					<v-col cols="12">
						<v-checkbox v-model="model.frosted" label="Fosco"></v-checkbox>
					</v-col>
				</v-row>
				<m-record-details v-bind="model" />
				<v-row>
					<v-col cols="12">
						<m-form-footer-buttons
							@submit="submitForm"
							@cancel="cancelForm"
							@delete="deleteForm"
							:show-delete="!formNewMode" />
					</v-col>
				</v-row>
			</v-responsive>
		</v-container>
	</v-form>
</template>
