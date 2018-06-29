<template>
    <v-dialog v-model="dialog" max-width="500px">
        <v-btn slot="activator" color="primary" dark class="mb-2">Create</v-btn>
        <v-card>
            <v-card-title>
                <span class="title">New Employee</span>
            </v-card-title>
            <v-card-text>
                <v-container grid-list-md>
                    <v-layout wrap>
                        <v-flex md12>
                            <v-text-field v-model="form.fullname" label="Full Name"></v-text-field>
                        </v-flex>
                        <v-flex md12>
                            <v-text-field v-model="form.cardno" label="Card No"></v-text-field>
                        </v-flex>
                        <v-flex md12>
                            <v-text-field v-model="form.position" label="Position"></v-text-field>
                        </v-flex>
                        <v-flex md12>
                            <v-text-field v-model="form.username" label="Username"></v-text-field>
                        </v-flex>
                        <v-flex md12>
                            <v-text-field v-model="form.password" label="Password" type="password"></v-text-field>
                        </v-flex>
                    </v-layout>
                </v-container>
            </v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="error" @click.native="dialog = false">Cancel</v-btn>
                <v-btn color="success" @click="create">Save</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
import { EMPLOYEE_CREATE } from '@/store/actions-type'

export default {
    data () {
        return {
            dialog: false,
            form: {
                username: '',
                password: '',
                fullname: '',
                cardno: '',
                position: ''
            }
        }
    },

    methods: {
        create () {
            this.$store.dispatch(EMPLOYEE_CREATE, JSON.stringify(this.form)).then(() => {
                this.$notify({ type: 'success', text: 'Employee has been successfully created!' })
                this.dialog = false
            }).catch((err) => {
                this.$notify({ type: 'error', text: err })    
            })
        }
    }
}
</script>
