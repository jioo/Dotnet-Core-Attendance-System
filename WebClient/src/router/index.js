import Vue from 'vue'
import store from '@/store'
import Router from 'vue-router'
import AuthService from '@/services/auth-service.js'
import { LOGOUT } from '@/store/actions-type'

Vue.use(Router)

const router = new Router({
    mode: 'history',
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
            meta: { auth: true, roles: ['Admin'] },
            component: () => import('@/views/employees')
        },
        {
            path: '/logs',
            name: 'logs',
            meta: { auth: true },
            component: () => import('@/views/logs')
        },
        {
            path: '/change-password',
            name: 'change-password',
            meta: { auth: true },
            component: () => import('@/views/change-password')
        }
    ]
})


// before each transition we check if the route need authentication or roles
router.beforeEach((to, from, next) => {

    // check if the user needs to be authenticated
    if (to.matched.some(m => m.meta.auth)) {

        AuthService.checkAuth().then(() => {
            // check if route requires role for authorization
            if (to.matched.some(m => m.meta.roles)) {
                let requiredRoles = to.meta.roles
                let query

                if (requiredRoles.includes('Admin')) {
                    query = AuthService.isAdmin()
                } else {
                    query = AuthService.isEmployee()
                }

                query.then(() => {
                    // user has valid roles
                    next()

                }).catch(() => {
                    // redirect unauthorized user
                    Vue.prototype.$notify({
                        type: 'error',
                        text: 'Unauthorized page!',
                    })
                    return next('/logs')
                })
            }

            next()

        }).catch(() => {
            // redirect unauthenticated user
            store.dispatch(LOGOUT).then(() => {
                Vue.prototype.$notify({
                    type: 'error',
                    text: 'Session has expired!',
                })
                return next('/login')
            })
        })
    }

    // authorize transition
    next()
})


// router.beforeResolve((to, from, next) => {
//     // If this isn't an initial page load.
//     if (to.name) {
//         // Start the route progress bar.
//         NProgress.start()
//     }
//     next()
// })

// router.afterEach((to, from) => {
//     // Complete the animation of the route progress bar.
//     NProgress.done()
// })

export default router
