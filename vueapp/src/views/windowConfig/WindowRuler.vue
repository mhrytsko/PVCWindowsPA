<script>
	import {reactive} from 'vue'
	import Swal from 'sweetalert2';

	import netApi from '@/api/network/index.js'
	import {handleFileChange, displayImageFromByteArray} from '@/mixins/imageTools';


	import {get} from 'lodash-es';

	export default {
		emits: ['update:modelValue', 'update:show'],

		props: {
			modelValue: {
				//required: true,
			},

			show: {
				type: Boolean,
				required: true,
				default: false,
			},
		},

		data()
		{
			return {
				file: null,
				fileType: undefined,
				fileName: undefined,
				fileSrc: undefined,

				loadingMeasure: false,
			}
		},

		methods: {
			/*onSelect(value) {
				const selectedValue = get(value, '0', this.modelValue);
				this.$emit('update:modelValue', selectedValue);
			},*/

			onClose() {
				this.$emit('update:show', false);
			},


			handleFileChange(event) {
				handleFileChange(event, (result) => {
					const {file, fileName, fileType} = result;

					this.file = file;
					this.fileType = fileType;
					this.fileName = fileName;

					if(this.file instanceof File)
						displayImageFromByteArray(file, (result) => { reactive(this).fileSrc = result });
					else
						this.fileSrc = undefined;
				});
			},

			tryMeasure()
			{
				if(this.file instanceof File)
				{
					const formData = new FormData();
					formData.append('file', this.file, this.fileName);
					
					this.loadingMeasure = true;
					netApi.post('Tools', 'DetectWindow', formData, 
						data => {
							console.log("DetectWindow - OK", data);
							this.loadingMeasure = false;

							if(data.success === true)
							{
								return Swal.fire({
									title: 'O tamanho aproximado da janela',
									text: `Largura: ${get(data, 'windowWidth', 0)} cm e Altura: ${get(data, 'windowHeight', 0)} cm`,
									icon: 'success',
								});
							}
							else
							{
								return Swal.fire({
									title: 'Falha no processamento',
									text: 'Não foi possível calcular o tamanho da janela',
									icon: 'error',
								});
							}
						},
						error => {
							console.error("DetectWindow - ERROR", error);
							this.loadingMeasure = false;

							return Swal.fire({
									title: 'Erro no processamento',
									text: 'Ocorreu um erro ao processar o seu pedido',
									icon: 'error',
								});
						});
				}
			}
		},

	};
</script>

<template>
	<v-dialog
		:modelValue="show"
		@update:modelValue="onClose"
		fullscreen
		:scrim="false"
		scrollable
		transition="dialog-bottom-transition">
		<v-card>
			<v-toolbar>
				<v-btn icon dark @click="onClose">
					<v-icon>mdi-close</v-icon>
				</v-btn>
			</v-toolbar>
			<v-card-title>
				<span class="text-h5">Medir janela</span>
			</v-card-title>
			<v-card-text>
				<v-row>
					<v-col>
						<v-sheet>
							<v-alert
								density="compact"
								type="warning"
								title="Funcionalidade experimental"
								text="Por favor, coloque uma moeda de 2€ perto da janela e tire uma foto. Será feita uma tentativa de estimar as dimensões aproximadas de sua janela." />
						</v-sheet>
					</v-col>
				</v-row>
				<v-row>
					<v-col sm="6" md="5" lg="4">
						<v-file-input
							v-model="file"
							label="Foto da janela"
							accept="image/*"
							@change="handleFileChange"
							prepend-icon="mdi-camera"></v-file-input>
					</v-col>
					<v-col cols="auto">
						<v-btn icon="mdi-ruler" @click="tryMeasure" :disabled="!file || loadingMeasure" :loading="loadingMeasure"/>
					</v-col>
				</v-row>
				<v-row>
					<v-col>
						<v-img
							:src="fileSrc"
							alt="Foto"
							width="300"
							height="300"
							:class="{'bg-grey-lighten-2': !fileSrc}"></v-img>
					</v-col>
				</v-row>
			</v-card-text>
		</v-card>
	</v-dialog>
</template>

<style>
	.swal2-container
	{
		z-index: 10000;
	}
</style>