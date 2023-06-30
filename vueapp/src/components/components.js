import { defineAsyncComponent } from 'vue'

export const DataTable = defineAsyncComponent(() => import('@/components/DataTable.vue'));
export const Simple2DWindow = defineAsyncComponent(() => import('@/components/window/Simple2DWindow.vue'));
export const Simple3DWindow = defineAsyncComponent(() => import('@/components/window/Simple3DWindow.vue'));
export const FormErrors = defineAsyncComponent(() => import('@/components/FormErrors.vue'));
export const FormFooterButtons = defineAsyncComponent(() => import('@/components/FormFooterButtons.vue'));
export const RecordDetails = defineAsyncComponent(() => import('@/components/RecordDetails.vue'));

export const NumericInput = defineAsyncComponent(() => import('@/components/inputs/NumericInput.vue'));


export default
{
    DataTable,
    Simple2DWindow,
    Simple3DWindow,
    FormErrors,
    FormFooterButtons,
    RecordDetails,
    NumericInput
}