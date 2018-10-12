import LogService from '@/services/log-service'
import { FETCH_LOGS, FETCH_LOG, LOG_EMPLOYEE, LOG_EDIT, LOG_DELETE, RESET_META } from './actions-type'
import { SET_LOGS, SET_LOG, UPDATE_LOG, UPDATE_META } from './mutations-type'
import moment from 'moment'

const state = {
    logs: {
        data: [],
        meta: {
            "search": null,
            "descending": null,
            "page": 1,
            "rowsPerPage": 10,
            "sortBy": null,
            "startDate": '',
            "endDate": ''
        }
    }
}

const getters = {
    logs (state) {
        return state.logs
    }
}

const mutations = {
    [SET_LOGS] (state, payload) {
        let { startDate, endDate } = payload.meta
        if (startDate) startDate = moment(startDate).format('YYYY-MM-DD')
        if (endDate) endDate = moment(endDate).format('YYYY-MM-DD')

        payload.meta = {
            ...payload.meta,
            startDate: startDate,
            endDate: endDate
        }

        state.logs = payload
    },
    [SET_LOG] (state, payload) {
        state.logs.data = payload
    },
    [UPDATE_LOG] (state, payload) {
        let index = state.logs.data.findIndex(x => x.id == payload.id)
        if (index === -1) return
        state.logs.data.splice(index, 1, payload)
    },
    [UPDATE_META] (state, payload) {
        state.logs.meta = payload
    }
}

const actions = {
    [FETCH_LOGS] ({commit}, meta) {
        return LogService.query(meta).then((response) => {
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
    },
    [RESET_META] ({commit}) {
        commit(UPDATE_META, {
            "search": null,
            "descending": null,
            "page": 1,
            "rowsPerPage": 10,
            "sortBy": null,
            "startDate": '',
            "endDate": ''
        })
    }
}

export default {
    state,
    actions,
    mutations,
    getters
}
