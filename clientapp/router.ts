import Vue from 'vue'
import Router from 'vue-router'
import Layout from './components/Layout.vue'
import Dashboard from './components/routes/Dashboard.vue'
import Sources from './components/routes/Sources.vue'
import LogAnalyzer from './components/routes/LogAnalyzer.vue'
import RestUsageAnalyzer from './components/routes/RestUsageAnalyzer.vue'
Vue.use(Router)

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      redirect: '/home',
      component: Layout,
      children: [
        {
          path: '/home',
          name: 'home',
          component: Dashboard,
        },
        {
          path: '/sources',
          name: 'sources',
          component: Sources,
        },
        {
          path: '/loganalyzer',
          name: 'loganalyzer',
          component: LogAnalyzer,
        },
        {
          path: 'restusageanalyzer',
          name: 'restusageanalyzer',
          component: RestUsageAnalyzer,
        },
      ],
    }
  ]
})
