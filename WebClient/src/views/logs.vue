<template>
    <v-container fluid fill-height>
        <v-layout align-center justify-center>
            <v-flex md12>
                <log-edit ref='logEdit' />
                <log-list :logs="logs" v-on:on-edit="openEditDialog" />
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
import { mapGetters } from 'vuex'
import { FETCH_LOGS } from '@/store/actions-type'
import LogList from '@/components/log-list'
import LogEdit from '@/components/log-edit'

export default {
    components: {
        LogList,
        LogEdit
    },

    computed: {
        ...mapGetters(["logs"])
    },

    methods: {
        initialize () {
            this.$store.dispatch(FETCH_LOGS)
        },
        openEditDialog (params) {
            this.$refs.logEdit.openEditDialog(params);
        }
    },

    async created () {
        await this.initialize()
    }
}
</script>



