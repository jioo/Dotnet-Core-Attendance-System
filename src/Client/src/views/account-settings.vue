<template>
    <v-container fluid>
        <v-layout align-center justify-center>
            <v-flex md12>
                
                <v-card class="elevation-12">
                    <v-toolbar dark color="orange">
                        <v-toolbar-title>Settings</v-toolbar-title>
                        <v-spacer></v-spacer>
                    </v-toolbar>
                    <v-card-text>
                        <v-form v-model="valid" ref="form">

                            <v-layout row wrap>
                                <v-flex md12 class="my-4" >
                                    <h2 class="orange--text">Change Password</h2>
                                    <v-divider color="white"></v-divider>
                                </v-flex>
                            </v-layout>

                            <v-layout row wrap>
                                <v-flex md6>
                                    <v-text-field
                                        box
                                        label="Old Password" 
                                        type="password"
                                        color="orange"
                                        v-model="form.oldPassword"   
                                        required 
                                        :rules="[required]"
                                    ></v-text-field>
                                </v-flex>
                            </v-layout>

                            <v-layout row wrap>
                                <v-flex md6>
                                    <v-text-field
                                        box
                                        label="New Password" 
                                        type="password"
                                        color="orange"
                                        v-model="form.newPassword"   
                                        required 
                                        :rules="[required, minLength]"
                                    ></v-text-field>
                                </v-flex>
                            </v-layout>

                            <v-layout row wrap>
                                <v-flex md6>
                                    <v-text-field
                                        box
                                        label="Confirm Password" 
                                        type="password"
                                        color="orange"
                                        v-model="form.confirmPassword"   
                                        required 
                                        :rules="[required, equalToPassword]"
                                    ></v-text-field>
                                </v-flex>
                            </v-layout>

                            <v-layout row wrap>
                                <v-btn class="success" :loading="isLoading" @click.prevent="changePassword" >Submit</v-btn>
                            </v-layout>
                        </v-form>
                    </v-card-text>
                </v-card>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
import { mapGetters } from 'vuex'

export default {
    data () {
        return {
            valid: false,
            form: {},
            required: (value) => !!value || 'This field is required.',
            minLength: function (value) {
                if(value == null || value.length >= 6) return true
                return 'Password must be atleast 6 characters.'
            },
            equalToPassword: (value) => 
                (!!value && value == this.form.newPassword) || 
                'Please enter the same value again.',
        }
    },

    computed: {
        ...mapGetters(['isLoading', 'currentUser'])
    },

    methods: {
        changePassword () {
            if (this.$refs.form.validate()) {
                this.$axios.put('accounts/change-password', JSON.stringify(this.form)).then((res) => {
                    this.resetForm()
                    this.$notify({ type: 'success', text: 'Your password has been updated'})
                })
            }
        },

        resetForm () {
            this.$refs.form.reset()
        }
    },

    created () {
        this.form.username = this.currentUser.username
    }
}
</script>