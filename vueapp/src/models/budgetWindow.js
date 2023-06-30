import ModelBase from "./modelBase";

export default class BudgetWindow extends ModelBase {
    constructor(json = {}) {
        super(json);

        this.budgetId = json.budgetId || null;
        this.windowId = json.windowId || null;
    }

    toFormData() {
		const formData = super.toFormData();

		formData.append('budgetId', this.getSafeKeyValue(this.budgetId));
        formData.append('windowId', this.getSafeKeyValue(this.windowId));

		return formData;
	}

    toJSON() {
        return {
            budgetId: this.getSafeKeyValue(this.budgetId),
            windowId: this.getSafeKeyValue(this.windowId)
        }
    }
}