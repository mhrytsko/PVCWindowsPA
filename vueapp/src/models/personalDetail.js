import CrudModel from './crudModel.js';

class PersonalDetail extends CrudModel {
    constructor(json = {}) {
        super(json);
        this.firstName = json.firstName || null;
        this.lastName = json.lastName || null;
        this.email = json.email || null;
        this.phone = json.phone || null;
    }

    toFormData() {
        const formData = super.toFormData();
        formData.append('firstName', this.firstName);
        formData.append('lastName', this.lastName);
        formData.append('email', this.email);
        formData.append('phone', this.phone);
        return formData;
    }

    toJSON()
    {
        var data = super.toJSON()

        return {
            ...data,
            firstName: this.firstName,
            lastName: this.lastName,
            email: this.email,
            phone: this.phone,
        }
    }
}

export default PersonalDetail;