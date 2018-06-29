import ApiService from './api-service'

export default {
    login (credentials) {
        return ApiService.post('auth/login', credentials)
    }
}
