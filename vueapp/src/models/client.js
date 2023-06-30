import CrudModel from './crudModel.js';
import PersonalDetail from './personalDetail.js';

import { reactive } from 'vue'

class Budget extends CrudModel {
    constructor(json = {}) {
        super(json);
        this.managerId = json.managerId || null;
        this.personalDataId = json.personalDataId || null;
        this.personalDetail = new PersonalDetail(json.personalDetail);
        this.name = json.name || null;
    }

    hydrate(data)
    {
        super.hydrate(data)

        if(!this.personalDetail)
            reactive(this).personalDetail = reactive(new PersonalDetail({ id: this.personalDataId }));
    }

    toFormData() {
        const formData = super.toFormData();
        formData.append('managerId', this.getSafeKeyValue(this.managerId));
        formData.append('personalDataId', this.getSafeKeyValue(this.personalDataId));
        return formData;
    }

    toJSON()
    {
        var data = super.toJSON()

        return {
            ...data,
            managerId: this.managerId,
            personalDataId: this.personalDataId,
            personalDetail: this.personalDetail?.toJSON(),
        }
    }
}

export default Budget;