const state = {
    settings: {
        TimeIn: '9:00',
        TimeOut: '18:00',
        GracePeriod: '15'
    }
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
    }
}

const actions = {
    FETCH_SETTINGS ({commit}) {
        commit('SET_SETTINGS', {})
    },
    SETTINGS_EDIT ({commit}, payload) {
        commit('UPDATE_SETTINGS', payload)
    }
}

export default {
    state,
    actions,
    mutations,
    getters
}