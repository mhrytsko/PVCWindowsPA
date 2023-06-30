import CrudModel from './crudModel.js';
import BudgetWindow from './budgetWindow.js';

import {unref} from 'vue'

class Budget extends CrudModel {
    constructor(json = {}) {
        super(json);
        this.userId = json.userId || null;
        this.budgetNumber = json.budgetNumber || null;
        this.budgetWindows = json.budgetWindows ? json.budgetWindows.map(bw => new BudgetWindow(bw)) : null;
        this.clientId = json.clientId || null;
    }

    toFormData() {
        const formData = super.toFormData();
        formData.append('userId', this.getSafeKeyValue(this.userId));
        formData.append('budgetNumber', this.budgetNumber);
        formData.append('budgetWindows', JSON.stringify(this.budgetWindows || []));
        formData.append('clientId', this.getSafeKeyValue(this.clientId));
        return formData;
    }

    toJSON()
    {
        var data = super.toJSON()

        return {
            ...data,
            userId: this.userId,
            budgetNumber: this.budgetNumber,
            budgetWindows: unref(this.budgetWindows),
            clientId: this.clientId
        }
    }
}

export default Budget;