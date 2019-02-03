import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from '@/store'

import './plugins/vuetify'
import './plugins/axios'
import './plugins/vee-validate'
import './plugins/vue-notification'
import './plugins/vuetify'

Vue.config.productionTip = false


new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
