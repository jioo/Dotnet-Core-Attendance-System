const state = {
    settings: {}
}

const getters = {
    settings (state) {
        return state.settings
    }
}

const mutations = {
    SET_SETTINGS (state, payload) {
        state.settings = payload
    },
    UPDATE_SETTINGS (state, payload) {
        let index = state.settings.findIndex(x => x.Id == payload.Id)
        state.settings.splice(index, 1, payload)
    },
    REMOVE_SETTINGS (state) {
        state.settings = {}
    }
}

const actions = {
    SET_SETTINGS ({commit}, payload) {
        commit('SET_SETTINGS', payload)
    },
    UPDATE_SETTINGS ({commit}, payload) {
        commit('UPDATE_SETTINGS', payload)
    },
    REMOVE_SETTINGS ({commit}) {
        commit('REMOVE_SETTINGS')
    }
}

export default {
    state,
    actions,
    mutations,
    getters
}