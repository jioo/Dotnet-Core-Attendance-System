<template lang="html">
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
                :rules="[required]"></v-text-field>
            
            <v-text-field 
                label="Password"
                type="password"
                prepend-icon="lock"   
                color="orange"
                v-model="form.password" 
                required 
                :rules="[required]" 
                v-on:keyup.enter.prevent="loginUser()"></v-text-field>
        </v-form>
        </v-card-text>
        <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn 
                color="orange" 
                :loading="isLoading" 
                @click.prevent="loginUser">Login</v-btn>
        </v-card-actions>
    </v-card>
</template>

<script>
import { mapGetters } from "vuex"

export default {
    data () {
        return {
            valid: false,
            form: {
                username: 'admin',
                password: '123456'
            },
            required: (value) => !!value || 'This field is required.'
        }
    },

    computed: {
        ...mapGetters(["currentUser", "isLoading"])
    },

    methods: {
        loginUser () {
            if (this.$refs.form.validate()) {

                this.$axios.post('auth/login', JSON.stringify(this.form)).then((res) => {    
                    this.$store.dispatch('LOGIN', res)
                    const currentUserRole = res.user.roles
                    if(currentUserRole.includes('Admin')) {
                        this.$router.push({ name: 'employees' })
                    } else {
                        this.$router.push({ name: 'logs' })
                    }
                })
            }
        },

        logout () {
            this.$store.dispatch('LOGOUT')
        }
    },

    created () {
        this.logout()
    }
}
</script>