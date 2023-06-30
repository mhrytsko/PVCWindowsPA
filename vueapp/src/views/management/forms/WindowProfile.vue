<script>
	import WindowProfile from '@/models/windowProfile.js';
	import formHandlers from '@/mixins/formHandlers.js';

	import {handleFileChange} from '@/mixins/imageTools';
	import netApi from '@/api/network';

	export default {
		name: 'WindowProfileForm',

		mixins: [formHandlers],

		data() {
			return {
				formInfo: {
					controller: 'WindowProfiles',
					goBack: {name: 'management-window-profiles'},
					modelType: WindowProfile,
				},
				model: new WindowProfile(),
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
		<v-container align-items="left" fluid>
			<v-responsive class="align-center text-center fill-height">
				<v-row class="d-flex align-left justify-left">
					<v-col cols="auto">
						<div class="text-h4 font-weight-bold">Perfil da janela PVC</div>
					</v-col>
				</v-row>

				<m-form-errors :errors="formErrors" />

				<v-row>
					<v-col cols="12">
						<v-text-field id="profile-name" v-model.trim="model.name" label="Nome" required></v-text-field>
					</v-col>
				</v-row>
				<v-row>
					<v-col cols="12">
						<v-textarea v-model="model.description" label="Descrição" :maxlength="10000"></v-textarea>
					</v-col>
				</v-row>
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
						<v-img
							:src="model.image?.fileSrc"
							alt="Foto do profil"
							width="350"
							height="350"
							:class="{'bg-grey-lighten-2': !model.image?.fileSrc}"></v-img>
					</v-col>
				</v-row>
				<v-row>
					<v-col sm="6" md="5" lg="4">
						<v-file-input
							v-model="file"
							label="Foto do profil"
							accept="image/*"
							@change="handleFileChange"
							prepend-icon="mdi-camera"></v-file-input>
					</v-col>
				</v-row>
				<v-row>
					<v-col cols="6" sm="4" md="2" lg="2">
						<m-numeric
							v-model="model.thermalInsulation"
							:is-decimal="true"
							:max="9.99"
							label="Isolamento térmico"
							prefix="Uw até"
							suffix="W/m²K" />
					</v-col>
					<v-col cols="6" sm="4" md="2" lg="2">
						<m-numeric
							v-model="model.soundInsulation"
							:max="99"
							label="Isolamento acústico"
							prefix="Rw até"
							suffix="dB" />
					</v-col>
				</v-row>
				<v-row>
					<v-col cols="6" sm="4" md="2" lg="2">
						<m-numeric
							v-model.number="model.frameChamberCount"
							:max="5"
							label="Frame Chamber Count"
							type="number" />
					</v-col>
					<v-col cols="6" sm="4" md="2" lg="2">
						<m-numeric
							v-model.number="model.constructionDepth"
							:max="999"
							label="Construction Depth"
							suffix="mm"/>
					</v-col>
				</v-row>

				<v-row>
					<v-col cols="9" sm="5" md="4" lg="3">
						<m-numeric
							v-model.number="model.maxGlassThickness"
							:max="999"
							label="Espessura máxima do vidro suportada"
							suffix="mm" />
					</v-col>
				</v-row>

				<v-row>
					<v-col cols="6" sm="5" md="4" lg="3">
						<m-numeric
							v-model.number="model.maxLeafSizeWidth"
							:max="5000"
							label="Largura máx. folha"
							suffix="mm" />
					</v-col>
					<v-col cols="6" sm="5" md="4" lg="3">
						<m-numeric
							v-model.number="model.maxLeafSizeHeight"
							:max="5000"
							label="Altura máx. folha"
							suffix="mm" />
					</v-col>
				</v-row>


				<v-row>
					<v-col cols="12" sm="5" md="4" lg="3">
						<v-text-field v-model="model.airPermeability" label="Permeabilidade ao ar"></v-text-field>
					</v-col>
					<v-col cols="12" sm="5" md="4" lg="3">
						<v-text-field v-model="model.waterTightness" label="Estanquidade à água"></v-text-field>
					</v-col>
				</v-row>
				<v-row>
					<v-col cols="12" sm="5" md="4" lg="3">
						<v-text-field v-model="model.windResistance" label="Resistência ao vento"></v-text-field>
					</v-col>
					<v-col cols="12" sm="5" md="4" lg="3">
						<v-text-field v-model="model.antiTheftResistance" label="Resistência antirroubo"></v-text-field>
					</v-col>
				</v-row>

				<v-row>
					<v-col cols="12">
						<v-text-field v-model="model.colors" label="Colors" multiple></v-text-field>
					</v-col>
				</v-row>

				<v-row class="d-flex align-left justify-left">
					<v-col cols="auto">
						<div class="text-h5 font-weight-bold">Tipos de abertura suportados</div>
					</v-col>
				</v-row>
				<v-row>
					<v-col cols="12" sm="6" md="3">
						<v-list>
							<v-list-item>
								<v-switch
									v-model="model.sideHungOpening"
									label="Batente"
									color="primary"
									density="compact"></v-switch>
							</v-list-item>
							<v-list-item>
								<v-switch
									v-model="model.tiltAndTurnOpening"
									label="Oscilobatente"
									color="primary"
									density="compact"></v-switch>
							</v-list-item>
							<v-list-item>
								<v-switch
									v-model="model.tiltOnlyOpening"
									label="Basculante"
									color="primary"
									density="compact"></v-switch>
							</v-list-item>
							<v-list-item>
								<v-switch
									v-model="model.tiltAndParallelOpening"
									label="Oscilo-paralelo"
									color="primary"
									density="compact"></v-switch>
							</v-list-item>
						</v-list>
					</v-col>
				</v-row>

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
