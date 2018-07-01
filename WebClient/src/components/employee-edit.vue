<template>
    <v-dialog v-model="dialog" persistent max-width="500px">
        <v-card>
            <v-progress-linear indeterminate v-if="isLoading"></v-progress-linear>
            <v-card-title>
                <span class="title">Edit Employee</span>
            </v-card-title>
            <v-card-text>
                <v-container grid-list-md>
                    <v-form v-model="valid" ref="form">
                        <v-layout wrap>
                            <v-flex md12>
                                <v-text-field v-model="form.FullName" label="Full Name" required :rules="[required]"></v-text-field>
                            </v-flex>
                            <v-flex md12>
                                <v-text-field v-model="form.CardNo" label="Card No" required :rules="[required]"></v-text-field>
                            </v-flex>
                            <v-flex md12>
                                <v-text-field v-model="form.Position" label="Position"></v-text-field>
                            </v-flex>
                        </v-layout>
                    </v-form>
                </v-container>
            </v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="error" :loading="isLoading" @click.native="dialog = false">Cancel</v-btn>
                <v-btn color="success" :loading="isLoading" @click="edit">Update</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
import { mapGetters } from 'vuex'
import { EMPLOYEE_EDIT, FETCH_EMPLOYEE } from '@/store/actions-type'
import { EventBus } from '@/event-bus.js'

export default {
    data () {
        return {
            valid: false,
            dialog: false,
            form: {},
            required: (value) => !!value || 'This field is required.',
        }
    },

    computed: {
        ...mapGetters(["isLoading"])
    },

    methods: {
        openEditDialog (id) {
            this.$store.dispatch(FETCH_EMPLOYEE, id).then((res) => {
                this.form = res
                this.dialog = true
            })
        },
        edit () {
            if (this.$refs.form.validate()) {
                this.$store.dispatch(EMPLOYEE_EDIT, JSON.stringify(this.form)).then(() => {
                    this.$notify({ type: 'success', text: 'Employee has been successfully updated!' })
                    this.dialog = false
                })
            }
        }
    },

    mounted () {
        EventBus.$on('employee-edit', (id) => {
            this.openEditDialog(id)
        })
    }
}
</script>
