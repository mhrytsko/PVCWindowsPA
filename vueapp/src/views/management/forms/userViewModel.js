import User from '@/models/user.js';

export default class UserViewModel extends User {
    constructor(json = {}) {
        super(json);

        this.password = json.password || '';
    }

    toFormData() {
		const formData = super.toFormData();

		formData.append('password', this.password);

		return formData;
	}

    toJSON() {
        const data = super.toJSON();

        return {
            ...data,
            password: this.password,
        }
    }
}