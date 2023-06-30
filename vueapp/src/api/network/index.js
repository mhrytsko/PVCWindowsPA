import axios from 'axios';

import _isEmpty from 'lodash-es/isEmpty';
import _set from 'lodash-es/set';
import _get from 'lodash-es/get';

import printJS from 'print-js';

import auth from '@/api/auth.js';
import eventBus from '@/api/eventBus.js';

const baseApiUrl = `${import.meta.env.BASE_URL}api`;

// // Interceptor de solicitação
// axios.interceptors.request.use((config) => {
// 	// Verifique se os dados são do tipo FormData
// 	if (config.data instanceof FormData) {
// 		// Definir o cabeçalho Content-Type como multipart/form-data
// 		config.headers['Content-Type'] = 'multipart/form-data';
// 	}
// 	return config;
// });

function getAxiosConfig(options) {
	let token = auth.getAuthToken(),
		headers = {
			'Cache-Control': 'no-cache',
		};

	if (!_isEmpty(token)) _set(headers, 'Authorization', `Bearer ${token}`);

	return {
		headers,
		...options,
	};
}

export function apiUrl(controller, action, params) {
	let url = `${baseApiUrl}/${controller}`;

	if (!_isEmpty(action)) url = `${url}/${action}`;

	if (!_isEmpty(params)) url = `${url}/${params}`;

	return url;
}

function onError(error) {
	/**
	 * 401 - Unauthorized
	 * 400 - Bad request / Validation errors
	 * 404 - Not found
	 * 500 - Server Error
	 */

	if (error.response.status === 401) {
		const authHeader = error.response.headers['www-authenticate'];

		if (authHeader) {
			const errorType = _get(authHeader.match(/error="(.*?)"/), 1);
			const errorDescription = _get(authHeader.match(/error_description="(.*?)"/), 1);
			if (errorType === 'invalid_token') {
				// o token inválido e/ou expirado,
				// TODO: adicionar o redirecionar para a página de login
				console.error(errorDescription);

				if (!_isEmpty(errorDescription))
					eventBus.emit('show-generic-error', {
						message: errorDescription,
					});

				auth.logOut();
				eventBus.emit('redirect-to-route', {name: 'home'});
			}
		}
	}

	const data = error.response.data;
	if (data) {
		console.error(data);

		if (!_isEmpty(data?.message))
			eventBus.emit('show-generic-error', {
				message: data?.message,
			});
	}

	return Promise.reject(error);
}

async function get(controller, action, params, fnCallback, fnErrorCallback, options) {
	let url = apiUrl(controller, action);

	return axios
		.get(
			url,
			getAxiosConfig({
				params,
				...options,
			})
		)
		.then(
			(response) => {
				if (fnCallback) fnCallback(response.data, response);
				return response;
			},
			(error) => {
				onError(error);
				if (fnErrorCallback) fnErrorCallback(error);
			}
		);
}

function post(controller, action, data, fnCallback, fnErrorCallback, options) {
	let url = apiUrl(controller, action);

	return axios.post(url, data, getAxiosConfig(options)).then(
		(response) => {
			if (fnCallback) fnCallback(response.data, response);
		},
		(error) => {
			onError(error);
			if (fnErrorCallback) fnErrorCallback(error);
		}
	);
}

function getImage(controller, action, params, fnCallback, fnErrorCallback) {
	let url = apiUrl(controller, action, params);

	return axios
		.get(
			url,
			getAxiosConfig({
				responseType: 'blob',
			})
		)
		.then(
			(response) => {
				const reader = new FileReader();
				reader.onload = () => {
					if (fnCallback) fnCallback(reader.result, response);
				};
				reader.readAsDataURL(response.data);
			},
			(error) => {
				//onError(error);
				if (fnErrorCallback) fnErrorCallback(error);
			}
		);
}

const DOWNLOAD_TYPE = {
	IMAGE: 'image-new-tab',
	PRINT: 'print-pdf',
	DOWNLOAD: 'download',
}

function downloadData(controller, action, data, fnCallback, fnErrorCallback, options, openType = 'download') {
	let url = apiUrl(controller, action);

	return axios
		.post(
			url,
			data,
			getAxiosConfig({
				responseType: 'blob',
				...options
			})
		)
		.then(
			(response) => {
				const blob = new Blob([response.data]); // Cria um Blob a partir dos dados da resposta

				// Obter o cabeçalho Content-Disposition
				const contentDisposition = response.headers['content-disposition'];

				// Extrai o nome do arquivo do cabeçalho Content-Disposition
				let filename = '';
				if (contentDisposition) {
					const filenameRegex = /filename\*=UTF-8''([^;]*)/;
					const matches = filenameRegex.exec(contentDisposition);
					if (matches != null && matches[1]) {
						filename = decodeURIComponent(matches[1]); // Decodifica o nome do arquivo
					} else {
						const filenameRegex2 = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
						const matches2 = filenameRegex2.exec(contentDisposition);
						if (matches2 != null && matches2[1]) {
							filename = matches2[1].replace(/['"]/g, '');
						}
					}
				}

				if (openType === 'image-new-tab') // Imege in the new tab
				{
					// Abre a imagem em uma nova janela ou guia
					const imageBlob = new Blob([response.data], {type: 'image/png'});
					const objectUrl = URL.createObjectURL(imageBlob);
					window.open(objectUrl, '_blank');
					/*const reader = new FileReader();
					reader.onloadend = () => {
						const newWindow = window.open('', '_blank');
						if (newWindow) {
							newWindow.document.write(
								'<html><head><title>Image</title></head><body><img src="' + reader.result + '" /></body></html>'
							);
							newWindow.document.close();
						}
					};
					reader.readAsDataURL(imageBlob);*/
				} 
				else if(openType === 'print-pdf') // Print PDF
				{
					const urlBlob = URL.createObjectURL(blob); // Cria uma URL temporária para o Blob
					printJS({printable:urlBlob, type:'pdf', showModal:true})
				} 
				else // Download
				{ 
					const urlBlob = URL.createObjectURL(blob); // Cria uma URL temporária para o Blob
					const downloadLink = document.createElement('a');
					downloadLink.href = urlBlob // Usar um URL o para o Blob
					downloadLink.download = !_isEmpty(filename) ? filename : 'file'; // Nome do ficheiro que será baixado
					document.body.appendChild(downloadLink);
					downloadLink.click();
				}

				if (fnCallback) fnCallback(response);
			},
			(error) => {
				//onError(error);
				if (fnErrorCallback) fnErrorCallback(error);
			}
		);
}

export default {
	apiUrl,
	get,
	post,
	getImage,
	downloadData,
	downloadType: DOWNLOAD_TYPE
};
