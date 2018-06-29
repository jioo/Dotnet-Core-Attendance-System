import Vue from 'vue'
import Vuex from 'vuex'
import createPersistedState from 'vuex-persistedstate'

import auth from './auth-module'
import employee from './employee-module'
import log from './log-module'
import general from './general-module'

Vue.use(Vuex)

export default new Vuex.Store({
    plugins: [
        createPersistedState()
    ],
    
    modules: {
        auth,
        employee,
        log,
        general
    }
})