<template>
    <v-flex md12 class="pt-4">
        <v-card class="elevation-12">
            <v-toolbar dark color="orange">
                <v-toolbar-title>Logs</v-toolbar-title>
                <v-spacer></v-spacer>
            </v-toolbar>

            <v-card-title> 
                <list-filter v-on:onFilter="onFilter" />
            </v-card-title>
            
            <v-data-table 
                :items="items" 
                :rows-per-page-items="[10]"  
                disable-initial-sort  
                :loading="isLoading" 
                :hide-actions="true" 
                :headers="headers" 
                :pagination.sync="pagination">
                <v-progress-linear slot="progress" indeterminate></v-progress-linear>

                <template slot="items" slot-scope="props">
                    <td v-if="isRole('Admin')">{{ props.item.fullName }}</td>

                    <td v-bind:class="{ red: isLate(props.item.timeIn) }">
                        {{ props.item.timeIn }} &nbsp; 
                        <v-chip outline color="white" v-if="isLate(props.item.timeIn)" >{{ computeTimeInDiff(props.item.timeIn) }}</v-chip>
                    </td>
                    <td v-bind:class="{ green: isUnderTime(props.item.timeOut) }">
                        {{ props.item.timeOut }} &nbsp; 
                        <v-chip outline color="white" v-if="isUnderTime(props.item.timeOut)" >{{ computeTimeOutDiff(props.item.timeOut) }}</v-chip>
                    </td>

                    <td v-if="isRole('Admin')">
                        <v-btn icon class="mx-0" @click="update(props.item.id)">
                            <v-icon color="teal">edit</v-icon>
                        </v-btn>
                    </td>
                </template>
            </v-data-table>
            <div class="text-xs-center pt-2" >
                <v-pagination v-model="logs.meta.page" :length="pages" :total-visible="7" @input="onPageChange"></v-pagination>
            </div>
        </v-card>
    </v-flex>
</template>

<script>
import { mapGetters } from "vuex"
import { EventBus } from '@/event-bus.js'
import ListFilter from './list-filter'
import BroadcastConnection from '@/services/broadcast-service'
import moment from 'moment'

export default {
    components: {
        ListFilter
    },

    data () {
        return {
            logs: {
                meta: {},
                data: []
            },
            search: '',
            pagination: {},
            dateFilter: {},
            headers: [],
            items: [],
            config: {},
        }
    },

    watch: {
        pagination: {
            handler(newValue, oldValue){
                if(newValue.sortBy !== oldValue.sortBy ||
                   newValue.descending !== oldValue.descending)
                {
                    const newMeta = Object.assign({}, this.logs.meta)
                    newMeta.sortBy = newValue.sortBy
                    newMeta.descending = newValue.descending
                    
                    this.getlist(newMeta)
                }
            },
            deep: true
        }
    },

    computed: {
        ...mapGetters(["currentUser", "settings", "isLoading"]),
        pages () {
            const { rowsPerPage, totalItems } = this.logs.meta

            if (rowsPerPage == null || totalItems == null) return 0

            return Math.ceil(totalItems / rowsPerPage)
        }
    },

    methods: {
        update (data) {
            this.$store.dispatch('STORE_KEY', data)
            this.$router.push({ path: '/logs/update' })
        },

        setDefaultSettings () {
            const { TimeIn, TimeOut, GracePeriod } = this.settings
            this.config = {
                gracePeriod: GracePeriod,
                timeIn: moment(TimeIn, 'LTS').add(GracePeriod, 'm'),
                timeOut: moment(TimeOut, 'LTS')
            } 
        },

        setDefaultMeta () {
            this.logs.meta = {
                "search": null,
                "descending": null,
                "page": 1,
                "rowsPerPage": 10,
                "sortBy": null,
                "startDate": '',
                "endDate": ''
            }
        },

        onFilter (payload) {
            const { startDate, endDate, search } = payload
            const newMeta = Object.assign({}, this.logs.meta)
            newMeta.startDate = startDate
            newMeta.endDate = endDate
            newMeta.search = search
            
            console.log(newMeta)
            this.getlist(newMeta)
        },

        async onPageChange (page) {
            this.logs.meta.page = page
            await this.getlist()
        },

        getlist (newMeta) {
            
            let meta = newMeta || this.logs.meta

            // map object to produce: page=1&rowsPerPage=10...
            const metaUrl = Object.keys(meta)
                    .map(key => {
                        const value = (meta[key] === 'null')
                            ? ''
                            : meta[key] || ''

                        return `${key}=${value}`
                    }).join('&')
            
            // get request based on metaUrl
            this.$axios.get(`log?${metaUrl}`).then((res) => {
                this.items = res.data
                this.logs = res

                this.headers = [
                    { text: "Employee Name", value: "fullName" },
                    { text: "Time In", value: "timeIn" },
                    { text: "Time Out", value: "timeOut" },
                    { text: 'Actions', sortable: false }
                ]

                if(this.isRole('Employee')) {
                    const userId = this.currentUser.empId
                    this.items = this.logs.data.filter(m => m.employeeId === userId)
                    this.headers = [
                        { text: "Time In", value: "timeIn" },
                        { text: "Time Out", value: "timeOut" }
                    ]
                }
            })
        },

        isRole (params) {
            const checkRole = this.currentUser.roles
            return checkRole.includes(params)
        },
        
        isLate (date) {
            if (!date) return

            const time = this.formatDateToTime(date)
            const defaultTimeIn = this.config.timeIn

            return time.isAfter(defaultTimeIn)
        },

        isUnderTime (date) {
            if (!date) return

            const time = this.formatDateToTime(date)
            const defaultTimeOut = this.config.timeOut

            return time.isBefore(defaultTimeOut)
        },

        computeTimeInDiff (date) {
            if (!date || !this.isLate(date)) return

            const time = this.formatDateToTime(date)
            const defaultTimeIn = this.config.timeIn
            const difference = moment.duration(time.diff(defaultTimeIn))

            return this.displayResult(difference)
        }, 

        computeTimeOutDiff (date) {
            if (!date || !this.isUnderTime(date)) return

            const time = this.formatDateToTime(date)
            const defaultTimeOut = this.config.timeOut
            const difference = moment.duration(defaultTimeOut.diff(time))

            return this.displayResult(difference)
        },

        formatDateToTime (dateString) {
            const time = `${dateString.split(' ')[1]} ${dateString.split(' ')[2]}`
            return moment(time, 'LTS')
        },

        displayResult (difference) {
            const result = difference.asMinutes().toFixed(0)
            const hours = Math.floor(result / 60)
            let minutes = result % 60
            
            if (minutes < 10) minutes = ("0" + minutes).slice(-2)
            
            return `${hours}:${minutes}`
        }
    },

    mounted () {
        this.getlist()
        this.setDefaultSettings()

        BroadcastConnection.on("employee-logged", () => {
            this.getlist()
        })
    },

    created () {
        this.setDefaultMeta()
    }
}
</script>   