import ApiService from './api-service'

export default {
    login (credentials) {
        return ApiService.post('auth/login', credentials)
    },
    csrfToken() {
        return ApiService.get('xsrfToken').then((response) => {
            console.log('token: ', response.token)
            return response.token
        })
    },
    checkAuth() {
        return ApiService.query('auth/check')
    },
    isAdmin() {
        return ApiService.query('auth/is-admin')
    },
    isEmployee() {
        return ApiService.query('auth/is-employee')
    }
}
