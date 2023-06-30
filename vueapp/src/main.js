// Components
import App from './App.vue'
import components from '@/components/components.js';

// Style
import './styles/index.scss'

// Composables
import { createApp } from 'vue'

// Plugins
import { registerPlugins } from '@/plugins'
import eventBus from '@/api/eventBus.js';

const app = createApp(App)



app.component('m-data-table', components.DataTable)
app.component('m-simple-2d-window', components.Simple2DWindow)
app.component('m-simple-3d-window', components.Simple3DWindow)
app.component('m-form-errors', components.FormErrors)
app.component('m-form-footer-buttons', components.FormFooterButtons)
app.component('m-record-details', components.RecordDetails)

app.component('m-numeric', components.NumericInput)

app.config.compilerOptions.isCustomElement = tag => tag === 'model-viewer'
app.config.globalProperties.$eventBus = eventBus

registerPlugins(app)

app.mount('#app')