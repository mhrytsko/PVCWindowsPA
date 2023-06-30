import ModelBase from './modelBase.js';

class IdentityUserOfGuid extends ModelBase {
    constructor(json = {}) {
        super(json);
        this.id = json.id || null;
        this.userName = json.userName || null;
        this.normalizedUserName = json.normalizedUserName || null;
        this.email = json.email || null;
        this.normalizedEmail = json.normalizedEmail || null;
        this.emailConfirmed = json.emailConfirmed || false;
        //this.passwordHash = json.passwordHash || null;
        this.securityStamp = json.securityStamp || null;
        //this.concurrencyStamp = json.concurrencyStamp || null;
        this.phoneNumber = json.phoneNumber || null;
        this.phoneNumberConfirmed = json.phoneNumberConfirmed || false;
        this.twoFactorEnabled = json.twoFactorEnabled || false;
        this.lockoutEnd = json.lockoutEnd || null;
        this.lockoutEnabled = json.lockoutEnabled || false;
        this.accessFailedCount = json.accessFailedCount || 0;
        // Defina as propriedades adicionais específicas de IdentityUserOfGuid aqui
    }

    toFormData() {
        const formData = super.toFormData();
        formData.append('id', this.getSafeKeyValue(this.id));
        formData.append('userName', this.getSafeStringValue(this.userName));
        formData.append('email', this.getSafeStringValue(this.email));
        formData.append('emailConfirmed', this.emailConfirmed);
        formData.append('securityStamp', this.getSafeStringValue(this.securityStamp));
        formData.append('phoneNumber', this.phoneNumber);
        formData.append('phoneNumberConfirmed', this.phoneNumberConfirmed);
        formData.append('twoFactorEnabled', this.twoFactorEnabled);
        return formData;
    }

    toJSON() {
		const data = super.toJSON();

		return {
            ...data,
            id: this.getSafeKeyValue(this.id),
            userName: this.userName,
            email: this.email,
            emailConfirmed: this.emailConfirmed,
            securityStamp: this.securityStamp,
            phoneNumber: this.phoneNumber,
            phoneNumberConfirmed: this.phoneNumberConfirmed,
            twoFactorEnabled: this.twoFactorEnabled
        };
	}
}

export default IdentityUserOfGuid;