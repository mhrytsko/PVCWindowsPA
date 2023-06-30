import CrudModel from './crudModel.js';
import Brand from './brand.js';
import Image from './image.js';

class WindowGlassType extends CrudModel {
    constructor(json = {}) {
        super(json);
        this.name = json.name || '';
        this.description = json.description || null;
        this.brandId = json.brandId || null;
        this.brand = json.brand ? new Brand(json.brand) : null;
        this.imageId = json.imageId || null;
        this.image = json.image ? new Image(json.image) : null;
        this.thickness = json.thickness || 0.0;
        this.chamberCount = json.chamberCount || 0;
        this.thermalInsulation = json.thermalInsulation || 0.0;
        this.soundInsulation = json.soundInsulation || 0;
        this.antiTheftResistance = json.antiTheftResistance || null;
        this.frosted = json.frosted || false;
    }

	toFormData() {
		const formData = super.toFormData();

		formData.append('name', this.name);
		formData.append('description', this.description);
		formData.append('brandId', this.brandId);
		formData.append('imageId', this.imageId);
		formData.append('thickness', this.thickness);
		formData.append('chamberCount', this.chamberCount);
		formData.append('thermalInsulation', this.thermalInsulation);
		formData.append('soundInsulation', this.soundInsulation);
		formData.append('antiTheftResistance', this.antiTheftResistance);
		formData.append('frosted', this.frosted);

		return formData;
	}

    toSrvData()
    {
        return this.toFormData()
    }
}

export default WindowGlassType;