import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

export default new Router({
    routes: [
        {
            path: '/',
            name: 'home',
            component: () => import('@/views/home')
        },
        {
            path: '/login',
            name: 'login',
            component: () => import('@/views/login')
        },
        {
            path: '/employees',
            name: 'employees',
            component: () => import('@/views/employees')
        },
        {
            path: '/logs',
            name: 'logs',
            component: () => import('@/views/logs')
        }
    ]
})
