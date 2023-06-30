import CrudModel from './crudModel.js';
import WindowColor from './windowColor.js';
import WindowProfile from './windowProfile.js';
import LeafConfiguration from './leafConfiguration.js';

class Window extends CrudModel {
    constructor(json = {}) {
        super(json);
        this.description = json.description || null;
        this.indorColorId = json.indorColorId || null;
        this.outdorColorId = json.outdorColorId || null;
        this.windowProfileId = json.windowProfileId || null;
        this.width = json.width || 0;
        this.height = json.height || 0;
        this.indorColor = json.indorColor ? new WindowColor(json.indorColor) : null;
        this.outdorColor = json.outdorColor ? new WindowColor(json.outdorColor) : null;
        this.windowProfile = json.windowProfile ? new WindowProfile(json.windowProfile) : null;
        this.leafConfigurations = (json.leafConfigurations || []).map(leafConfig => new LeafConfiguration(leafConfig));
        this.leafConfigurationsMap = (json.leafConfigurationsMap || []).map(leafConfig => new LeafConfiguration(leafConfig));
    }

	toFormData() {
		const formData = super.toFormData();

		formData.append('description', this.description);
		formData.append('indorColorId', this.indorColorId);
		formData.append('outdorColorId', this.outdorColorId);
		formData.append('windowProfileId', this.windowProfileId);
        formData.append('width', this.width);
        formData.append('height', this.height);

        (this.leafConfigurations || []).map((leafConfig, i) => leafConfig?.toOtherFormData(formData, `leafConfigurations[${i}]`));

		return formData;
	}

    toJSON() {
		const data = super.toJSON();

		return {
            ...data,
            description: this.description,
            indorColorId: this.indorColorId,
            outdorColorId: this.outdorColorId,
            windowProfileId: this.windowProfileId,
            width: this.width,
            height: this.height,
            leafConfigurations: (this.leafConfigurations || []).map((leafConfig) => leafConfig?.toJSON())
        };
	}

    toSrvData()
    {
        return this.toJSON()
    }
}

export default Window;