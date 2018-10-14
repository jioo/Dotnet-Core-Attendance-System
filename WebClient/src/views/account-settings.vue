<template>
    <v-container fluid>
        <v-layout align-center justify-center>
            <v-flex md12>
                <settings />
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
import Settings from '@/components/account/settings'

export default {
    components: {
        Settings
    }
}
</script>