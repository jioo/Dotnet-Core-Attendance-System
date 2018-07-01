<template>
    <v-card class="elevation-12">
        <v-card-title > 
            Employee List
            <v-spacer></v-spacer>
            <v-text-field append-icon="search" v-model="search" label="Search"
            single-line hide-details></v-text-field>
        </v-card-title>
        <v-data-table :headers="headers" :items="employees" :search="search" :loading="isLoading" 
        disable-initial-sort :rows-per-page-items="[10]" :pagination.sync="pagination" hide-actions>
            <v-progress-linear slot="progress" indeterminate></v-progress-linear>
            <template slot="items" slot-scope="props">
                <td>{{ props.item.FullName }}</td>
                <td>{{ props.item.Position }}</td>
                <td>{{ props.item.CardNo }}</td>
                <td>
                    <v-btn icon class="mx-0" @click="openDialog(props.item.Id)">
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

export default {
    props: ['employees'],
    data () {
        return {
            search: "",
            pagination: {},
            headers: [
                { text: "Employee Name", value: "FullName" },
                { text: "Position", value: "Position" },
                { text: "Card No", value: "CardNo" },
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