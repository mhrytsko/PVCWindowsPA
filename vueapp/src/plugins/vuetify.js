// Styles
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'
//import colors from 'vuetify/lib/util/colors'

// Composables
import { createVuetify } from 'vuetify'

import * as components from 'vuetify/components'
import * as labsComponents from 'vuetify/labs/components'
import * as directives from 'vuetify/directives'


// https://vuetifyjs.com/en/introduction/why-vuetify/#feature-guides
export default createVuetify({
    components:{
      ...components,
      ...labsComponents
    },
    directives,
    theme: {
      defaultTheme: 'light',
      themes: {
        light: {
          colors: {
            primary: '#1867C0',
            secondary: '#5CBBF6',
            //surface: colors.grey.lighten4
          },
        },
      },
    },
  })