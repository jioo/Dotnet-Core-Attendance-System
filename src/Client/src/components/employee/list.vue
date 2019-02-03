<template>
    <v-flex md12 class="pt-4">
        <v-card class="elevation-12">
            <v-toolbar dark color="orange">
                <v-toolbar-title>Employee List</v-toolbar-title>
                <v-spacer></v-spacer>
                <v-btn color="success" :to="{ path: '/employees/create' }">Create</v-btn>
            </v-toolbar>
            <v-card-title > 
                <v-spacer></v-spacer>
                <v-text-field
                    label="Search" 
                    append-icon="search" 
                    v-model="search" 
                    single-line 
                    hide-details
                ></v-text-field>
            </v-card-title>
            
            <v-data-table 
                :headers="headers" 
                :items="items" 
                :search="search" 
                :loading="isLoading" 
                disable-initial-sort 
                :rows-per-page-items="[10]" 
                :pagination.sync="pagination" 
                hide-actions
            >
                <v-progress-linear slot="progress" indeterminate></v-progress-linear>

                <template slot="items" slot-scope="props">
                    <td>{{ props.item.identity.userName }}</td>
                    <td>{{ props.item.fullName }}</td>
                    <td>{{ props.item.position }}</td>
                    <td>{{ props.item.cardNo }}</td>
                    <td>
                        <v-btn 
                            icon 
                            class="mx-0" 
                            @click.prevent="update(props.item.id)"
                        >
                            <v-icon color="teal">edit</v-icon>
                        </v-btn>
                    </td>
                </template>
            </v-data-table>
            
            <div class="text-xs-center pt-2" >
                <v-pagination 
                    v-model="pagination.page" 
                    :length="pages" 
                    :total-visible="7"
                ></v-pagination>
            </div>
        </v-card>
    </v-flex>
</template>

<script>
import { mapGetters } from 'vuex'
import { EventBus } from '@/event-bus.js'

export default {
    data () {
        return {
            items: [],
            search: '',
            pagination: {},
            headers: [
                { text: 'Username', value: 'identity.userName' },
                { text: 'Employee Name', value: 'fullName' },
                { text: 'Position', value: 'position' },
                { text: 'Card No', value: 'cardNo' },
                { text: 'Actions', sortable: false }
            ]
        }
    },

    computed: {
        ...mapGetters(['isLoading']),
        pages () {
        if (this.pagination.rowsPerPage == null ||
            this.pagination.totalItems == null
            ) return 0

            return Math.ceil(this.pagination.totalItems / this.pagination.rowsPerPage)
        }
    },

    methods: {
        update (data) {
            this.$store.dispatch('STORE_KEY', data)
            this.$router.push({ path: '/employees/update' })
        },

        async getList () {
            this.items = await this.$axios.get('employee')
        }
    },

    watch: {
        items (val) {
            this.pagination.totalItems = val.length
        }
    },

    mounted () {
        this.getList()
    }
}
</script>