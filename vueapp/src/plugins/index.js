// Plugins
import { loadFonts } from './webfontloader'
import vuetify from './vuetify'

import router from '../router/router.js'
import pinia from '../store/store.js'

export function registerPlugins (app) {
  loadFonts()
  app
    .use(vuetify)
    .use(pinia)
    .use(router)
}
