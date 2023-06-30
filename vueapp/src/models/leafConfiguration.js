import CrudModel from './crudModel.js';

class LeafConfiguration extends CrudModel {
    constructor(json = {}) {
        super(json);
        this.windowId = json.windowId || null;
        this.windowGlassTypeId = json.windowGlassTypeId || null;
        this.openingSystem = json.openingSystem || 0;
        this.openingDirection = json.openingDirection || 0;
        this.hasHandle = json.hasHandle || false;
        this.width = json.width || 0;
        this.height = json.height || 0;
        this.x = json.x || 0;
        this.y = json.y || 0;
        this.windowGlassType = json.windowGlassType || null;
        this.window = json.window || null;
        this.frosted = json.frosted || false
    }

    toFormData() {
        const formData = super.toFormData();
        formData.append('windowId', this.windowId);
        formData.append('windowGlassTypeId', this.windowGlassTypeId);
        formData.append('openingSystem', this.openingSystem);
        formData.append('openingDirection', this.openingDirection);
        formData.append('hasHandle', this.hasHandle);
        formData.append('width', this.width);
        formData.append('height', this.height);
        formData.append('x', this.x);
        formData.append('y', this.y);
        formData.append('frosted', this.frosted);
        return formData;
    }

    toJSON()
    {
        var data = super.toJSON()

        return {
            ...data,
            windowId: this.windowId,
            windowGlassTypeId: this.windowGlassTypeId,
            openingSystem: this.openingSystem,
            openingDirection: this.openingDirection,
            hasHandle: this.hasHandle,
            width: this.width,
            height: this.height,
            x: this.x,
            y: this.y,
            frosted: this.frosted
        }
    }
}

export default LeafConfiguration;