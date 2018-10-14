<template>
    <v-flex md12 class="pt-4">
        <v-card class="elevation-12">
            <v-toolbar dark color="orange">
                <v-btn icon :loading="isLoading" :to="{ name: 'employees' }">
                    <v-icon>arrow_back</v-icon>
                </v-btn>
                <v-toolbar-title>Update</v-toolbar-title>
                <v-spacer></v-spacer>
            </v-toolbar>
            <v-card-text>
                <v-container grid-list-md>
                    <v-form v-model="formStates.form1.isValid" :ref="formStates.form1.name">

                        <v-layout row wrap>
                            <v-flex md12 class="my-4" >
                                <h2 class="orange--text">Update Information</h2>
                                <v-divider color="white"></v-divider>
                                <v-progress-linear :indeterminate="true" class="ma-0" v-show="formStates.form1.isLoading"></v-progress-linear>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap>
                            <v-flex md6>
                                <v-text-field
                                    box
                                    label="Name" 
                                    color="orange"
                                    v-model="form.fullName"
                                    required 
                                    :rules="[required]"
                                    :disabled="!formStates.form1.isEdit"></v-text-field>
                            </v-flex>
                            <v-flex md2>
                                <v-btn icon large dark class="mx-0" 
                                :color="!formStates.form1.isEdit ? 'teal' : 'grey'"
                                @click.prevent.native="toggleEditState(formStates.form1.name)">
                                    <v-icon>{{ !formStates.form1.isEdit ? 'edit' : 'close'  }}</v-icon>
                                </v-btn>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap>
                             <v-flex md6>
                                <v-text-field
                                    box 
                                    label="Card No"
                                    color="orange"
                                    v-model="form.cardNo"
                                    required 
                                    :rules="[required]"
                                    :disabled="!formStates.form1.isEdit"></v-text-field>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap>
                            <v-flex md6>
                                <v-text-field
                                    box 
                                    label="Position"
                                    color="orange"
                                    v-model="form.position"
                                    required 
                                    :rules="[required]"
                                    :disabled="!formStates.form1.isEdit"></v-text-field>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap>
                            <v-scale-transition>
                                <v-btn 
                                    color="success" 
                                    :loading="formStates.form1.isLoading"
                                    v-show="formStates.form1.isEdit"
                                    @click.prevent="submit({
                                        scope: formStates.form1.name, 
                                        url: 'employee',
                                        successMessage: 'Employee Information has been successfully updated!'
                                    })">Submit</v-btn>
                            </v-scale-transition>
                        </v-layout>
                    </v-form>

                    <v-form v-model="formStates.form2.isValid" :ref="formStates.form2.name">

                        <v-layout row wrap>
                            <v-flex md12 class="my-4" >
                                <h2 class="orange--text">Update Password</h2>
                                <v-divider color="white"></v-divider>
                                <v-progress-linear :indeterminate="true" class="ma-0" v-show="formStates.form2.isLoading"></v-progress-linear>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap>
                            <v-flex md6>
                                <v-text-field
                                    box 
                                    label="New Password"
                                    type="Password"
                                    color="orange"
                                    v-model="form.newPassword"
                                    required 
                                    :rules="[required, minLength]"
                                    :disabled="!formStates.form2.isEdit"></v-text-field>
                            </v-flex>
                            <v-flex md2>
                                <v-btn icon large dark class="mx-0" 
                                :color="!formStates.form2.isEdit ? 'teal' : 'grey'"
                                @click.prevent.native="toggleEditState(formStates.form2.name)">
                                    <v-icon>{{ !formStates.form2.isEdit ? 'edit' : 'close'  }}</v-icon>
                                </v-btn>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap>
                            <v-scale-transition>
                                <v-btn 
                                    color="success" 
                                    :loading="formStates.form2.isLoading"
                                    v-show="formStates.form2.isEdit"
                                    @click.prevent="submit({
                                        scope: formStates.form2.name, 
                                        url: 'accounts/update-password',
                                        successMessage: 'Password has been successfully updated!'
                                    })">Submit</v-btn>
                            </v-scale-transition>
                        </v-layout>

                    </v-form>
                </v-container>
            </v-card-text>
        </v-card>
    </v-flex>
</template>

<script>
import { mapGetters } from 'vuex'
import store from '@/store'

export default {
    data () {
        return {
            originalData: {},
            form: {},
            formStates: {
                form1: {
                    name: 'form1',
                    isValid: false,
                    isLoading: false,
                    isEdit: false,
                },
                form2: {
                    name: 'form2',
                    isValid: false,
                    isLoading: false,
                    isEdit: false,
                }
            },
            required: (value) => !!value || 'This field is required.',
            minLength: function (value) {
                if(value == null || value.length >= 6) return true
                return 'Password must be atleast 6 characters.'
            }
        }
    },

    computed: {
        ...mapGetters(["isLoading", "currentKey"])
    },

    methods: {
        async getDetails () {
            this.originalData = await axios.get(`employee/${this.currentKey}`)
            this.originalData.userName = this.originalData.identity.userName
        },

        resetToOriginalData () {
            this.form = Object.assign({}, this.originalData)
        },

        toggleEditState (scope) {
            const state = this.formStates[scope].isEdit
            this.formStates[scope].isEdit = !state
            
            if (scope === 'form2') this.$refs[scope].reset()
            if (state) this.resetToOriginalData()
        },

        async submit (data) {
            const { scope, url, successMessage } = data

            // Check if all fields are valid within the scope
            if (this.$refs[scope].validate()) {
                this.formStates[scope].isLoading = true

                // Send put request
                await this.$axios.put(url, JSON.stringify(this.form)).then(() => {
                    this.$notify({ type: 'success', text: successMessage })
                    this.formStates[scope].isEdit = false
                })
                
                // Update the originalData
                await this.getDetails()
                this.formStates[scope].isLoading = false
            }
        }
    },

    async mounted () {
        await this.getDetails();
        this.resetToOriginalData();
    },

    beforeRouteLeave (to, from, next) {
        store.dispatch('DESTROY_KEY')
        next()
    }
}
</script>