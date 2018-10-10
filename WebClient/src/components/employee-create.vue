<template>
    <v-dialog v-model="dialog" persistent max-width="500px">
        <v-btn slot="activator" color="primary" dark class="mb-2">Create</v-btn>
        <v-card>
            <v-toolbar dark color="orange">
                <v-toolbar-title>Create Employee</v-toolbar-title>
                <v-spacer></v-spacer>
            </v-toolbar>
            <v-card-text>
                <v-container grid-list-md>
                    <v-form v-model="valid" ref="form">
                        <v-layout wrap>
                            <v-flex md12>
                                <v-text-field v-model="form.fullName" label="Full Name" required :rules="[required]"></v-text-field>
                            </v-flex>
                            <v-flex md12>
                                <v-text-field v-model="form.cardNo" label="Card No" required :rules="[required]"></v-text-field>
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
                <v-btn color="success" :loading="isLoading" @click.prevent="create">Save</v-btn>
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
                fullName: '',
                cardNo: '',
                position: ''
            },
            required: (value) => !!value || 'This field is required.',
            minLength: function (value) {
                if(value == null || value.length >= 6) return true
                return 'Password must be atleast 6 characters.'
            },
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
