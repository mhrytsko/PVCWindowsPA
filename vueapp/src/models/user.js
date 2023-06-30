import IdentityUserOfGuid from './identityUserOfGuid.js';
import PersonalDetail from './personalDetail.js';
import Window from './window.js';
import Budget from './budget.js';

import {isEmpty} from 'lodash-es'

class User extends IdentityUserOfGuid {
    constructor(json = {}) {
        super(json);
        this.userName = json.userName || '';
        //this.passwordHash = json.passwordHash || null;
        this.role = json.role || 0;
        this.personalDataId = json.personalDataId || null;
        this.personalDetail = json.personalDetail ? new PersonalDetail(json.personalDetail) : new PersonalDetail();
        this.creationDate = json.creationDate || null;
        this.modificationDate = json.modificationDate || null;
        this.state = json.state || 0;
        this.windows = json.windows ? json.windows.map(window => new Window(window)) : null;
        this.budgets = json.budgets ? json.budgets.map(budget => new Budget(budget)) : null;
    }

    hydrate(data)
    {
        super.hydrate(data)

        if(!this.personalDetail)
            this.personalDetail = new PersonalDetail()
    }

	toFormData() {
		const formData = super.toFormData();

		formData.append('role', this.role);
		formData.append('personalDataId', this.personalDataId);
        this.personalDetail?.toOtherFormData(formData, 'personalDetail');
        if(!isEmpty(this.creationDate))
            formData.append('creationDate', this.creationDate);
        if(!isEmpty(this.modificationDate))
            formData.append('modificationDate', this.modificationDate);
        formData.append('state', this.state);

		return formData;
	}

    toJSON() {
		const data = super.toJSON();

		return {
            ...data,
            role: this.role,
            personalDataId: this.personalDataId,
            personalDetail: this.personalDetail?.toJSON(),
            creationDate: this.creationDate,
            modificationDate: this.modificationDate,
            state: this.state
        };
	}
}

export default User;