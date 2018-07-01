<template>
    <v-container fluid fill-height>
        <v-layout align-center justify-center>
            <v-flex xs12 sm8 md4>
                <change-password />
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
import ChangePassword from '@/components/change-password'

export default {
    components: {
        ChangePassword
    }
}
</script>

