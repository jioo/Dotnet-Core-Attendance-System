<template lang="html">
    <div>
        <v-toolbar app fixed clipped-left>
            <v-toolbar-side-icon @click.stop="drawer = !drawer"></v-toolbar-side-icon>
            <v-toolbar-title>ATTENDANCE SYSTEM</v-toolbar-title>
            <v-spacer></v-spacer>
        </v-toolbar>
        
        <v-navigation-drawer v-model="drawer" clipped fixed app>
            <v-list dense>
                <v-list-tile v-if="isAuthenticated">
                    <v-list-tile-content>
                        <v-list-tile-title>Logged in as <b class="orange--text"> {{ currentUser.user.fullName }}</b></v-list-tile-title>
                    </v-list-tile-content>
                </v-list-tile>

                <v-subheader class="mt-3 grey--text text--darken-1">MAIN NAVIGATION</v-subheader>

                <v-list-tile exact :to="{ name: 'home' }" v-if="!isAuthenticated"
                v-ripple active-class="orange--text" >
                    <v-list-tile-action>
                        <v-icon>home</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-content>
                        <v-list-tile-title>Home</v-list-tile-title>
                    </v-list-tile-content>
                </v-list-tile>

                <v-list-tile exact :to="{ name: 'employees' }" v-if="isAuthenticated && isRole('Admin')"
                v-ripple active-class="orange--text" >
                    <v-list-tile-action>
                        <v-icon>supervisor_account</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-content>
                        <v-list-tile-title>Employees</v-list-tile-title>
                    </v-list-tile-content>
                </v-list-tile>

                <v-list-tile exact :to="{ name: 'logs' }" v-if="isAuthenticated"
                v-ripple active-class="orange--text" >
                    <v-list-tile-action>
                        <v-icon>list</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-content>
                        <v-list-tile-title>Logs</v-list-tile-title>
                    </v-list-tile-content>
                </v-list-tile>

                <v-subheader class="mt-3 grey--text text--darken-1">ACCOUNT</v-subheader>

                <v-list-tile exact :to="{ name: 'login' }" v-if="!isAuthenticated"
                v-ripple active-class="orange--text" >
                    <v-list-tile-action>
                        <v-icon>account_circle</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-content>
                        <v-list-tile-title>Login</v-list-tile-title>
                    </v-list-tile-content>
                </v-list-tile>

                <v-list-tile exact :to="{ name: 'change-password' }" v-if="isAuthenticated"
                v-ripple active-class="orange--text" >
                    <v-list-tile-action>
                        <v-icon>settings</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-content>
                        <v-list-tile-title>Change Password</v-list-tile-title>
                    </v-list-tile-content>
                </v-list-tile>

                <v-list-tile @click="logout" v-if="isAuthenticated"
                v-ripple active-class="orange--text" >
                    <v-list-tile-action>
                        <v-icon>power_settings_new</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-content>
                        <v-list-tile-title>Logout</v-list-tile-title>
                    </v-list-tile-content>
                </v-list-tile>
            </v-list>
        </v-navigation-drawer>
    </div>
</template>

<script>
import { mapGetters } from "vuex";
import { LOGOUT } from '@/store/actions-type'
import { EventBus } from '@/event-bus.js'

export default {
    data() {
        return {
            drawer: true,
            items: [
                { icon: "home", title: "Home", to: "home", visible: true },
                { icon: "supervisor_account", title: "Login", to: "login", visible: !this.isAuthenticated }
            ]
        };
    },

    computed: {
        ...mapGetters(["isAuthenticated", "currentUser"])
    },

    methods: {
        logout () {
            this.$store.dispatch(LOGOUT).then(() => {
                this.$router.push({ name: 'home' })
            })
        },
        isRole(param) {
            if(!this.currentUser) return

            let check = this.currentUser.user.roles
            return check.includes(param)
        }
    },

    created () {
        EventBus.$on('toggle-drawer', () => {
            this.drawer = false
        })
    }
}
</script>
