import Vue from 'vue'
import store from '@/store'
import Router from 'vue-router'

Vue.use(Router)

const router = new Router({
    mode: 'history',
    routes: [
        /*
         * ------------------------------------------------------
         *  Home
         * ------------------------------------------------------
         */
        {
            path: '/',
            name: 'home',
            component: () => import('@/views/home')
        },

        /*
         * ------------------------------------------------------
         *  Account login
         * ------------------------------------------------------
         */
        {
            path: '/login',
            name: 'login',
            component: () => import('@/views/login')
        },
        
        /*
         * ------------------------------------------------------
         *  Employees
         * ------------------------------------------------------
         */
        {
            path: '/employees', 
            component: () => import('./views/employee.vue'),
            children: [
                { 
                    path: '', 
                    name: 'employees', 
                    component: () => import('./components/employee/list.vue'), 
                    meta: { auth: true, roles: ['Admin'] } 
                },
                { 
                    path: 'create', 
                    component: () => import('./components/employee/create.vue'), 
                    meta: { auth: true, roles: ['Admin'] } 
                },
                { 
                    path: 'update', 
                    component: () => import('./components/employee/update.vue'), 
                    meta: { auth: true, roles: ['Admin'] }, 
                    beforeEnter: (to, from, next) => {
                        const key = store.getters.currentKey
                        if(!key) {
                            next('/employees')
                        } else {
                            next()
                        }
                    } 
                }
            ]
        },
        
        /*
         * ------------------------------------------------------
         *  Logs
         * ------------------------------------------------------
         */
        {
            path: '/logs', 
            component: () => import('./views/logs.vue'),
            children: [
                { 
                    path: '', 
                    name: 'logs', 
                    component: () => import('./components/logs/list.vue'), 
                    meta: { auth: true } 
                },
                { 
                    path: 'update', 
                    component: () => import('./components/logs/update.vue'), 
                    meta: { auth: true }, 
                    beforeEnter: (to, from, next) => {
                        const key = store.getters.currentKey
                        if(!key) {
                            next('/logs')
                        } else {
                            next()
                        }
                    } 
                }
            ]
        },

        /*
         * ------------------------------------------------------
         *  Account settings
         * ------------------------------------------------------
         */
        {
            path: '/settings',
            name: 'settings',
            meta: { auth: true },
            component: () => import('@/views/settings')
        }
    ]
})

// before each transition we will check if the route needs authentication or roles
router.beforeEach((to, from, next) => {
    // can't call Vue using 'this' variable inside route guards, 
    // so call the prototype instead..
    const vue = Vue.prototype

    // check if the user needs to be authenticated
    if (to.matched.some(m => m.meta.auth)) {
        
        // check if the route needs a specific role
        let requiredRole = ""
        if (to.matched.some(m => m.meta.roles)) {
            requiredRole = to.meta.roles
        }
        
        // challenge the user's token by sending a request
        // in api/challenge endpoint
        vue.$axios.post(`auth/challenge/${requiredRole}`).then(() => {
            // redirect to authenticated page
            next()
        }).catch(() => {
            // redirect unauthenticated user
            store.dispatch('LOGOUT').then(() => {
                return next('/login')
            })
        })
    }

    // skip authentication
    next()
})

export default router