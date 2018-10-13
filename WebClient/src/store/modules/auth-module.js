import JwtService from '@/services/jwt-service'

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
    SET_AUTH (state, data) {
        state.isAuthenticated = true
        state.user = data.user
        JwtService.saveToken(data.access_token)
    },
    DESTROY_AUTH (state) {
        state.isAuthenticated = false
        state.user = {}
        JwtService.destroyToken()
    }
}

const actions = {
    LOGIN ({commit}, data) {
        commit('SET_AUTH', data)
    },
    LOGOUT ({commit}) {
        commit('DESTROY_AUTH')
    }
}

export default {
    state,
    actions,
    mutations,
    getters
}
