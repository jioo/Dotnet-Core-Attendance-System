import ApiService from './api-service'

export default {
    query (meta) {
        return ApiService.query(`log?${meta}`)
    },

    get (id) {
        return ApiService.get('log', id)
    },

    post (data) {
        return ApiService.post('log', data)
    },

    put (data) {
        return ApiService.put('log', data)
    },

    destroy (id) {
        return ApiService.delete('log', id)
    }
}
