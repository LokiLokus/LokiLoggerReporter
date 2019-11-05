import Vue from 'vue'
import App from './App.vue'
import router from './router'
import './registerServiceWorker'
import BootstrapVue from 'bootstrap-vue'
//import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import { library } from '@fortawesome/fontawesome-svg-core'
import { fas } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import Datetime from 'vue-datetime'
import 'vue-datetime/dist/vue-datetime.css'
import VueApexCharts from 'vue-apexcharts'



Vue.config.productionTip = false

Vue.use(VueApexCharts)

Vue.component('apexchart', VueApexCharts)
Vue.use(BootstrapVue)
Vue.use(Datetime)
library.add(fas)

Vue.component('font-awesome-icon', FontAwesomeIcon)

new Vue({
  router,
  render: h => h(App)
}).$mount('#app')
