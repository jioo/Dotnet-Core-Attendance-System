<template lang="html">
    <v-card class="elevation-12">
        <v-toolbar color="orange">
            <v-toolbar-title>Employee Log</v-toolbar-title>
            <v-spacer></v-spacer>
        </v-toolbar>

        <v-card-text>
        <v-form v-model="valid" ref="form">
            <v-text-field prepend-icon="person" label="CardNo" type="text" color="orange" ref="cardNo" autofocus
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
import { LOG_EMPLOYEE } from '@/store/actions-type'
import { EventBus } from '@/event-bus.js'
import * as signalR from "@aspnet/signalr";

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
                    if (res.TimeOut === '' ) {
                        this.$notify({ type: 'success', text: 'Welcome '+ res.FullName+ '!<br /> Time in: ' + res.TimeIn, duration: notifDuration })
                    } else {
                        this.$notify({ type: 'success', text: 'Time out: ' + res.TimeIn, duration: notifDuration })
                    }
                })
            }
        },
        resetForm () {
            this.$refs.form.reset()
        }
    },

    created () {
        EventBus.$emit('toggle-drawer')
    },

    mounted () {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5000/broadcast")
            .build();

        connection.on("send", (data) => {
            console.log(data)
        })

        connection.start().catch(err => document.write(err));
    }
}
</script>
