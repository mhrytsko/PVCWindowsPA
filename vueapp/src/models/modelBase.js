import { merge , isEmpty } from 'lodash-es';
import {v4 as uuidv4} from 'uuid';

class ModelBase 
{
    constructor()
    {
        this.uid = uuidv4()
    }

    hydrate(data)
    {
        merge(this, data);
    }

    getSafeKeyValue(id)
    {
        return isEmpty(id) ? null : id
    }

    getSafeStringValue(strValue)
    {
        return isEmpty(strValue) ? '' : strValue
    }

    toFormData() {
        const formData = new FormData();
        return formData;
    }

    toJSON()
    {
        return {}
    }

    toSrvData()
    {
        return this.toJSON()
    }
}

export default ModelBase;