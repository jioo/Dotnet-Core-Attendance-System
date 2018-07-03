import Vue from 'vue'
import store from '@/store'
import axios from 'axios'
import JwtService from './jwt-service'
import { BASE_URL } from '@/config.js'
import { LOADING_START, LOADING_END } from '@/store/actions-type'

const ApiService = {
    init () {
        axios.defaults.baseURL = BASE_URL

        // Add a request interceptor
        axios.interceptors.request.use((config) => {
            config['headers'] = {
                'Authorization': 'Bearer ' + JwtService.getToken(),
                'Content-Type': 'application/json'
            }
            return config;
        }, error => Promise.reject(error))

        // Add a response interceptor
        axios.interceptors.response.use((response) => {
            store.commit(LOADING_END)
            // always retorn data in response   
            return response.data;
        }, error => {
            store.commit(LOADING_END)
            return Promise.reject(error)
        })
    },

    query (resource) {
        store.commit(LOADING_START)
        return axios.get(resource)
    },

    get (resource, id) {
        store.commit(LOADING_START)
        return axios.get(`${resource}/${id}`)
    },

    post (resource, payload) {
        store.commit(LOADING_START)
        return new Promise((resolve, reject) => {
            axios.post(resource, payload).then((res) => {
                resolve(res) 
            }).catch((error) => {
                Vue.prototype.$notify({
                    type: 'error',
                    text: error.response.data,
                })
            })
        })

        
    },

    put (resource, payload) {
        store.commit(LOADING_START)
        return new Promise((resolve, reject) => {
            axios.put(resource, payload).then((res) => {
                resolve(res) 
            }).catch((error) => {
                Vue.prototype.$notify({
                    type: 'error',
                    text: error.response.data,
                })
            })
        })
    },

    delete (resource, id) {
        return axios.delete(`${resource}/${id}`).catch((error) => {
            throw new Error(`ApiService ${error}`)
        })
    }
}

export default ApiService
