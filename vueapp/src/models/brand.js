import CrudModel from './crudModel.js';
import Image from './image.js';

import { reactive } from 'vue'

import { isEmpty } from 'lodash-es';

class Brand extends CrudModel {
    constructor(json = {}) {
        super(json);
        this.name = json.name || null;
        this.description = json.description || null;
        this.site = json.site || null;
        this.imageId = json.imageId || null;
        this.image = json.image ? new Image(json.image) : (!isEmpty(this.imageId) ? new Image({ id: this.imageId }) : null);
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
        formData.append('description', this.getSafeStringValue(this.description));

        formData.append('site', this.getSafeStringValue(this.site));

        formData.append('imageId', this.getSafeKeyValue(this.imageId));
        if(this.image?.isDirty === true) // Só se houver alteração é que vamos submeter a nova imagem
            this.image?.toOtherFormData(formData, 'image');

        return formData;
    }

    toJSON()
    {
        var data = super.toJSON()

        return {
            ...data,
            name: this.name,
            description: this.description,
            site: this.site,
            imageId: this.imageId
        }
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

export default Brand;