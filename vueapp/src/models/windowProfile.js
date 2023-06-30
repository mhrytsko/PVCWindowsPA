import CrudModel from './crudModel.js';
import Brand from './brand.js';
import Image from './image.js';
import WindowProfileColor from './windowProfileColor.js';

import { reactive } from 'vue'

import { isEmpty } from 'lodash-es';

class WindowProfile extends CrudModel {
	constructor(json = {}) {
		super(json);
		this.name = json.name || null;
		this.description = json.description || null;
		this.brandId = json.brandId || null;
		this.brand = json.brand ? new Brand(json.brand) : null;
		this.imageId = json.imageId || null;
		this.image = json.image ? new Image(json.image) : (!isEmpty(this.imageId) ? new Image({ id: this.imageId }) : null);
		this.thermalInsulation = json.thermalInsulation || 0.0;
		this.soundInsulation = json.soundInsulation || 0;
		this.antiTheftResistance = json.antiTheftResistance || null;
		this.constructionDepth = json.constructionDepth || 0;
		this.frameChamberCount = json.frameChamberCount || 0;
		this.airPermeability = json.airPermeability || null;
		this.waterTightness = json.waterTightness || null;
		this.windResistance = json.windResistance || null;
		this.maxGlassThickness = json.maxGlassThickness || 0.0;
		this.colors = json.colors ? json.colors.map((color) => new WindowProfileColor(color)) : null;

		this.sideHungOpening = json.sideHungOpening || false;
		this.tiltAndTurnOpening = json.tiltAndTurnOpening || false;
		this.tiltOnlyOpening = json.tiltOnlyOpening || false;
		this.tiltAndParallelOpening = json.tiltAndParallelOpening || false;

		this.maxLeafSizeWidth = json.maxLeafSizeWidth || 0;
		this.maxLeafSizeHeight = json.maxLeafSizeHeight || 0;
	}

	hydrate(data)
    {
        super.hydrate(data)

        if(!this.image && !isEmpty(this.imageId))
            reactive(this).image = reactive(new Image({ id: this.imageId }))
    }

	toFormData() {
		const formData = super.toFormData();

		formData.append('name', this.getSafeStringValue(this.name));
		formData.append('description', this.getSafeStringValue(this.description));
		formData.append('brandId', this.getSafeKeyValue(this.brandId));
		formData.append('imageId', this.getSafeKeyValue(this.imageId));
        if(this.image?.isDirty === true) // Só se houver alteração é que vamos submeter a nova imagem
            this.image?.toOtherFormData(formData, 'image');
		formData.append('thermalInsulation', this.thermalInsulation);
		formData.append('soundInsulation', this.soundInsulation);
		formData.append('antiTheftResistance', this.getSafeStringValue(this.antiTheftResistance));
		formData.append('constructionDepth', this.constructionDepth);
		formData.append('frameChamberCount', this.frameChamberCount);
		formData.append('airPermeability', this.getSafeStringValue(this.airPermeability));
		formData.append('waterTightness', this.getSafeStringValue(this.waterTightness));
		formData.append('windResistance', this.getSafeStringValue(this.windResistance));
		formData.append('maxGlassThickness', this.maxGlassThickness);

		formData.append('sideHungOpening', this.sideHungOpening);
		formData.append('tiltAndTurnOpening', this.tiltAndTurnOpening);
		formData.append('tiltOnlyOpening', this.tiltOnlyOpening);
		formData.append('tiltAndParallelOpening', this.tiltAndParallelOpening);

		formData.append('maxLeafSizeWidth', this.maxLeafSizeWidth);
		formData.append('maxLeafSizeHeight', this.maxLeafSizeHeight);

		return formData;
	}

	toSrvData()
    {
        return this.toFormData()
    }

	setImage({file, fileName, fileType})
    {
        this.image = new Image({
            id: this.imageId,
            file,
            fileName,
            fileType,
            isDirty: true
        })
    }
}

export default WindowProfile;
