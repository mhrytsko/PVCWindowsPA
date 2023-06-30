import ModelBase from './modelBase.js';

import { isEmpty } from 'lodash-es';

class CrudModel extends ModelBase {
    constructor(json = {}) {
        super(json);
        this.id = json.id || null;
        this.state = json.state || 0;
        this.creationDate = json.creationDate || null;
        this.modificationDate = json.modificationDate || null;
    }

    toFormData() {
        const formData = super.toFormData();
        formData.append('id', this.getSafeKeyValue(this.id));
        formData.append('state', this.state);
        if(!isEmpty(this.creationDate))
            formData.append('creationDate', this.creationDate);
        if(!isEmpty(this.modificationDate))
            formData.append('modificationDate', this.modificationDate);
        return formData;
    }

    toJSON()
    {
        var data = super.toJSON();

        return {
            ...data,
            id: this.getSafeKeyValue(this.id),
            state: this.state,
            creationDate: this.creationDate,
            modificationDate: this.modificationDate
        }
    }

    toBlobFormData()
    {
        return new Blob([this.toFormData()], { type: 'multipart/form-data' })
    }

    toOtherFormData(mainFormData, prefix)
    {
        const formData = this.toFormData();
        for (const pair of formData.entries()) {
            mainFormData.append(`${prefix}.${pair[0]}`, pair[1]);
        }
    }
}

export default CrudModel;