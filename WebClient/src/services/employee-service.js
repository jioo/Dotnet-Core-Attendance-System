import ApiService from './api-service'

export default {
    query () {
        return ApiService.query('employee')
    },

    get (id) {
        return ApiService.get('employee', id)
    },

    post (data) {
        return ApiService.post('accounts/register', data)
    },

    put (data) {
        return ApiService.put('employee', data)
    },

    destroy (id) {
        return ApiService.delete('employee', id)
    }
}
