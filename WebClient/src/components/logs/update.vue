<template>
    <v-flex md12 class="pt-4">
        <v-card class="elevation-12">
            <v-toolbar dark color="orange">
                <v-btn icon :loading="isLoading" :to="{ name: 'logs' }">
                    <v-icon>arrow_back</v-icon>
                </v-btn>
                <v-toolbar-title>Update Log</v-toolbar-title>
                <v-spacer></v-spacer>
            </v-toolbar>
            <v-card-text>
                <v-container grid-list-md>
                    <v-form ref="form">
                        <v-layout row wrap>
                            <v-flex md6>
                                <v-text-field
                                    box
                                    disabled
                                    label="Name" 
                                    color="orange"
                                    v-model="form.fullName"></v-text-field>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap>
                            <v-flex md3>
                                <v-text-field
                                    box
                                    disabled
                                    slot="activator"
                                    :value="computedInDate"
                                    readonly
                                    label="Time In"
                                ></v-text-field>
                            </v-flex>
                            <v-flex md3>
                                <v-menu
                                    ref="inTime"
                                    :close-on-content-click="false"
                                    :return-value.sync="inTime"
                                    lazy
                                    transition="scale-transition">
                                    <v-text-field
                                        box
                                        slot="activator"
                                        :value="computedTimeIn"
                                        readonly></v-text-field>
                                    <v-time-picker v-model="inTime" @change="$refs.inTime.save(inTime)"></v-time-picker>
                                </v-menu>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap v-if="!toggleTimeOut">
                            <v-flex md3>
                                <v-text-field
                                    box
                                    disabled
                                    slot="activator"
                                    :value="computedOutDate"
                                    readonly
                                    label="Time out"></v-text-field>
                            </v-flex>
                            <v-flex md3>
                                <v-menu
                                    ref="outTime"
                                    :close-on-content-click="false"
                                    :return-value.sync="outTime"
                                    lazy
                                    transition="scale-transition">
                                    <v-text-field
                                        box
                                        slot="activator"
                                        :value="computedOutTime"
                                        readonly
                                        ></v-text-field>
                                    <v-time-picker v-model="outTime" @change="$refs.outTime.save(outTime)"></v-time-picker>
                                </v-menu>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap v-else>
                            <v-flex md6>
                                <v-text-field disabled label="Time out"></v-text-field>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap>
                            <v-flex md12>
                                <v-checkbox v-model="toggleTimeOut" label="Clear Time Out"></v-checkbox>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap>
                            <v-btn color="success" :loading="isLoading" @click.prevent="submit">Update</v-btn>
                        </v-layout>
                    </v-form>
                </v-container>
            </v-card-text>
        </v-card>        
    </v-flex>
</template>

<script>
import { mapGetters } from 'vuex'
import { EventBus } from '@/event-bus.js'
import moment from 'moment'

export default {
    data () {
        return {
            form: {},
            toggleTimeOut: false,
            inDate: '',
            outDate: '',
            inTime: null,
            outTime: null
        }
    },
 
    computed: {
        ...mapGetters(["isLoading", "currentKey"]),

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
        async getDetails () {
            this.form = await axios.get(`log/${this.currentKey}`)
        },

        submit () {
            this.form.timeIn = this.computedInDate + ' ' + this.computedTimeIn
            this.form.timeOut = (!this.toggleTimeOut && this.computedOutDate && this.computedOutTime) 
                ? this.computedOutDate + ' ' + this.computedOutTime 
                : ''

            this.$axios.put('log', JSON.stringify(this.form)).then(() => {
                this.$notify({ type: 'success', text: 'Log has been successfully updated!' })
                this.$router.push({ name: 'logs' })
            })
        },

        initInDateTime () {
            const inDate = this.form.timeIn.split(' ')[0]
            const inTime = (this.form.timeIn)
                ? this.form.timeIn.split(' ')[1] + ' ' + this.form.timeIn.split(' ')[2]
                : this.form.timeIn
            this.inDate = this.format('date', inDate, false)
            this.inTime =  this.format('time', inTime, false)
        },

        initOutDateTime () {
            const outDate = this.form.timeOut.split(' ')[0]
            const outTime = (this.form.timeOut !== '') 
                ? this.form.timeOut.split(' ')[1] + ' ' + this.form.timeOut.split(' ')[2]
                : this.form.timeOut
            
            this.outDate = this.format('date', outDate, false)
            this.outTime =  this.format('time', outTime, false)
        },

        format(type, value, isOriginalFormat = true) {
            if (value === null || value === '') return null

            let originalFormat = (type === 'date') ? 'YYYY-MM-DD' : 'HH:mm:ss'
            let newFormat =  (type === 'date') ? 'MM/DD/YYYY' : 'h:mm:ss A'

            return (isOriginalFormat)
                ? moment(value, originalFormat).format(newFormat).toUpperCase()
                : moment(value, newFormat).format(originalFormat).toUpperCase()
        }
    },

    async mounted () {
        await this.getDetails()

        this.initInDateTime()
        this.initOutDateTime()
    }
}
</script>