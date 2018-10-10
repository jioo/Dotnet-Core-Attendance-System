<template>
    <v-container fluid>
        <v-layout align-center justify-center>
            <v-flex md12>
                <log-edit />
                <log-list />
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
import LogList from '@/components/log-list'
import LogEdit from '@/components/log-edit'

export default {
    components: {
        LogList,
        LogEdit
    },
}
</script>



