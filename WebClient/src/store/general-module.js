import { LOADING_START, LOADING_END  } from './actions-type'
import { COMMIT_LOADING_START, COMMIT_LOADING_END } from './mutations-type'

const state = {
    isLoading: false
}

const getters = {
    isLoading (state) {
        return state.isLoading
    }
}

const mutations = {
    [COMMIT_LOADING_START] (state) {
        state.isLoading = true
    },
    [COMMIT_LOADING_END] (state) {
        state.isLoading = false
    }
}

const actions = {
    [LOADING_START] ({commit}) {
        commit(COMMIT_LOADING_START)
    },
    [LOADING_END] ({commit}) {
        commit(COMMIT_LOADING_END)
    }
}

export default {
    state,
    actions,
    mutations,
    getters
}