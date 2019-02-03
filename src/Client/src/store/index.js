import Vue from 'vue'
import Vuex from 'vuex'
import createPersistedState from 'vuex-persistedstate'
import shared from './modules/shared-module'
import auth from './modules/auth-module'
import form from './modules/form-module'
import settings from './modules/settings-module'

Vue.use(Vuex)

export default new Vuex.Store({
    plugins: [
        createPersistedState()
    ],

    modules: {
        shared,
        auth,
        form,
        settings
    }
})