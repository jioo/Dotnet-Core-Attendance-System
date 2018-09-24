const XSRF_TOKEN = 'xsrf_token'
import axios from 'axios'

export default {
    getToken () {
        return window.localStorage.getItem(XSRF_TOKEN)
    },

    setToken() {
        axios.get("auth/xsrfToken").then(res => {
            window.localStorage.setItem(XSRF_TOKEN, res.data.result.token)
        })
    }
}