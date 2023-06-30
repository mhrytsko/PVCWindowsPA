<script>
	import netApi from '@/api/network/index.js';
	import {get} from 'lodash-es';
	import Swal from 'sweetalert2';

	export default {
		data() {
			return {
				file: undefined,
                flattenHierarchy: true
			};
		},

		methods: {
			handleFileChange(event) {
				var file = get(event, 'target.files[0]', null);
				if (file instanceof File) {
					var formData = new FormData();
					formData.append('file', file, file.name);
					netApi.post('Tools', 'RestoreModelsData', formData, () => {
						Swal.fire({
							icon: 'success',
							title: 'Restore - OK',
							showConfirmButton: false,
							timer: 1500,
						});
					});
				}
			},

            downloadSchema()
            {
                netApi.get("Tools", "ModelsSchemaAsFile", {
                    flattenHierarchy: this.flattenHierarchy
                }, (_, response) => {

                    const blob = new Blob([response.data], { type: 'application/zip' }); // Cria um Blob a partir dos dados da resposta

                    const downloadLink = document.createElement('a');
                    downloadLink.href = URL.createObjectURL(blob); // Cria uma URL temporária para o Blob
                    downloadLink.download = 'modelsSchema.zip'; // Nome do arquivo ZIP que será baixado
                    downloadLink.click();
                }, undefined, {
                    responseType: 'blob'
                })
            },

            downloadData()
            {
                netApi.get("Tools", "ModelsData", undefined, (_, response) => {

                    const blob = new Blob([response.data], { type: 'application/zip' }); // Cria um Blob a partir dos dados da resposta

                    const downloadLink = document.createElement('a');
                    downloadLink.href = URL.createObjectURL(blob); // Cria uma URL temporária para o Blob
                    downloadLink.download = 'modelsData.zip'; // Nome do arquivo ZIP que será baixado
                    downloadLink.click();
                }, undefined, {
                    responseType: 'blob'
                })
            }
		},
	};
</script>

<template>
	<v-container>
		<v-responsive class="align-left text-left fill-height">
			<v-row>
				<v-col cols="3">
					<v-btn size="x-large" @click="downloadSchema">Download Model Schema</v-btn>
				</v-col>
				<v-col cols="2">
					<v-checkbox v-model="flattenHierarchy" label="Flatten Hierarchy"></v-checkbox>
				</v-col>
			</v-row>

            <v-row>
				<v-col cols="3">
					<v-btn size="x-large" @click="downloadData">Download Models Data</v-btn>
				</v-col>
			</v-row>

            <br >
            <br >
            <br >

			<v-row>
				<v-col cols="3">
					<v-file-input
						show-size
						label="Upload Models Data"
						accept=".zip,.rar,.7zip"
						@change="handleFileChange" />
				</v-col>
			</v-row>
		</v-responsive>
	</v-container>
</template>
