<template lang="html">
    <v-card class="elevation-12">
        <v-toolbar color="orange">
            <v-toolbar-title>Employee Log</v-toolbar-title>
            <v-spacer></v-spacer>
        </v-toolbar>
        <v-card-text>
        <v-form v-model="valid" ref="form">
            <v-text-field prepend-icon="person" label="CardNo" type="text" autofocus color="orange"
            v-model="form.cardno" required :rules="[required]"></v-text-field>
            
            <v-text-field prepend-icon="lock" label="Password" type="password" color="orange"
            v-model="form.password" required :rules="[required]"></v-text-field>
        </v-form>
        </v-card-text>
        <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="orange" @click="logEmp">Login</v-btn>
        </v-card-actions>
    </v-card>
</template>

<script>
import { LOG_EMPLOYEE } from '@/store/actions-type'

export default {
    data () {
        return {
            valid: false,
            form: {
                cardno: '123456',
                password: '123456'
            },
            required: (value) => !!value || 'This field is required.'
        }
    },

    methods: {
        logEmp () {
            if (this.$refs.form.validate()) {
                this.$store.dispatch(LOG_EMPLOYEE, JSON.stringify(this.form)).then((res) => {
                    this.resetForm()
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
    }
}
</script>
