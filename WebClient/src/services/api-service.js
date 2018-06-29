import Vue from 'vue'
import axios from 'axios'
import JwtService from './jwt-service'

const ApiService = {
    init () {
        axios.defaults.baseURL = 'http://localhost:5000/api/'

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
            // always retorn data in response
            return response.data;
        }, error => Promise.reject(error))
    },

    query (resource) {
        return axios.get(resource).catch((error) => {
            throw new Error(`ApiService ${error}`)
        })
    },

    get (resource, id) {
        return axios.get(`${resource}/${id}`).catch((error) => {
            throw new Error(`ApiService ${error}`)
        })
    },

    post (resource, payload) {
        return new Promise((resolve, reject) => {
            axios.post(resource, payload).then((res) => {
                resolve(res) 
            }).catch((error) => {
                reject(error.response.data)
            })
        })
    },

    put (resource, payload) {
        return new Promise((resolve, reject) => {
            axios.put(resource, payload).then((res) => {
                resolve(res) 
            }).catch((error) => {
                reject(error.response.data)
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
