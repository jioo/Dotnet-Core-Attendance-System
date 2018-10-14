<template lang="html">
    <v-container fluid fill-height>
        <v-layout align-center justify-center>
            <v-flex xs12 sm8 md4>
                <employee-login />
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
import EmployeeLogin from '@/components/employee-login'

export default {
    components: {
        EmployeeLogin
    }
}
</script>
