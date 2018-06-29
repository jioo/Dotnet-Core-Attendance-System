<template>
    <v-card class="elevation-12">
        <v-card-title > 
            Logs
            <v-spacer></v-spacer>
            <v-text-field append-icon="search" v-model="search" label="Search"
            single-line hide-details></v-text-field>
        </v-card-title>
        <v-data-table :headers="headers" :items="logs" :search="search" disable-initial-sort>
            <template slot="items" slot-scope="props">
                <td>{{ props.item.FullName }}</td>
                <td>{{ props.item.TimeIn }}</td>
                <td>{{ props.item.TimeOut }}</td>
                <td class="justify-center layout px-0">
                    <v-btn icon class="mx-0" @click="openDialog(props.item)">
                        <v-icon color="teal">edit</v-icon>
                    </v-btn>
                </td>
            </template>
        </v-data-table>
    </v-card>
</template>

<script>
import { mapGetters } from "vuex";

export default {
    props: ['logs'],
    data () {
        return {
            search: "",
            headers: [
                { text: "Employee Name", value: "FullName" },
                { text: "Time In", value: "TimeIn" },
                { text: "Time Out", value: "TimeOut" },
                { text: 'Actions', sortable: false }
            ]
        }
    },

    methods: {
        openDialog (params) {
            this.$emit('on-edit', params)
        }
    }
}
</script>