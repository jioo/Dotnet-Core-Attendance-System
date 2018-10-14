<template lang="html">
    <v-container fluid fill-height>
        <v-layout align-center justify-center>
            <v-flex xs12 sm8 md4>
                <login />
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
import Login from '@/components/login'

export default {
    components: {
        Login
    }
}
</script>