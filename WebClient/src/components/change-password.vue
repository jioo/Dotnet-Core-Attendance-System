<template>
    <v-card class="elevation-12">
        <v-card-text>
            <v-form v-model="valid" ref="form">
                <v-text-field label="Old Password" type="password" color="orange"
                v-model="form.oldPassword" required :rules="[required]"></v-text-field>
                
                <v-text-field label="New Password" type="password" color="orange"
                v-model="form.newPassword" required :rules="[required, minLength]"></v-text-field>

                <v-text-field label="Confirm Password" type="password" color="orange"
                v-model="form.confirmPassword" required :rules="[required, equalToPassword]"></v-text-field>
            </v-form>
        </v-card-text>
        <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="orange" :loading="isLoading" @click.prevent="changePassword" >Update</v-btn>
        </v-card-actions>
    </v-card>
</template>

<script>
import { mapGetters } from 'vuex'
import { CHANGE_PASSWORD } from '@/store/actions-type'

export default {
    data () {
        return {
            valid: false,
            form: {
                username: '',                
                oldPassword: '',
                newPassword: '',
                confirmPassword: ''
            },
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
        ...mapGetters(["isLoading", "currentUser"])
    },

    methods: {
        changePassword () {
            if (this.$refs.form.validate()) {
                this.$store.dispatch(CHANGE_PASSWORD, JSON.stringify(this.form)).then((res) => {
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
        this.form.username = this.currentUser.user.username
    }
}
</script>

