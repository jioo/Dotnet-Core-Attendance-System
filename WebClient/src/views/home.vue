<template lang="html">
    <v-container fluid fill-height>
        <v-layout align-center justify-center>
            <v-flex xs12 sm8 md4>
                <log-employee-form />
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
import LogEmployeeForm from '@/components/log-employee-form'

export default {
    components: {
        LogEmployeeForm
    }
}
</script>
