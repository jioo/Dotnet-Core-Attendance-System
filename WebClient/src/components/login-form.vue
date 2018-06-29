<template lang="html">
    <v-card class="elevation-12">
        <v-toolbar color="orange">
            <v-toolbar-title>Account Login</v-toolbar-title>
            <v-spacer></v-spacer>
        </v-toolbar>
        <v-card-text>
        <v-form v-model="valid" ref="form">
            <v-text-field prepend-icon="person" label="Username" type="text" autofocus color="orange"
            v-model="form.username" required :rules="[required]"></v-text-field>
            
            <v-text-field prepend-icon="lock" label="Password" type="password" color="orange"
            v-model="form.password" required :rules="[required]"></v-text-field>
        </v-form>
        </v-card-text>
        <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="orange" @click="loginUser">Login</v-btn>
        </v-card-actions>
    </v-card>
</template>

<script>
import { LOGIN } from '@/store/actions-type'

export default {
    data () {
        return {
            valid: false,
            form: {
                username: 'superadmin',
                password: '123456'
            },
            required: (value) => !!value || 'This field is required.'
        }
    },

    methods: {
        loginUser () {
            if (this.$refs.form.validate()) {
                this.$store.dispatch(LOGIN, JSON.stringify(this.form)).then((res) => {
                    this.$notify({ type: 'success', text: 'Login success!' })
                    this.$router.push({ name: 'employees' })
                })
            }
        }
    }
}
</script>

<style lang="css">
</style>
