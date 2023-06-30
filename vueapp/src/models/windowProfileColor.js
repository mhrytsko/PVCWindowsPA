import ModelBase from './modelBase.js';
import WindowColor from './windowColor.js';
//import WindowProfile from './windowProfile.js';

class WindowProfileColor extends ModelBase {
	constructor(json = {}) {
		super(json);
		this.colorId = json.colorId || null;
		this.profileId = json.profileId || null;
		this.color = json.color ? new WindowColor(json.color) : null;
		this.profile = json.profile || null;
	}

	toFormData() {
		const formData = super.toFormData();

		formData.append('colorId', this.colorId);
		formData.append('profileId', this.profileId);

		return formData;
	}

	toJSON() {
		const data = super.toJSON();

		return {
            ...data,
            colorId: this.colorId,
            profileId: this.profileId,
        };
	}
}

export default WindowProfileColor;
