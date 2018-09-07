import EmployeeService from '@/services/employee-service'
import { EMPLOYEE_CREATE, EMPLOYEE_EDIT, FETCH_EMPLOYEE, FETCH_EMPLOYEES } from './actions-type'
import { SET_EMPLOYEES, SET_EMPLOYEE, UPDATE_EMPLOYEE } from './mutations-type'

const state = {
    employees: {}
}

const getters = {
    employees (state) {
        return state.employees
    }
}

const mutations = {
    [SET_EMPLOYEES] (state, payload) {
        state.employees = payload
    },
    [SET_EMPLOYEE] (state, payload) {
        state.employees.unshift(payload)
    },
    [UPDATE_EMPLOYEE] (state, payload) {
        let index = state.employees.findIndex(x => x.id == payload.id)
        state.employees.splice(index, 1, payload)
    }
}

const actions = {
    [FETCH_EMPLOYEES] ({commit}) {
        return EmployeeService.query().then((data) => {
            commit(SET_EMPLOYEES, data)
        })
    },
    [FETCH_EMPLOYEE] ({state}, id) {
        return new Promise((resolve, reject) => {
            EmployeeService.get(id).then((res) => {
                resolve(res)
            }).catch((err) => {
                reject(err)
            })
        })
    },
    [EMPLOYEE_CREATE] ({commit}, payload) {
        return new Promise((resolve, reject) => {
            EmployeeService.post(payload).then((response) => {
                commit(SET_EMPLOYEE, response)
                resolve()
            })
        })
    },
    [EMPLOYEE_EDIT] ({commit}, payload) {
        return new Promise((resolve, reject) => {
            EmployeeService.put(payload).then((response) => {
                commit(UPDATE_EMPLOYEE, response)
                resolve()
            }).catch((err) => {
                reject(err)
            })
        })
    }
}

export default {
    state,
    actions,
    mutations,
    getters
}
