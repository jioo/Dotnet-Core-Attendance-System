<template>
    <v-card class="elevation-12">
        <v-toolbar dark color="orange">
            <v-toolbar-title>Logs</v-toolbar-title>
            <v-spacer></v-spacer>
        </v-toolbar>
        <v-card-title > 
            <log-list-filter v-on:onFilter="onFilter" />
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
                    <v-btn icon class="mx-0" @click="openDialog(props.item)">
                        <v-icon color="teal">edit</v-icon>
                    </v-btn>
                </td>
            </template>
        </v-data-table>
        
        <div class="text-xs-center pt-2" >
            <v-pagination v-model="logs.meta.page" :length="pages" :total-visible="7" @input="onPageChange"></v-pagination>
        </div>
    </v-card>
</template>

<script>
import { mapGetters } from "vuex";
import { FETCH_LOGS, RESET_META } from '@/store/actions-type'
import { EventBus } from '@/event-bus.js'
import BroadcastConnection from '@/services/broadcast-service'
import moment from 'moment'
import LogListFilter from '@/components/log-list-filter'

export default {
    components: {
        LogListFilter
    },

    data () {
        return {
            search: '',
            pagination: {},
            dateFilter: {},
            headers: [],
            items: [],
            gracePeriod: '',
            defaultTimeIn: '',
            defaultTimeOut: '',
            searchDelayTimer: 0,
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
        ...mapGetters(["logs", "currentUser", "settings", "isLoading"]),
        pages () {
            if (this.logs.meta.rowsPerPage == null ||
                this.logs.meta.totalItems == null) return 0

            return Math.ceil(this.logs.meta.totalItems / this.logs.meta.rowsPerPage)
        }
    },

    methods: {
        openDialog (params) {
            EventBus.$emit('log-edit', params)
        },

        async onPageChange (page) {
            this.logs.meta.page = page
            await this.getlist()
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

        async getlist (newMeta) {
            /* map object to produce: page=1&rowsPerPage=10... */
            let meta = newMeta || this.logs.meta;
            const metaUrl = Object.keys(meta)
                .map(key => {
                    const value = (meta[key] === 'null')
                        ? ''
                        : meta[key] || ''

                    return `${key}=${value}`
                }).join('&')
            
            await this.$store.dispatch(FETCH_LOGS, metaUrl).then(() => {
                this.items = this.logs.data
                console.table(this.items)

                this.headers = [
                    { text: "Employee Name", value: "fullName" },
                    { text: "Time In", value: "timeIn" },
                    { text: "Time Out", value: "timeOut" },
                    { text: 'Actions', sortable: false }
                ];

                if(this.isRole('Employee')) {
                    const userId = this.currentUser.user.empId
                    this.items = this.logs.data.filter(m => m.employeeId === userId)
                    this.headers = [
                        { text: "Time In", value: "timeIn" },
                        { text: "Time Out", value: "timeOut" }
                    ];
                }
            })
        },

        initSettings () {
            this.gracePeriod = this.settings.GracePeriod
            this.defaultTimeIn = moment(this.settings.TimeIn, 'LTS').add(this.gracePeriod, 'm')
            this.defaultTimeOut = moment(this.settings.TimeOut, 'LTS')
        },

        isRole (params) {
            const checkRole = this.currentUser.user.roles
            return checkRole.includes(params)
        },
        
        isLate (date) {
            if (!date) return
            const timeToCheck = this.formatDateToTime(date)
            
            // check if timeToCheck > default time in
            return timeToCheck.isAfter(this.defaultTimeIn)
        },

        isUnderTime (date) {
            if (!date) return
            const timeToCheck = this.formatDateToTime(date)

            // check if timeToCheck < default time out
            return timeToCheck.isBefore(this.defaultTimeOut)
        },

        computeTimeInDiff (date) {
            if (!date) return
            if (!this.isLate(date)) return

            const time = this.formatDateToTime(date)
            const difference = moment.duration(time.diff(this.defaultTimeIn));

            const result = difference.asMinutes().toFixed(0)
            return this.displayResult(result)
        }, 

        computeTimeOutDiff (date) {
            if (!date) return
            if (!this.isUnderTime(date)) return

            const time = this.formatDateToTime(date)
            const difference = moment.duration(this.defaultTimeOut.diff(time));

            const result = difference.asMinutes().toFixed(0)
            return this.displayResult(result)
        },

        formatDateToTime (dateString) {
            const timeString = dateString.split(' ')[1] + ' ' + dateString.split(' ')[2]
            return moment(timeString, 'LTS')
        },

        displayResult (result) {
            const hours = Math.floor(result / 60)
            let minutes = result % 60
            
            if (minutes < 10) {
                minutes= ("0" + minutes).slice(-2);
            }
            return hours + ':' + minutes
        }
    },

    async mounted () {
        await this.getlist()
        this.initSettings()

        BroadcastConnection.on("employee-logged", () => {
            this.getlist()
        })
    },

    created () {
        this.$store.dispatch(RESET_META)
    }
}
</script>
