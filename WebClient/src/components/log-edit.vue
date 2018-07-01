<template>
    <v-dialog v-model="dialog" persistent max-width="500px">
        <v-card>
            <v-progress-linear indeterminate v-if="isLoading"></v-progress-linear>
            <v-card-title>
                <span class="title">Edit Log</span>
            </v-card-title>
            <v-card-text>
                <v-container grid-list-md>
                    <v-layout row wrap>
                        <v-form ref="form">
                        
                        <v-flex md12>
                            <v-menu
                                :close-on-content-click="false"
                                ref="inDate"
                                lazy
                                transition="scale-transition">
                                <v-text-field
                                    slot="activator"
                                    :value="computedInDate"
                                    readonly
                                    label="Time In"
                                ></v-text-field>
                                <v-date-picker v-model="inDate" no-title @input="$refs.inDate.save(inDate)"></v-date-picker>
                            </v-menu>
                            <v-menu
                                ref="inTime"
                                :close-on-content-click="false"
                                :return-value.sync="inTime"
                                lazy
                                transition="scale-transition">
                                <v-text-field
                                    slot="activator"
                                    :value="computedTimeIn"
                                    readonly></v-text-field>
                                <v-time-picker v-model="inTime" @change="$refs.inTime.save(inTime)"></v-time-picker>
                            </v-menu>
                        </v-flex>
                        
                        <v-flex md12 v-if="!toggleTimeOut">
                            <v-menu
                                :close-on-content-click="false"
                                ref="outDate"
                                lazy
                                transition="scale-transition">
                                <v-text-field
                                    slot="activator"
                                    :value="computedOutDate"
                                    readonly
                                    label="Time out"></v-text-field>
                                <v-date-picker v-model="outDate" no-title @input="$refs.outDate.save(outDate)"></v-date-picker>
                            </v-menu>
                            <v-menu
                                ref="outTime"
                                :close-on-content-click="false"
                                :return-value.sync="outTime"
                                lazy
                                transition="scale-transition">
                                <v-text-field
                                    slot="activator"
                                    :value="computedOutTime"
                                    readonly
                                    ></v-text-field>
                                <v-time-picker v-model="outTime" @change="$refs.outTime.save(outTime)"></v-time-picker>
                            </v-menu>
                        </v-flex>

                        <v-flex md10 v-else>
                            <v-text-field disabled label="Time out"></v-text-field>
                        </v-flex>

                        <v-flex md12>
                            <v-checkbox v-model="toggleTimeOut" label="Clear Time Out"></v-checkbox>
                        </v-flex>

                        </v-form>
                    </v-layout>
                </v-container>
            </v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="error" :loading="isLoading" @click.native="dialog = false">Cancel</v-btn>
                <v-btn color="success" :loading="isLoading" @click="edit">Update</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script>
import { mapGetters } from 'vuex'
import { LOG_EDIT } from '@/store/actions-type'
import { EventBus } from '@/event-bus.js'
import moment from 'moment'

export default {
    data () {
        return {
            toggleTimeOut: false,
            dialog: false,
            form: {},
            inDate: '',
            outDate: '',
            inTime: null,
            outTime: null
        }
    },
 
    computed: {
        ...mapGetters(["isLoading"]),

        computedInDate () {
            return this.format('date', this.inDate)
        },
        computedOutDate () {
            return this.format('date', this.outDate)
        },
        computedTimeIn () {
            return this.format('time', this.inTime)
        },
        computedOutTime () {
            return this.format('time', this.outTime)
        }
    },

    watch: {
        toggleTimeOut (val) {
            if (val) {
                this.outDate = ''
                this.outTime = ''
            } else {
                this.initOutDateTime()
            }
        }
    },

    methods: {
        openEditDialog (params) {
            this.form = params
            this.dialog = true
            
            this.initInDateTime()
            this.initOutDateTime()
        },
        edit () {
            this.form.TimeIn = this.computedInDate + ' ' + this.computedTimeIn
            this.form.TimeOut = (!this.toggleTimeOut && this.computedOutDate && this.computedOutTime) 
                ? this.computedOutDate + ' ' + this.computedOutTime 
                : ''
            
            this.$store.dispatch(LOG_EDIT, JSON.stringify(this.form)).then(() => {
                this.$refs.form.reset()
                this.$notify({ type: 'success', text: 'Log has been successfully updated!' })
                this.dialog = false
            })
        },
        format(type, value, isOriginalFormat = true) {
            if (value === null || value === '') return null

            let originalFormat = (type === 'date') ? 'YYYY-MM-DD' : 'HH:mm:ss'
            let newFormat =  (type === 'date') ? 'MM/DD/YYYY' : 'h:mm:ss A'

            return (isOriginalFormat)
                ? moment(value, originalFormat).format(newFormat).toUpperCase()
                : moment(value, newFormat).format(originalFormat).toUpperCase()
        },
        initInDateTime () {
            const inDate = this.form.TimeIn.split(' ')[0]
            const inTime = (this.form.TimeIn)
                ? this.form.TimeIn.split(' ')[1] + ' ' + this.form.TimeIn.split(' ')[2]
                : this.form.TimeIn
            this.inDate = this.format('date', inDate, false)
            this.inTime =  this.format('time', inTime, false)
        },
        initOutDateTime () {
            const outDate = this.form.TimeOut.split(' ')[0]
            const outTime = (this.form.TimeOut !== '') 
                ? this.form.TimeOut.split(' ')[1] + ' ' + this.form.TimeOut.split(' ')[2]
                : this.form.TimeOut
            
            this.outDate = this.format('date', outDate, false)
            this.outTime =  this.format('time', outTime, false)
        }
    },

    mounted () {
        EventBus.$on('log-edit', (params) => {
            this.openEditDialog(params)
        })
    }
}
</script>
