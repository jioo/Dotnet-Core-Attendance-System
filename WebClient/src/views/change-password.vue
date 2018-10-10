<template>
    <v-container fluid>
        <v-layout align-center justify-center>
            <v-flex md6>
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

