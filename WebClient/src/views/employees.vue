<template>
    <v-container fluid fill-height>
        <v-layout align-center justify-center>
            <v-flex md12>
                <employee-create />
                <employee-edit />
                <employee-list :employees="employees" />
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
import { mapGetters } from 'vuex'
import { FETCH_EMPLOYEES } from '@/store/actions-type'
import EmployeeList from '@/components/employee-list'
import EmployeeCreate from '@/components/employee-create'
import EmployeeEdit from '@/components/employee-edit'

export default {
    components: {
        EmployeeList,
        EmployeeCreate,
        EmployeeEdit
    },

    computed: {
        ...mapGetters(["employees"])
    },

    methods: {
        initialize () {
            this.$store.dispatch(FETCH_EMPLOYEES)
        }
    },

    async created () {
        await this.initialize()
    }
}
</script>



