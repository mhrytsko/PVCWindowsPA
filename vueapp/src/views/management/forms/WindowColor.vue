<script>
	import WindowColor from '@/models/windowColor.js';
	import formHandlers from '@/mixins/formHandlers.js';
	import {ColorType} from '@/mixins/systemEnums.js';

	import {handleFileChange} from '@/mixins/imageTools';
	import netApi from '@/api/network';

	export default {
		name: 'WindowColorForm',

		mixins: [formHandlers],

		data() {
			return {
				formInfo: {
					controller: 'WindowColors',
					goBack: {name: 'management-window-colors'},
					modelType: WindowColor,
				},
				model: new WindowColor(),
				file: null,
				colorTypes: ColorType,
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

		watch: {
			'model.colorType'(newValue) {
				if (newValue === ColorType.Solid.value) {
					this.model.imageId = null;
					this.model.image = null;
				} else if (newValue === ColorType.Pattern.value) {
					this.model.hexCode = null;
				}
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
						<div class="text-h4 font-weight-bold">Cor do perfil</div>
					</v-col>
				</v-row>

				<m-form-errors :errors="formErrors" />

				<v-row>
					<v-col cols="12" sm="6" md="3" lg="2">
						<v-select
							v-model="model.brandId"
							:items="brands"
							item-value="id"
							item-title="name"
							label="Marca"
							required>
						</v-select>
					</v-col>
				</v-row>

				<v-row>
					<v-col>
						<v-text-field v-model="model.name" label="Nome" required :counter="150"></v-text-field>
					</v-col>
				</v-row>

				<v-row>
					<v-col>
						<v-radio-group inline label="Tipo da cor" v-model="model.colorType">
							<v-radio
								v-for="color in colorTypes" 
								:key="color.value"
								:label="color.text"
								:value="color.value" />
						</v-radio-group>
					</v-col>
				</v-row>

				<v-row v-if="model.colorType === colorTypes.Solid.value">
					<v-col>
						<v-color-picker
							v-model="model.hexCode"
							show-swatches
							mode="hexa"
							:modes="['hexa']"></v-color-picker>
					</v-col>
				</v-row>
				<template v-else-if="model.colorType === colorTypes.Pattern.value">
					<v-row>
						<v-col>
							<v-img
								:src="model.image?.fileSrc"
								alt="Textura"
								width="200"
								height="200"
								:class="{'bg-grey-lighten-2': !model.image?.fileSrc}"></v-img>
						</v-col>
					</v-row>
					<v-row>
						<v-col sm="6" md="5" lg="4">
							<v-file-input
								v-model="file"
								label="Textura"
								accept="image/*"
								@change="handleFileChange"
								prepend-icon="mdi-format-color-fill"></v-file-input>
						</v-col>
					</v-row>
				</template>

				<m-record-details v-bind="model" />

				<m-form-footer-buttons
					@submit="submitForm"
					@cancel="cancelForm"
					@delete="deleteForm"
					:show-delete="!formNewMode" />
			</v-responsive>
		</v-container>
	</v-form>
</template>
