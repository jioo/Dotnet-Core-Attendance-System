<template>
    <v-dialog v-model="dialog" persistent max-width="500px">
        <v-btn slot="activator" color="primary" dark class="mb-2">Create</v-btn>
        <v-card>
            <v-progress-linear indeterminate v-if="isLoading"></v-progress-linear>
            <v-card-title>
                <span class="title">New Employee</span>
            </v-card-title>
            <v-card-text>
                <v-container grid-list-md>
                    <v-form v-model="valid" ref="form">
                        <v-layout wrap>
                            <v-flex md12>
                                <v-text-field v-model="form.fullname" label="Full Name" required :rules="[required]"></v-text-field>
                            </v-flex>
                            <v-flex md12>
                                <v-text-field v-model="form.cardno" label="Card No" required :rules="[required]"></v-text-field>
                            </v-flex>
                            <v-flex md12>
                                <v-text-field v-model="form.position" label="Position"></v-text-field>
                            </v-flex>
                            <v-flex md12>
                                <v-text-field v-model="form.username" label="Username" required :rules="[required]"></v-text-field>
                            </v-flex>
                            <v-flex md12>
                                <v-text-field v-model="form.password" label="Password" required :rules="[required, minLength]" type="password"></v-text-field>
                            </v-flex>
                        </v-layout>
                    </v-form>
                </v-container>
            </v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="error" :loading="isLoading" @click.native="dialog = false">Cancel</v-btn>
                <v-btn color="success" :loading="isLoading" @click="create">Save</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
import { mapGetters } from 'vuex'
import { EMPLOYEE_CREATE } from '@/store/actions-type'

export default {
    data () {
        return {
            valid: false,
            dialog: false,
            form: {
                username: '',
                password: '',
                fullname: '',
                cardno: '',
                position: ''
            },
            required: (value) => !!value || 'This field is required.',
            minLength: function (value) {
                if (value == null) return true

                if(value.length >= 6) {
                    return true
                }

                return 'Password must be atleast 6 characters.'
            }
        }
    },

    computed: {
        ...mapGetters(["isLoading"])
    },  

    methods: {
        create () {
            if (this.$refs.form.validate()) {
                this.$store.dispatch(EMPLOYEE_CREATE, JSON.stringify(this.form)).then(() => {
                    this.$notify({ type: 'success', text: 'Employee has been successfully created!' })
                    this.dialog = false
                })
            }
        },
        resetForm () {
            this.$refs.form.reset()
        }
    },

    watch: {
        dialog: function (val) {
            this.$refs.form.reset()
        }
    }
}
</script>
