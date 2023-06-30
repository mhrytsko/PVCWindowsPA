import {get} from 'lodash-es';

export function convertByteArrayToBase64(byteArray) {
	const blob = new Blob([byteArray], {type: 'image/jpeg'});
	const reader = new FileReader();

	return new Promise((resolve, reject) => {
		reader.onloadend = () => resolve(reader.result);
		reader.onerror = reject;
		reader.readAsDataURL(blob);
	});
}

export function displayImageFromByteArray(byteArray, fnCallback) {
	if (byteArray) {
		return convertByteArrayToBase64(byteArray)
			.then((result) => {
				fnCallback(result);
				return result;
			})
			.catch((error) => {
				console.error('Erro ao converter byteArray para base64:', error);
				fnCallback(null);
			});
	} else {
		fnCallback(null);
	}
}

export function handleFileChange(event, fnCallback) {
	var file = get(event, 'target.files[0]', null);

	if (file) {
        fnCallback({
            file: file,
            fileName: file.name,
            fileType: file.type
        });
		/*const fileReader = new FileReader();

		fileReader.onload = (e) => {
			fnCallback({
				file: e.target.result, // Array de bytes do arquivo
				fileName: file.name, // Extraia o nome do arquivo do objeto File
                fileType: file.type
			});
		};

		fileReader.readAsArrayBuffer(file);*/
	} else {
		fnCallback({
			file: null,
			fileName: null,
		});
	}
}

export default {
	convertByteArrayToBase64,
	displayImageFromByteArray,
	handleFileChange,
};
