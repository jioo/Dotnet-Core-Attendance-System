<template>
    <v-flex md12 class="pt-4">
        <v-card class="elevation-12">
            <v-toolbar dark color="orange">
                <v-btn icon :loading="isLoading" :to="{ name: 'employees' }">
                    <v-icon>arrow_back</v-icon>
                </v-btn>
                <v-toolbar-title>Create</v-toolbar-title>
                <v-spacer></v-spacer>
            </v-toolbar>
            <v-card-text>
                <v-container grid-list-md>
                    <v-form v-model="valid" ref="form">

                        <v-layout row wrap>
                            <v-flex md12 class="my-4" >
                                <h2 class="orange--text">Employee Information</h2>
                                <v-divider color="white"></v-divider>
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
                                    :rules="[required]"></v-text-field>
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
                                    :rules="[required]"></v-text-field>
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
                                    :rules="[required]"></v-text-field>
                            </v-flex>
                        </v-layout>
                        
                        <v-layout row wrap>
                            <v-flex md12 class="my-4" >
                                <h2 class="orange--text">Account Information</h2>
                                <v-divider color="white"></v-divider>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap>
                            <v-flex md6>
                                <v-text-field
                                    box 
                                    label="Username"
                                    color="orange"
                                    v-model="form.username"
                                    required 
                                    :rules="[required]"></v-text-field>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap>
                            <v-flex md6>
                                <v-text-field
                                    box 
                                    label="Password"
                                    type="password"
                                    color="orange"
                                    v-model="form.password"
                                    required 
                                    :rules="[required]"></v-text-field>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap>
                            <v-btn color="success" :loading="isLoading" @click.prevent="submit">Submit</v-btn>
                        </v-layout>
                    </v-form>
                </v-container>
            </v-card-text>
        </v-card>
    </v-flex>
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
        }
    },

    computed: {
        ...mapGetters(["isLoading"])
    },  

    methods: {
        submit () {
            if (this.$refs.form.validate()) {
                this.$axios.post('accounts/register', JSON.stringify(this.form)).then(() => {
                    this.$notify({ type: 'success', text: 'Employee has been successfully created!' })
                    this.$router.push({ name: 'employees' })
                })
            }
        }
    }
}
</script>