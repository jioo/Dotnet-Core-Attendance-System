<template lang="html">
    <v-card class="elevation-12">
        <v-toolbar color="orange">
            <v-toolbar-title>Employee Log</v-toolbar-title>
            <v-spacer></v-spacer>
        </v-toolbar>

        <v-card-text>
        <v-form v-model="valid" ref="form">
            <v-text-field prepend-icon="person" label="Card No." type="text" color="orange" ref="cardNo" autofocus
            v-model="form.cardno" required :rules="[required]" v-on:keyup.enter="$refs.password.focus()"></v-text-field>
            
            <v-text-field prepend-icon="lock" label="Password" type="password" color="orange" ref="password"
            v-model="form.password" required :rules="[required]" v-on:keyup.enter="logEmp()"></v-text-field>
        </v-form>
        </v-card-text>
        <v-card-actions>    
            <v-spacer></v-spacer>   
            <v-btn color="orange" :loading="isLoading" @click="logEmp">Login</v-btn>
        </v-card-actions>
    </v-card>
</template>

<script>
import { mapGetters } from 'vuex'
import * as signalR from "@aspnet/signalr";
import { LOG_EMPLOYEE, LOGOUT } from '@/store/actions-type'
import { EventBus } from '@/event-bus.js'

export default {
    data () {
        return {
            valid: false,
            form: {
                cardno: '',
                password: ''
            },
            required: (value) => !!value || 'This field is required.'
        }
    },

    computed: {
        ...mapGetters(["isLoading"])
    },

    methods: {
        logEmp () {
            if (this.$refs.form.validate()) {
                this.$store.dispatch(LOG_EMPLOYEE, JSON.stringify(this.form)).then((res) => {
                    this.resetForm()
                    this.$refs.cardNo.focus()
                    const notifDuration = 4000
                    if (res.timeOut === '' ) {
                        this.$notify({ type: 'success', text: 'Welcome '+ res.fullName+ '!<br /> Time in: ' + res.timeIn, duration: notifDuration })
                    } else {
                        this.$notify({ type: 'success', text: 'Time out: ' + res.timeIn, duration: notifDuration })
                    }
                })
            }
        },
        resetForm () {
            this.$refs.form.reset()
        },
        logout () {
            this.$store.dispatch(LOGOUT).then()
        }
    },

    created () {
        EventBus.$emit('toggle-drawer')
        this.logout()
    }
}
</script>
