import AccountService from '@/services/account-service'
import { CHANGE_PASSWORD } from './actions-type'

const actions = {
    [CHANGE_PASSWORD] ({commit}, payload) {
        return new Promise((resolve, reject) => {
            AccountService.changePassword(payload).then(() => {
                resolve()
            }).catch(() => {
                reject()
            })
        })
    }
}   

export default {
    actions
}