import AuthService from '@/services/auth-service'
import JwtService from '@/services/jwt-service'
import { LOGIN, LOGOUT } from './actions-type'
import { SET_AUTH, DESTROY_AUTH } from './mutations-type'

const state = {
    user: {},
    isAuthenticated: !!JwtService.getToken()
}

const getters = {
    currentUser (state) {
        return state.user
    },
    isAuthenticated (state) {
        return state.isAuthenticated
    }
}

const mutations = {
    [SET_AUTH] (state, user) {
        state.isAuthenticated = true
        state.user = user
        JwtService.saveToken(state.user.access_token)
    },
    [DESTROY_AUTH] (state) {
        state.isAuthenticated = false
        state.user = {}
        JwtService.destroyToken()
    }
}

const actions = {
    [LOGIN] ({commit}, credentials) {
        return new Promise((resolve) => {
            AuthService.login(credentials).then((data) => {
                commit(SET_AUTH, data)
                resolve(data)
            })
        })
    },
    [LOGOUT] ({commit}) {
        return new Promise((resolve) => {
            commit(DESTROY_AUTH)
            resolve()
        })
    }
}

export default {
    state,
    actions,
    mutations,
    getters
}
