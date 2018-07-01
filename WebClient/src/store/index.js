import Vue from 'vue'
import Vuex from 'vuex'
import createPersistedState from 'vuex-persistedstate'

import account from './account-module'
import auth from './auth-module'
import employee from './employee-module'
import log from './log-module'
import general from './general-module'
import settings from './settings-module'

Vue.use(Vuex)

export default new Vuex.Store({
    plugins: [
        createPersistedState()
    ],
    
    modules: {
        auth,
        employee,
        log,
        general,
        settings,
        account
    }
})