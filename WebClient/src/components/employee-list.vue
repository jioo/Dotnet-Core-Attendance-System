<template>
    <v-card class="elevation-12">
        <v-card-title > 
            Employee List
            <v-spacer></v-spacer>
            <v-text-field append-icon="search" v-model="search" label="Search"
            single-line hide-details></v-text-field>
        </v-card-title>
        <v-data-table :headers="headers" :items="employees" :search="search" disable-initial-sort>
            <template slot="items" slot-scope="props">
                <td>{{ props.item.FullName }}</td>
                <td>{{ props.item.Position }}</td>
                <td>{{ props.item.CardNo }}</td>
                <td>{{ props.item.Status }}</td>
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
    props: ['employees'],
    data () {
        return {
            search: "",
            headers: [
                { text: "Employee Name", value: "FullName" },
                { text: "Position", value: "Position" },
                { text: "Card No", value: "CardNo" },
                { text: "Status", value: "Status" },
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