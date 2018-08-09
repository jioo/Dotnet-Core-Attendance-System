<template>
    <v-card class="elevation-12">
        <v-card-title > 
            Logs
            <v-spacer></v-spacer>
            <v-text-field append-icon="search" v-model="search" label="Search"
            single-line hide-details></v-text-field>
        </v-card-title>
        <v-data-table :headers="headers" :items="items" :search="search" :loading="isLoading" 
        disable-initial-sort :rows-per-page-items="[10]" :pagination.sync="pagination" hide-actions>
            <v-progress-linear slot="progress" indeterminate></v-progress-linear>
            <template slot="items" slot-scope="props">
                <td v-if="isRole('Admin')">{{ props.item.FullName }}</td>

                <td v-bind:class="{ red: isLate(props.item.TimeIn) }">
                    {{ props.item.TimeIn }} &nbsp; 
                    <v-chip outline color="white" v-if="isLate(props.item.TimeIn)" >{{ computeTimeInDiff(props.item.TimeIn) }}</v-chip>
                </td>
                <td v-bind:class="{ green: isUnderTime(props.item.TimeOut) }">
                    {{ props.item.TimeOut }} &nbsp; 
                    <v-chip outline color="white" v-if="isUnderTime(props.item.TimeOut)" >{{ computeTimeOutDiff(props.item.TimeOut) }}</v-chip>
                </td>

                <td v-if="isRole('Admin')">
                    <v-btn icon class="mx-0" @click="openDialog(props.item)">
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
import { mapGetters } from "vuex";
import { FETCH_LOGS } from '@/store/actions-type'
import { EventBus } from '@/event-bus.js'
import BroadcastConnection from '@/services/broadcast-service'
import moment from 'moment'

export default {
    data () {
        return {
            search: "",
            pagination: {},
            headers: [],
            items: [],
            gracePeriod: '',
            defaultTimeIn: '',
            defaultTimeOut: ''
        }
    },

    computed: {
        ...mapGetters(["logs", "currentUser", "settings", "isLoading"]),
        pages () {
        if (this.pagination.rowsPerPage == null ||
            this.pagination.totalItems == null
            ) return 0

            return Math.ceil(this.pagination.totalItems / this.pagination.rowsPerPage)
        }
    },

    methods: {
        openDialog (params) {
            EventBus.$emit('log-edit', params)
        },
        async initialize () {
            await this.$store.dispatch(FETCH_LOGS)
            this.items = this.logs
            this.headers = [
                { text: "Employee Name", value: "FullName" },
                { text: "Time In", value: "TimeIn" },
                { text: "Time Out", value: "TimeOut" },
                { text: 'Actions', sortable: false }
            ];

            if(this.isRole('Employee')) {
                const userId = this.currentUser.user.empId
                this.items = this.logs.filter(m => m.EmployeeId == userId)
                this.headers = [
                    { text: "Time In", value: "TimeIn" },
                    { text: "Time Out", value: "TimeOut" }
                ];
            }
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
        await this.initialize()
        this.initSettings()

        BroadcastConnection.on("employee-logged", () => {
            this.initialize()
        })
    }
}
</script>
