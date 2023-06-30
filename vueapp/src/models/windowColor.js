import CrudModel from './crudModel.js';
import Brand from './brand.js';
import Image from './image.js';

import { ColorType } from '@/mixins/systemEnums.js'

import { reactive } from 'vue'
import { isEmpty } from 'lodash-es';

class WindowColor extends CrudModel {
    constructor(json = {}) {
        super(json);
        this.name = json.name || '';
        this.hexCode = json.hexCode || null;
        this.brandId = json.brandId || null;
        this.brand = json.brand ? new Brand(json.brand) : null;
        this.imageId = json.imageId || null;
        this.image = json.image ? new Image(json.image) : (!isEmpty(this.imageId) ? new Image({ id: this.imageId }) : null);

        this.colorType = json.colorType !== undefined ? json.colorType : ColorType.Solid.value;
    }

    hydrate(data)
    {
        super.hydrate(data)

        if(!this.image && !isEmpty(this.imageId))
            reactive(this).image = reactive(new Image({ id: this.imageId }))
    }

	toFormData() {
		const formData = super.toFormData();

		formData.append('name', this.getSafeStringValue(this.name));
		formData.append('brandId', this.getSafeKeyValue(this.brandId));
		formData.append('colorType', this.colorType);

        if(this.colorType === ColorType.Solid.value)
            formData.append('hexCode', this.getSafeStringValue(this.hexCode));
        else if(this.colorType === ColorType.Pattern.value)
        {
            formData.append('imageId', this.getSafeKeyValue(this.imageId));
            if(this.image?.isDirty === true) // Só se houver alteração é que vamos submeter a nova imagem
                this.image?.toOtherFormData(formData, 'image');
        }

		return formData;
	}

    toSrvData()
    {
        return this.toFormData()
    }

    setImage({file, fileName, fileType})
    {
        this.image = new Image({
            id: this.imageId,
            file,
            fileName,
            fileType,
            isDirty: true
        })
    }
}

export default WindowColor;