const ACCESS_TOKEN = 'access_token'

export default {
    getToken () {
        return window.localStorage.getItem(ACCESS_TOKEN)
    },

    saveToken (token) {
        window.localStorage.setItem(ACCESS_TOKEN, token)
    },
    
    destroyToken () {
        window.localStorage.removeItem(ACCESS_TOKEN)
    }
}
