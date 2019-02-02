const state = {
    key: ''
}

const getters = {
    currentKey (state) {
        return state.key
    }
}

const mutations = {
    SET_KEY (state, payload) {
        state.key = payload
    },
    REMOVE_KEY (state) {
        state.key = ''
    }
}

const actions = {
    STORE_KEY ({commit}, payload) {
        commit('SET_KEY', payload)
    },
    DESTROY_KEY ({commit}) {
        commit('REMOVE_KEY')
    }
}

export default {
    state,
    actions,
    mutations,
    getters
}