import LogService from '@/services/log-service'
import { FETCH_LOGS, FETCH_LOG, LOG_EMPLOYEE, LOG_EDIT, LOG_DELETE } from './actions-type'
import { SET_LOGS, SET_LOG, UPDATE_LOG } from './mutations-type'

const state = {
    logs: {}
}

const getters = {
    logs (state) {
        return state.logs
    }
}

const mutations = {
    [SET_LOGS] (state, payload) {
        state.logs = payload
    },
    [SET_LOG] (state, payload) {
        // state.logs = payload
    },
    [UPDATE_LOG] (state, payload) {
        let index = state.logs.findIndex(x => x.Id == payload.Id)
        state.logs.splice(index, 1, payload)
    }
}

const actions = {
    [FETCH_LOGS] ({commit}) {
        return LogService.query().then((response) => {
            commit(SET_LOGS, response)
        })
    },
    [FETCH_LOG] ({commit}, id) {
        return LogService.get(id)
    },
    [LOG_EMPLOYEE] ({commit}, payload) {
        return new Promise((resolve, reject) => {
            LogService.post(payload).then((response) => {
                commit(SET_LOG, response)
                resolve(response)
            }).catch((err) => {
                reject(err)
            })
        })
    },
    [LOG_EDIT] ({commit}, payload) {
        return new Promise((resolve, reject) => {
            LogService.put(payload).then((response) => {
                commit(UPDATE_LOG, response)
                resolve()
            }).catch((err) => {
                reject(err)
            })
        })
    },
    [LOG_DELETE] ({commit}, payload) {
        return LogService.destroy(payload)
    }

}

export default {
    state,
    actions,
    mutations,
    getters
}
