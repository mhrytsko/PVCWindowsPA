import CrudModel from './crudModel.js';

import { reactive } from 'vue'

import { isEmpty } from 'lodash-es';

import {displayImageFromByteArray} from '@/mixins/imageTools.js';
import { apiUrl } from '@/api/network/index.js'

class Image extends CrudModel {
	constructor(json = {}) {
		super(json);

		this._file = json.file || null;
		this.fileName = json.fileName || null;
		this.fileType = json.fileType || null;

		this.fileSrc = '';
		this.isDirty = json.isDirty || false

		this.fetchImage()
	}

	get file() {
		return this._file;
	}

	set file(newValue) {
		this._file = newValue;
		displayImageFromByteArray(this._file, (result) => (this.fileSrc = result));
	}

	getBlob() {
		if (this._file) return new Blob([this._file]);
		else return null;
	}

	toFormData() {
		const formData = super.toFormData();
		formData.append('fileName', this.getSafeStringValue(this.fileName));
		formData.append('fileType', this.getSafeStringValue(this.fileName));
		if(this._file instanceof File)
			formData.append('fileData', this._file, this.fileName);
		return formData;
	}

	fetchImage()
	{
		if(this._file !== null)
			displayImageFromByteArray(this._file, (result) => { reactive(this).fileSrc = result });
		else if(!isEmpty(this.id))/** != isDirty ? */
			this.fileSrc = apiUrl('Images', 'image', `${this.id}`)
	}
}

export default Image;
