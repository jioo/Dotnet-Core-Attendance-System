<template>
    <v-card class="elevation-12">
        <v-toolbar dark color="orange">
            <v-toolbar-title>Employee List</v-toolbar-title>
            <v-spacer></v-spacer>
            <employee-create />
        </v-toolbar>
        <v-card-title > 
            <v-spacer></v-spacer>
            <v-text-field append-icon="search" v-model="search" label="Search"
            single-line hide-details></v-text-field>
        </v-card-title>
        
        <v-data-table 
            :headers="headers" 
            :items="employees" 
            :search="search" 
            :loading="isLoading" 
            disable-initial-sort 
            :rows-per-page-items="[10]" 
            :pagination.sync="pagination" 
            hide-actions>
            <v-progress-linear slot="progress" indeterminate></v-progress-linear>

            <template slot="items" slot-scope="props">
                <td>{{ props.item.fullName }}</td>
                <td>{{ props.item.position }}</td>
                <td>{{ props.item.cardNo }}</td>
                <td>
                    <v-btn icon class="mx-0" @click.prevent="openDialog(props.item.id)">
                        <v-icon color="teal">edit</v-icon>
                    </v-btn>
                </td>
            </template>
        </v-data-table>
        <div class="text-xs-center pt-2" >
            <v-pagination v-model="pagination.page" :length="pages" :total-visible="7"></v-pagination>
        </div>
    </v-card>
</template>

<script>
import { mapGetters } from 'vuex'
import { EventBus } from '@/event-bus.js'
import EmployeeCreate from '@/components/employee-create'

export default {
    props: ['employees'],

    components: {
        EmployeeCreate
    },

    data () {
        return {
            search: "",
            pagination: {},
            headers: [
                { text: "Employee Name", value: "fullName" },
                { text: "Position", value: "position" },
                { text: "Card No", value: "cardNo" },
                { text: 'Actions', sortable: false }
            ]
        }
    },

    computed: {
        ...mapGetters(["isLoading"]),
        pages () {
        if (this.pagination.rowsPerPage == null ||
            this.pagination.totalItems == null
            ) return 0

            return Math.ceil(this.pagination.totalItems / this.pagination.rowsPerPage)
        }
    },

    methods: {
        openDialog (id) {
            EventBus.$emit('employee-edit', id)
        }
    }
}
</script>