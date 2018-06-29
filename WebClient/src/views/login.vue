<template lang="html">
    <v-container fluid fill-height>
        <v-layout align-center justify-center>
            <v-flex xs12 sm8 md4>
                <login-form />
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
import LoginForm from '@/components/login-form'

export default {
    components: {
        LoginForm
    }
}
</script>
