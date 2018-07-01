import ApiService from './api-service'

export default {
    changePassword (credentials) {
        return ApiService.post('accounts/change-password', credentials)
    }
}
