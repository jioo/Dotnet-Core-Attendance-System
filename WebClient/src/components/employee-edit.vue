<template>
    <v-dialog v-model="dialog" max-width="500px">
        <v-card>
            <v-card-title>
                <span class="title">Edit Employee</span>
            </v-card-title>
            <v-card-text>
                <v-container grid-list-md>
                    <v-layout wrap>
                        <v-flex md12>
                            <v-text-field :value="form.Identity.UserName" label="Username" disabled></v-text-field>
                        </v-flex>
                        <v-flex md12>
                            <v-text-field v-model="form.FullName" label="Full Name"></v-text-field>
                        </v-flex>
                        <v-flex md12>
                            <v-text-field v-model="form.CardNo" label="Card No"></v-text-field>
                        </v-flex>
                        <v-flex md12>
                            <v-text-field v-model="form.Position" label="Position"></v-text-field>
                        </v-flex>
                    </v-layout>
                </v-container>
            </v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="error" @click.native="dialog = false">Cancel</v-btn>
                <v-btn color="success" @click="edit">Update</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
import { EMPLOYEE_EDIT } from '@/store/actions-type'

export default {
    data () {
        return {
            dialog: false,
            form: {
                Identity: {}
            }
        }
    },

    methods: {
        openEditDialog (params) {
            this.form = params
            this.dialog = true
        },
        edit () {
            this.$store.dispatch(EMPLOYEE_EDIT, JSON.stringify(this.form)).then(() => {
                this.$notify({ type: 'success', text: 'Employee has been successfully updated!' })
                this.dialog = false
            })
        }
    }
}
</script>
