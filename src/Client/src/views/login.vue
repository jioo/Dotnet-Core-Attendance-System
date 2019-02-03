<template lang="html">
    <v-container fluid fill-height>
        <v-layout align-center justify-center>
            <v-flex xs12 sm8 md4>
                
                <v-card class="elevation-12">
                    <v-toolbar color="orange">
                        <v-toolbar-title>Account Login</v-toolbar-title>
                        <v-spacer></v-spacer>
                    </v-toolbar>
                    <v-card-text>
                    <v-form v-model="valid" ref="form">
                        <v-text-field 
                            label="Username"
                            type="text"
                            color="orange"
                            prepend-icon="person"   
                            autofocus 
                            v-model="form.username" 
                            required 
                            :rules="[required]"
                        ></v-text-field>
                        
                        <v-text-field 
                            label="Password"
                            type="password"
                            prepend-icon="lock"   
                            color="orange"
                            v-model="form.password" 
                            required 
                            :rules="[required]" 
                            v-on:keyup.enter.prevent="loginUser()"
                        ></v-text-field>
                    </v-form>
                    </v-card-text>
                    <v-card-actions>
                        <v-spacer></v-spacer>
                        <v-btn 
                            color="orange" 
                            :loading="isLoading" 
                            @click.prevent="loginUser"
                        >Login</v-btn>
                    </v-card-actions>
                </v-card>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
import { mapGetters } from 'vuex'

export default {
    data () {
        return {
            valid: false,
            form: {
                username: '',
                password: ''
            },
            required: (value) => !!value || 'This field is required.'
        }
    },

    computed: {
        ...mapGetters(['currentUser', 'isLoading'])
    },

    methods: {
        async loginUser () {
            if (this.$refs.form.validate()) {

                // authenticate user
                const result = await this.$axios.post('auth/login', JSON.stringify(this.form))
                await this.$store.dispatch('LOGIN', result)
                
                // set up default attendance settings
                const defaultSettings = await this.$axios.get('config')
                await this.$store.dispatch('SET_SETTINGS', defaultSettings)

                // redirect according to role
                const currentUserRole = result.user.roles
                if(currentUserRole.includes('Admin')) {
                    this.$router.push({ name: 'employees' })
                } else {
                    this.$router.push({ name: 'logs' })
                }
            }
        },

        async logout () {
            await this.$store.dispatch('REMOVE_SETTINGS')
            await this.$store.dispatch('LOGOUT')
        }
    },

    created () {
        this.logout()
    }
}
</script>