import Vue from 'vue'
import store from '@/store'
import axios from 'axios'
import JwtService from '@/services/jwt-service'

// Full config: https://github.com/axios/axios#request-config
axios.defaults.baseURL = process.env.VUE_APP_API_URL

let config = {
    // timeout: 60 * 1000, // Timeout
    // withCredentials: true, // Check cross-site Access-Control
}

const _axios = axios.create(config)
const defaultErrorMessage = "An error has occured."

// Add a request interceptor
_axios.interceptors.request.use(
    function (config) {
        // Do something before request is sent
        store.dispatch('LOADING_START')
        // Set up request headers
        config['headers'] = {
            'Authorization': 'Bearer ' + JwtService.getToken(),
            'Content-Type': 'application/json'
        }
        return config
    },
    function (error) {
        // Do something with request error
        store.dispatch('LOADING_END')
        // Global error notification
        Vue.prototype.$notify({
            type: 'error',
            text: error.response.data || defaultErrorMessage,
        })
        return Promise.reject(error)
    }
)

// Add a response interceptor
_axios.interceptors.response.use(
    function (response) {
        // Do something with response data
        store.dispatch('LOADING_END')
        // always return [data] in response
        return response.data  
    },
    function (error) {
        // Do something with response error
        store.dispatch('LOADING_END')
        let message = ""
        
        // Get response status code
        const statusCode = error.response.status
        switch(statusCode) {
            // Unauthenticated user
            case 401:
                message = "Session has expired"
                break;
            
            // Unauthorized user
            case 403:
                message = "You don't have permission to this page"
                break;

            default:
                message = error.response.data || defaultErrorMessage
                break;
        }
        
        // Global error notification
        Vue.prototype.$notify({
            type: 'error',
            text: message,
        })
        return Promise.reject(error)
    }
)

Plugin.install = function (Vue, options) {
    Vue.axios = _axios
    window.axios = _axios
    Object.defineProperties(Vue.prototype, {
        axios: {
            get() {
                return _axios
            }
        },
        $axios: {
            get() {
                return _axios
            }
        },
    })
}

Vue.use(Plugin)
export default Plugin